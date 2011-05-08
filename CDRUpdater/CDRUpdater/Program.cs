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
            /*byte[] cdr = null;
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
                    //return;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                DebugLog.Write("Unable to download CDR: {0}\n", ex.ToString());
                return;
            }*/

            byte[] cdr = File.ReadAllBytes(CDRBLOB);

                    byte[] current_hash = CryptoHelper.SHAHash(cdr);

                    string hash_hex = current_hash.Aggregate(new StringBuilder(), 
                               (sb,v)=>sb.Append(v.ToString("x2"))
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

            // the current CDR is always the most recently processed, incase we delete CDRs
            command.CommandText = "SELECT id FROM cdr ORDER BY date_processed DESC LIMIT 1";

            int prev_cdr_id = Convert.ToInt32(command.ExecuteScalar());

            command.CommandText = String.Format("INSERT INTO cdr (hash, version, date_updated, date_processed) VALUES ('{0}', {1}, '{2}', '{3}')",
                                                    hash_hex, CDRBlob.VersionNum, String.Format("{0:u}", CDRBlob.LastChangedExistingAppOrSubscriptionTime.ToDateTime()),
                                                    String.Format("{0:u}", DateTime.Now));

            command.ExecuteNonQuery();

            command.CommandText = "SELECT last_insert_id()";

            int cdr_id = Convert.ToInt32(command.ExecuteScalar());


            // capture app data and add app_* tables
            DataSet data = new DataSet();
            command = connection.CreateCommand();
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);


            using (StreamWriter sw_app = new StreamWriter(new FileStream("app.data", FileMode.Create)))
            using (StreamWriter sw_app_capture = new StreamWriter(new FileStream("app_capture.data", FileMode.Create)))
            {
                foreach (App app in CDRBlob.Apps)
                {
                    command.CommandText = "SELECT * FROM app WHERE app_id = " + app.AppID;
                    data.Clear();
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

                    foreach (AppVersion appv in app.Versions)
                    {
                        //sw.WriteLine(SQLQuery.BuildInsertQueryFromType("app_version", appv));
                    }

                }
            }


            // capture sub data
            data = new DataSet();

            using (StreamWriter sw_sub = new StreamWriter(new FileStream("sub.data", FileMode.Create)))
            using (StreamWriter sw_sub_capture = new StreamWriter(new FileStream("sub_capture.data", FileMode.Create)))
            using (StreamWriter sw_apps_subs = new StreamWriter(new FileStream("apps_subs.data", FileMode.Create)))
            {
                foreach (Sub sub in CDRBlob.Subs)
                {
                    command.CommandText = "SELECT * FROM sub WHERE sub_id = " + sub.SubID;
                    data.Clear();
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

                    foreach (int appid in sub.AppIDs)
                    {
                        sw_apps_subs.WriteLine(String.Format("{0}\t{1}\t{2}", appid, sub.SubID, cdr_id));
                    }
                }
            }

        }
    }
}
