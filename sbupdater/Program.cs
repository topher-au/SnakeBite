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

            update.SnakeBite.Version = 620;
            update.SnakeBite.URL = "http://www.xobanimot.com/snakebite/update/update.zip";

            update.Updater.Version = 1;
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

            if (update.Updater.Version > UpdaterVersion)
            {
                Console.WriteLine(String.Format("Updating SBUpdater to version {0}...", update.Updater.Version));
                // Process updating the updater
                DownloadAndUpdateUpdater(update.Updater.URL);
            }
            else
            {
                Console.WriteLine("SBUpdater is up to date.");
            }

            if (update.SnakeBite.Version > SBVersion)
            {
                Console.WriteLine(String.Format("Updating SnakeBite to version {0}...", update.SnakeBite.Version));
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

            // Extract archive
            FastZip z = new FastZip();
            z.ExtractZip("update.dat", "_update", "(.*?)");

            // Move update file
            File.Move("sbupdater.exe", "_updater.exe");
            File.Move("_update/sbupdater.exe", "sbupdater.exe");

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

        private static int GetSBVersion()
        {
            // get SB version or return 0
            try
            {
                var versionInfo = FileVersionInfo.GetVersionInfo("SnakeBite.exe");
                string version = versionInfo.ProductVersion;
                return Convert.ToInt32(version.Replace(".", ""));
            }
            catch
            {
                return 0;
            }
        }

        private static int GetUpdaterVersion()
        {
            // Get updater version
            string assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return Convert.ToInt32(assemblyVersion.Replace(".", ""));
        }
    }
}