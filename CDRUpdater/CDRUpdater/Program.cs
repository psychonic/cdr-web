using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SteamKit2;
using MySql.Data.MySqlClient;
using System.Data;

namespace CDRUpdater
{
    class Program
    {
        const string HASHFILE = "CDR.hash";
        const string CDRBLOB = "CDR.blob";
        const string SQL_CONNECTION_STRING = "connection_string.cfg";

        static void Main(string[] args)
        {
            string connection_string = File.ReadAllText(SQL_CONNECTION_STRING);

            DebugLog.Write("CDR updater running!\n");

            DebugLog.Write("Downloading CDR...\n");

            byte[] cdr = null;
            try
            {
                byte[] hash = null;
                try
                {
                    hash = File.ReadAllBytes(HASHFILE);
                }
                catch (Exception ex2)
                {
                    DebugLog.Write("Warning: Unable to read cdr hashfile: {0}\n", ex2.ToString());
                }

                cdr = Downloader.DownloadCDR(hash);

                if (cdr == null || cdr.Length == 0)
                {
                    DebugLog.Write("No new CDR. All done!\n\n");
                    return;
                }

                File.WriteAllBytes(CDRBLOB, cdr);
            }
            catch (Exception ex)
            {
                DebugLog.Write("Unable to download CDR: {0}\n", ex.ToString());
                return;
            }

            //byte[] cdr = File.ReadAllBytes(CDRBLOB);
            byte[] current_hash = CryptoHelper.SHAHash(cdr);

            string hash_hex = current_hash.Aggregate(new StringBuilder(),
                       (sb, v) => sb.Append(v.ToString("x2"))
                      ).ToString();

            try
            {
                File.WriteAllBytes(HASHFILE, current_hash);
            }
            catch (Exception ex2)
            {
                DebugLog.Write("Warning: Unable to write to hashfile: {0}\n", ex2.ToString());
            }

            DebugLog.Write("Parsing CDR...\n");

            CDR CDRBlob = null;

            using (BlobTypedReader<CDR> BlobReader = BlobTypedReader<CDR>.Create(CDRBLOB))
            {
                BlobReader.Process();

                CDRBlob = BlobReader.Target;
            }

            if (CDRBlob == null)
            {
                DebugLog.Write("Unable to parse CDR.");
                return;
            }

            DebugLog.Write("Building updates..\n");


            MySqlConnection connection = new MySqlConnection(connection_string);
            connection.Open();

            MySqlCommand command;


            // insert CDR
            command = connection.CreateCommand();

            // the current CDR is always the most recently processed
            command.CommandText = "SELECT id FROM cdr ORDER BY date_processed DESC LIMIT 1";

            int prev_cdr_id = Convert.ToInt32(command.ExecuteScalar());

            command.CommandText = String.Format("INSERT INTO cdr (hash, version, date_updated, date_processed, app_count, sub_count) VALUES ('{0}', {1}, '{2}', '{3}', {4}, {5})",
                                                    hash_hex, CDRBlob.VersionNum, String.Format("{0:u}", CDRBlob.LastChangedExistingAppOrSubscriptionTime.ToDateTime()),
                                                    String.Format("{0:u}", DateTime.Now),
                                                    CDRBlob.Apps.Count,
                                                    CDRBlob.Subs.Count);

            command.ExecuteNonQuery();

            command.CommandText = "SELECT last_insert_id()";

            int cdr_id = Convert.ToInt32(command.ExecuteScalar());


            // capture app data and add app_* tables
            DataSet data = new DataSet();
            command = connection.CreateCommand();
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);


