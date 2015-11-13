using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace sbupdater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Require argument to be passed from SnakeBite
            if (args.Length == 0) return;

            // Remove existing update files
            if (File.Exists("update.dat")) File.Delete("update.dat");
            if (Directory.Exists("_update")) Directory.Delete("_update", true);
            if (File.Exists("_updater.exe")) File.Delete("_updater.exe");

            switch (args[0])
            {
                case "-g":
                    GenerateUpdateXml();
                    break;

                case "-u":
                    ProcessUpdate();
                    break;
            }
        }

        private static void GenerateUpdateXml()
        {
            UpdateFile update = new UpdateFile();

            update.SnakeBite.OldVersion = 800;
            update.SnakeBite.Version.Version = "0.8.0.0";
            update.SnakeBite.URL = "http://www.xobanimot.com/snakebite/update/update.zip";

            update.Updater.OldVersion = 2;
            update.Updater.Version.Version = "0.0.0.2";
            update.Updater.URL = "http://www.xobanimot.com/snakebite/update/updater.zip";

            update.WriteXml("test.xml");
        }

        private static void ProcessUpdate()
        {
            Console.WriteLine("Fetching update info...");
            UpdateFile update = new UpdateFile();
            update.ReadXmlFromInterweb("http://www.xobanimot.com/snakebite/update/update.xml");

            var SBVersion = GetSBVersion();
            var UpdaterVersion = GetUpdaterVersion();

            if (update.Updater.Version.AsVersion() > UpdaterVersion)
            {
                Console.WriteLine(String.Format("Updating SBUpdater to version {0}...", update.Updater.Version.AsString()));
                // Process updating the updater
                DownloadAndUpdateUpdater(update.Updater.URL);
            }
            else
            {
                Console.WriteLine("SBUpdater is up to date.");
            }

            if (update.SnakeBite.Version.AsVersion() > SBVersion)
            {
                Console.WriteLine(String.Format("Updating SnakeBite to version {0}...", update.SnakeBite.Version.AsString()));
                // Process updating SnakeBite
                DownloadAndUpdateSnakeBite(update.SnakeBite.URL);
            }
            else
            {
                Console.WriteLine("SnakeBite is up to date.");
            }

            Process.Start("SnakeBite.exe");
        }

        private static void DownloadAndUpdateUpdater(string URL)
        {
            // Download update archive
            using (WebClient w = new WebClient()) w.DownloadFile(URL, "update.dat");

            // Move updater executable
            File.Move("sbupdater.exe", "_updater.exe");

            // Extract archive to local directory
            FastZip z = new FastZip();
            z.ExtractZip("update.dat", ".", "(.*?)");

            // Restart updater
            Process updater = new Process();
            updater.StartInfo.Arguments = "-u";
            updater.StartInfo.UseShellExecute = false;
            updater.StartInfo.FileName = "sbupdater.exe";
            updater.Start();

            Environment.Exit(0);
        }

        private static void DownloadAndUpdateSnakeBite(string URL)
        {
            // Download update archive
            using (WebClient w = new WebClient()) w.DownloadFile(URL, "update.dat");

            // Extract archive
            FastZip z = new FastZip();
            z.ExtractZip("update.dat", "_update", "(.*?)");

            // Move update file
            File.Delete("SnakeBite.exe");
            File.Delete("MakeBite.exe");
            File.Delete("GzsTool.Core.dll");
            File.Delete("fpk_dictionary.txt");
            File.Delete("qar_dictionary.txt");
            File.Move("_update/SnakeBite.exe", "SnakeBite.exe");
            File.Move("_update/MakeBite.exe", "MakeBite.exe");
            File.Move("_update/GzsTool.Core.dll", "GzsTool.Core.dll");
            File.Move("_update/fpk_dictionary.txt", "fpk_dictionary.txt");
            File.Move("_update/qar_dictionary.txt", "qar_dictionary.txt");

            // Restart updater
            Process updater = new Process();
            updater.StartInfo.Arguments = "-u";
            updater.StartInfo.UseShellExecute = false;
            updater.StartInfo.FileName = "sbupdater.exe";
            updater.Start();

            Environment.Exit(0);
        }

        private static void GetUpdateContents()
        {
        }

        private static Version GetSBVersion()
        {
            // get SB version or return 0
            try
            {
                var versionInfo = FileVersionInfo.GetVersionInfo("SnakeBite.exe");
                return new Version(versionInfo.ProductVersion);
            }
            catch
            {
                return null;
            }
        }

        private static Version GetUpdaterVersion()
        {
            // Get updater version
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}