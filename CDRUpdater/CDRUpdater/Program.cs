using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SteamKit2;

namespace CDRUpdater
{
    class Program
    {
        const string HASHFILE = "CDR.hash";
        const string CDRBLOB = "CDR.blob";

        static void Main(string[] args)
        {
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
                    //return;
                }
                else
                {
                    File.WriteAllBytes(CDRBLOB, cdr);

                    byte[] current_hash = CryptoHelper.SHAHash(cdr);

                    try
                    {
                        File.WriteAllBytes(HASHFILE, current_hash);
                    }
                    catch (Exception ex2)
                    {
                        DebugLog.Write("Warning: Unable to write to hashfile: {0}\n", ex2.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.Write("Unable to download CDR: {0}\n", ex.ToString());
                return;
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


        }
    }
}
