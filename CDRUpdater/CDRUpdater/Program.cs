using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SteamKit2;
using SteamKit2.Blob;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

namespace CDRUpdater
{
    class Program
    {
        const string HASHFILE = "CDR.hash";
        const string CDRBLOB = "CDR.blob";
        const string SQL_CONNECTION_STRING = "connection_string.cfg";

        const int CHUNK_PROCESS = 100;

        static void Main(string[] args)
        {
            string connection_string = File.ReadAllText(SQL_CONNECTION_STRING);

            DebugLog.Write("CDR updater running!\n");

            byte[] cdr = null;

            if (args.Length == 1 && File.Exists(args[0]))
            {
                cdr = File.ReadAllBytes(args[0]);
                File.WriteAllBytes(CDRBLOB, cdr);
                DebugLog.Write("Using historical blob {0}\n", args[0]);
            }
            else
            {
                DebugLog.Write("Downloading CDR...\n");

                try
                {
                    byte[] hash = null;
                    try
                    {
                        if (File.Exists(HASHFILE))
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
                    File.WriteAllBytes(String.Format("CDR.blob.{0}", DateTime.Now.ToFileTime()), cdr);

                }
                catch (Exception ex)
                {
                    DebugLog.Write("Unable to download CDR: {0}\n", ex.ToString());
                    return;
                }
            }

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

            ProcessCDR(connection_string, hash_hex);

            DebugLog.Write("Finished.\n");
        }

        static void ProcessCDR(string connection_string, string hash_hex)
        {
            CDR CDRBlob = null;

            using (BlobReader reader = BlobReader.CreateFrom(CDRBLOB))
            {
                CDRBlob = (CDR)BlobTypedReader.Deserialize(reader, typeof(CDR));
            }

            if (CDRBlob == null)
            {
                DebugLog.Write("Unable to parse CDR.");
                return;
            }

            DebugLog.Write("Building updates..\n");

            SQLConnection connection = new SQLConnection(connection_string);

            // the current CDR is always the most recently processed
            int prev_cdr_id = Convert.ToInt32(connection.ExecuteScalar("SELECT cdr_id FROM cdr ORDER BY date_processed DESC LIMIT 1"));

            DateTime now = DateTime.UtcNow;
            DateTime updated = CDRBlob.LastChangedExistingAppOrSubscriptionTime.ToDateTimeUTC();

            connection.Execute(String.Format("INSERT INTO cdr (hash, version, date_updated, date_processed, app_count, sub_count) VALUES ('{0}', {1}, '{2}', '{3}', {4}, {5})",
                                                    hash_hex, CDRBlob.VersionNum, updated.ToString("yyyy-MM-dd HH:mm"),
                                                    now.ToString("yyyy-MM-dd HH:mm"),
                                                    CDRBlob.Apps.Count,
                                                    CDRBlob.Subs.Count));

            int cdr_id = Convert.ToInt32(connection.ExecuteScalar("SELECT last_insert_id()"));

            DebugLog.Write("Building sub updates...\n");

            Dictionary<uint, bool> idCount = new Dictionary<uint, bool>(); // dictionary of subs or apps to check against repeats 
            Dictionary<uint, int> appSubCounts = new Dictionary<uint, int>();

            // capture sub data
            using (StreamWriter sw_sub = new StreamWriter(new FileStream("sub.data", FileMode.Create)))
            using (StreamWriter sw_sub_capture = new StreamWriter(new FileStream("sub_capture.data", FileMode.Create)))
            using (StreamWriter sw_apps_subs = new StreamWriter(new FileStream("apps_subs.data", FileMode.Create)))
            {
                for (int i = 0; i < CDRBlob.Subs.Count; i += CHUNK_PROCESS)
                {
                    List<string> ids = new List<string>();

                    SQLTable<Sub> subTable = new SQLTable<Sub>(new string[] { "sub_id" });
                    SQLTable<int> appSubsTable = new SQLTable<int>(new string[] { "sub_id", "app_id" });

                    for (int x = i; x < i + CHUNK_PROCESS && x < CDRBlob.Subs.Count; x++)
                    {
                        string id = CDRBlob.Subs[x].SubID.ToString();

                        if (idCount.ContainsKey(CDRBlob.Subs[x].SubID))
                        {
                            Console.WriteLine("Warning: Duplicate sub \"{0}\" ({1}), using the first definition.", CDRBlob.Subs[x].Name, CDRBlob.Subs[x].SubID);
                            continue;
                        }
                        idCount[CDRBlob.Subs[x].SubID] = true;

                        ids.Add(id);

                        subTable.Attach(CDRBlob.Subs[x], new object[] { id }, CDRBlob.Subs[x].SubID);

                        CDRBlob.Subs[x].virtual_app_count = CDRBlob.Subs[x].AppIDs.Count;

                        foreach (uint appid in CDRBlob.Subs[x].AppIDs)
                        {
                            appSubsTable.Attach((int)appid, new object[] { id, appid }, CDRBlob.Subs[x].SubID);

                            appSubCounts[appid] = (appSubCounts.ContainsKey(appid) ? appSubCounts[appid] + 1 : 1);
                        }
                    }

                    var reader = connection.ExecuteReader("SELECT * FROM sub WHERE sub_id IN (" + String.Join(",", ids.ToArray()) + ")");

                    subTable.Process(reader, (row, p_reader, subid) =>
                    {
                        string sub_current_data = null;
                        string sub_state_data = null;

                        SQLQuery.BuildDataInsertFromTypeWithChanges(row, p_reader, cdr_id, /*prev_cdr_id,*/ out sub_current_data, out sub_state_data);

                        string update;
                        if (sub_state_data == null && p_reader != null)
                            update = String.Format("{0:u}", p_reader["date_updated"]);
                        else
                            update = String.Format("{0:u}", updated);

                        sw_sub.Write(sub_current_data + "\t" + update + "\r\n");

                        if (sub_state_data != null)
                        {
                            sw_sub_capture.Write(sub_state_data + "\r\n");
                        }
                        else if (p_reader == null)
                        {
                            sw_sub_capture.Write("{0}\t1\t{1}\r\n", /*prev_cdr_id,*/ cdr_id, subid.ToString());
                        }
                    },
                    (p_reader) =>
                    {
                        DebugLog.Write("Warning, sub id {0} is missing from CDR but left in DB\n", p_reader["sub_id"]);

                        Debug.Fail("Unable to continue, missing subID");
                    });

                    reader.Close();

                    reader = connection.ExecuteReader("SELECT * FROM apps_subs WHERE sub_id IN (" + String.Join(",", ids.ToArray()) + ") AND cdr_id_last IS NULL");

                    appSubsTable.Process(reader, (row, p_reader, subid) =>
                    {
                        if (p_reader == null)
                        {
                            sw_apps_subs.Write(String.Format("{0}\t{1}\t{2}\t\\N\r\n", row, subid, cdr_id));
                        }
                    },
                    (p_reader) =>
                    {
                        sw_apps_subs.Write(String.Format("{0}\t{1}\t{2}\t{3}\r\n", p_reader["app_id"], p_reader["sub_id"], p_reader["cdr_id"], prev_cdr_id));
                    });

                    reader.Close();
                }
            }


            DebugLog.Write("Built sub updates...\n");
            idCount.Clear();
            DebugLog.Write("Building app updates...\n");


            // capture app data and add app_* tables
            using (StreamWriter sw_app = new StreamWriter(new FileStream("app.data", FileMode.Create)))
            using (StreamWriter sw_app_capture = new StreamWriter(new FileStream("app_capture.data", FileMode.Create)))
            using (StreamWriter sw_app_filesystem = new StreamWriter(new FileStream("app_filesystem.data", FileMode.Create)))
            using (StreamWriter sw_app_version = new StreamWriter(new FileStream("app_version.data", FileMode.Create)))
            {
                for (int i = 0; i < CDRBlob.Apps.Count; i += CHUNK_PROCESS)
                {
                    List<string> ids = new List<string>();

                    SQLTable<App> appTable = new SQLTable<App>(new string[] { "app_id" });
                    SQLTable<AppFilesystem> fsTable = new SQLTable<AppFilesystem>(new string[] { "app_id", "app_id_filesystem", "mount_name" });
                    SQLTable<AppVersion> versionTable = new SQLTable<AppVersion>(new string[] { "app_id", "description", "version_id" });

                    for (int x = i; x < i + CHUNK_PROCESS && x < CDRBlob.Apps.Count; x++)
                    {
                        if (idCount.ContainsKey(CDRBlob.Apps[x].AppID))
                        {
                            Console.WriteLine("Warning: Duplicate app \"{0}\" ({1}), using the first definition.", CDRBlob.Apps[x].Name, CDRBlob.Apps[x].AppID);
                            continue;
                        }
                        idCount[CDRBlob.Apps[x].AppID] = true;

                        string id = CDRBlob.Apps[x].AppID.ToString();
                        ids.Add(id);

                        appTable.Attach(CDRBlob.Apps[x], new object[] { id }, CDRBlob.Apps[x].AppID);

                        CDRBlob.Apps[x].virtual_sub_count = (appSubCounts.ContainsKey(CDRBlob.Apps[x].AppID) ? appSubCounts[CDRBlob.Apps[x].AppID] : 0);

                        foreach (AppFilesystem fs in CDRBlob.Apps[x].Filesystems)
                        {
                            fsTable.Attach(fs, new object[] { id, fs.AppID, fs.MountName }, CDRBlob.Apps[x].AppID);
                        }

                        foreach (AppVersion appv in CDRBlob.Apps[x].Versions)
                        {
                            versionTable.Attach(appv, new object[] { id, appv.Description, appv.VersionID }, CDRBlob.Apps[x].AppID);
                        }
                    }

                    var reader = connection.ExecuteReader("SELECT * FROM app WHERE app_id IN (" + String.Join(",", ids.ToArray()) + ")");

                    appTable.Process(reader, (row, p_reader, appid) =>
                    {
                        string app_current_data = null;
                        string app_state_data = null;

                        SQLQuery.BuildDataInsertFromTypeWithChanges(row, p_reader, cdr_id, /*prev_cdr_id,*/ out app_current_data, out app_state_data);

                        string update;
                        if (app_state_data == null && p_reader != null)
                            update = String.Format("{0:u}", p_reader["date_updated"]);
                        else
                            update = String.Format("{0:u}", updated);

                        sw_app.Write(app_current_data + "\t" + update + "\r\n");

                        if (app_state_data != null)
                        {
                            sw_app_capture.Write(app_state_data + "\r\n");
                        }
                        else if (p_reader == null)
                        {
                            sw_app_capture.Write("{0}\t1\t{1}\r\n", /*prev_cdr_id,*/ cdr_id, appid.ToString());
                        }
                    },
                    (p_reader) =>
                    {
                        DebugLog.Write("Warning, app id {0} is missing from CDR but left in DB\n", p_reader["app_id"]);

                        Debug.Fail("Unable to continue, missing appID");
                    });

                    reader.Close();


                    reader = connection.ExecuteReader("SELECT * FROM app_filesystem WHERE app_id IN (" + String.Join(",", ids.ToArray()) + ") AND cdr_id_last IS NULL");

                    fsTable.Process(reader, (row, p_reader, appid) =>
                    {
                        SQLQuery.BuildSubDataInsertFromType("app_filesystem", row, p_reader, appid, cdr_id, prev_cdr_id, sw_app_filesystem);
                    },
                    (p_reader) =>
                    {
                        SQLQuery.CloseoutRow(typeof(AppFilesystem), p_reader, prev_cdr_id, sw_app_filesystem);
                    });

                    reader.Close();


                    reader = connection.ExecuteReader("SELECT * FROM app_version WHERE app_id IN (" + String.Join(",", ids.ToArray()) + ") AND cdr_id_last IS NULL");

                    versionTable.Process(reader, (row, p_reader, appid) =>
                    {
                        SQLQuery.BuildSubDataInsertFromType("app_version", row, p_reader, appid, cdr_id, prev_cdr_id, sw_app_version);
                    },
                    (p_reader) =>
                    {
                        SQLQuery.CloseoutRow(typeof(AppVersion), p_reader, prev_cdr_id, sw_app_version);
                    });

                    reader.Close();
                }
            }

            DebugLog.Write("Built app updates...\n");
            DebugLog.Write("Updating database...\n");

            string[] files = new string[] { "app.data", "app_capture.data", "sub.data", "sub_capture.data", "app_filesystem.data", "app_version.data", "apps_subs.data" };
            string[] tables = new string[] { "app", "app_state_capture", "sub", "sub_state_capture", "app_filesystem", "app_version", "apps_subs" };

            for (int i = 0; i < files.Length; i++)
            {
                connection.Execute(String.Format("LOAD DATA INFILE '{0}' REPLACE INTO TABLE {1} LINES TERMINATED BY \"\r\n\"", SQLQuery.EscapeValue(Path.Combine(Environment.CurrentDirectory, files[i]), false), tables[i]));
            }
        }
    }
}