            using (StreamWriter sw_app = new StreamWriter(new FileStream("app.data", FileMode.Create)))
            using (StreamWriter sw_app_capture = new StreamWriter(new FileStream("app_capture.data", FileMode.Create)))
            using (StreamWriter sw_app_filesystem = new StreamWriter(new FileStream("app_filesystem.data", FileMode.Create)))
            using (StreamWriter sw_app_version = new StreamWriter(new FileStream("app_version.data", FileMode.Create)))
            {
                foreach (App app in CDRBlob.Apps)
                {
                    command.CommandText = "SELECT * FROM app WHERE app_id = " + app.AppID;
                    data.Reset();
                    adapter.Fill(data);

                    DataRow app_info = null;

                    if(data.Tables[0].Rows.Count > 0)
                        app_info = data.Tables[0].Rows[0];
                    
                    string app_current_data = null;
                    string app_state_data = null;

                    SQLQuery.BuildDataInsertFromTypeWithChanges(app, app_info, cdr_id, prev_cdr_id, out app_current_data, out app_state_data);

                    sw_app.WriteLine(app_current_data);

                    if (app_state_data != null)
                    {
                        sw_app_capture.WriteLine(app_state_data);
                    }
                    else if (app_info == null)
                    {
                        // capture created status
                        sw_app_capture.WriteLine("{0}\t1\t{1}", prev_cdr_id, app.AppID.ToString());
                    }


                    command.CommandText = "SELECT * FROM app_filesystem WHERE app_id = " + app.AppID + " AND cdr_id_last IS NULL";
                    data.Reset();
                    adapter.Fill(data);

                    var fsData = data.Tables[0].AsEnumerable();

                    foreach (AppFilesystem fs in app.Filesystems)
                    {
                        var row = fsData.Where(c => c["app_id_filesystem"].ToString() == fs.AppID.ToString() && c["mount_name"].ToString() == fs.MountName).ElementAtOrDefault(0);

                        // duplicate, ex railworks
                        if (row != null && row.RowState == DataRowState.Modified)
                            continue;

                        SQLQuery.BuildSubDataInsertFromType("app_filesystem", fs, row, app.AppID, cdr_id, prev_cdr_id, sw_app_filesystem);

                        if (row != null)
                            row.SetModified();
                    }

                    var toDelete = fsData.Where(c => c.RowState != DataRowState.Modified);

                    foreach (DataRow row in toDelete)
                    {
                        SQLQuery.CloseoutRow(typeof(AppFilesystem), row, prev_cdr_id, sw_app_filesystem);
                    }


                    command.CommandText = "SELECT * FROM app_version WHERE app_id = " + app.AppID + " AND cdr_id_last IS NULL";
                    data.Reset();
                    adapter.Fill(data);

                    var vData = data.Tables[0].AsEnumerable();

                    foreach (AppVersion appv in app.Versions)
                    {
                        var row = vData.Where(c => c["description"].ToString() == appv.Description && c["version_id"].ToString() == appv.VersionID.ToString()).ElementAtOrDefault(0);

                        // duplicate, ex 501. I don't think this is related to rollbacks
                        if (row != null && row.RowState == DataRowState.Modified)
                            continue;

                        SQLQuery.BuildSubDataInsertFromType("app_version", appv, row, app.AppID, cdr_id, prev_cdr_id, sw_app_version);

                        if (row != null)
                            row.SetModified();
                    }

                    toDelete = fsData.Where(c => c.RowState != DataRowState.Modified);

                    foreach (DataRow row in toDelete)
                    {
                        SQLQuery.CloseoutRow(typeof(AppVersion), row, prev_cdr_id, sw_app_version);
                    }


                }
            }


            // capture sub data

            using (StreamWriter sw_sub = new StreamWriter(new FileStream("sub.data", FileMode.Create)))
            using (StreamWriter sw_sub_capture = new StreamWriter(new FileStream("sub_capture.data", FileMode.Create)))
            using (StreamWriter sw_apps_subs = new StreamWriter(new FileStream("apps_subs.data", FileMode.Create)))
            {
                foreach (Sub sub in CDRBlob.Subs)
                {
                    command.CommandText = "SELECT * FROM sub WHERE sub_id = " + sub.SubID;
                    data.Reset();
                    adapter.Fill(data);

                    DataRow sub_info = null;

                    if (data.Tables[0].Rows.Count > 0)
                        sub_info = data.Tables[0].Rows[0];

                    string sub_current_data = null;
                    string sub_state_data = null;

                    SQLQuery.BuildDataInsertFromTypeWithChanges(sub, sub_info, cdr_id, prev_cdr_id, out sub_current_data, out sub_state_data);

                    sw_sub.WriteLine(sub_current_data);

                    if (sub_state_data != null)
                    {
                        sw_sub_capture.WriteLine(sub_state_data);
                    }
                    else if (sub_info == null)
                    {
                        // capture created status
                        sw_sub_capture.WriteLine("{0}\t1\t{1}", prev_cdr_id, sub.SubID.ToString());
                    }

                    foreach (int appid in sub.AppIDs)
                    {
                        sw_apps_subs.WriteLine(String.Format("{0}\t{1}\t{2}", appid, sub.SubID, cdr_id));
                    }
                }
            }

            string[] files = new string[] { "app.data", "app_capture.data", "sub.data", "sub_capture.data", "app_filesystem.data", "app_version.data" };
            string[] tables = new string[] { "app", "app_state_capture", "sub", "sub_state_capture", "app_filesystem", "app_version" };

            for (int i = 0; i < files.Length; i++)
            {
                command.CommandText = String.Format("LOAD DATA INFILE '{0}' REPLACE INTO TABLE {1} LINES TERMINATED BY \"\r\n\"", SQLQuery.EscapeValue(Path.Combine(Environment.CurrentDirectory, files[i]), false), tables[i]);

                command.ExecuteNonQuery();
            }

            files = new string[] { "apps_subs.data" };
            tables = new string[] { "apps_subs" };

            for (int i = 0; i < files.Length; i++)
            {
                command.CommandText = String.Format("LOAD DATA INFILE '{0}' IGNORE INTO TABLE {1} LINES TERMINATED BY \"\r\n\"", SQLQuery.EscapeValue(Path.Combine(Environment.CurrentDirectory, files[i]), false), tables[i]);

                command.ExecuteNonQuery();
            }

        }
    }
}
