using System.Collections.Generic;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using SnakeBite.GzsTool;
using System.Linq;

namespace SnakeBite
{
    internal static class ModManager
    {

        internal static string GameArchiveDir { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01";  } }
        internal static string GameArchivePath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat"; } }
        internal static string GameArchiveXmlPath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat.xml"; } }

        // Checks the saved InstallPath variable for the existence of MGSVTPP.exe
        internal static bool ValidInstallPath
        {
            get
            {
                string installPath = Properties.Settings.Default.InstallPath;
                if (Directory.Exists(installPath))
                {
                    if (File.Exists(String.Format("{0}\\MGSVTPP.exe", installPath)))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        internal static bool ExtractTempModData(string Filename)
        {
            if (File.Exists("_temp.zip"))
            {
                // delete temp.zip
                File.Delete("_temp.zip");
            }

            if (Directory.Exists("_temp"))
            {
                // delete temp directory
                Directory.Delete("_temp");
            }

            if (!File.Exists(Filename))
            {
                // specified file does not exist
                return false;
            }

            // copy file to app dir
            File.Copy(Filename, "_temp.zip");

            //extract

            FastZip unzipper = new FastZip();
            unzipper.ExtractZip("_temp.zip", "_temp", "*.*");

            //delete temp file
            File.Delete("_temp.zip");
            return true;
        }

        internal static void ExtractGameArchive()
        {
            // extract 01.dat archive using GzsTool
            GzsTool.GzsTool.Run(GameArchivePath);
        }

        internal static GameData BuildGameData()
        {
            if (Directory.Exists(GameArchiveDir)) Directory.Delete(GameArchiveDir, true);
            ExtractGameArchive();

            if (!Directory.Exists(GameArchiveDir)) return null;

            GameData buildData = new GameData();
            QarFile gameQarXml = new QarFile();
            gameQarXml.LoadFromFile(GameArchiveXmlPath);

            foreach(QarEntry gameQarEntry in gameQarXml.QarEntries)
            {
                buildData.GameQarEntries.Add(new ModQarEntry() { FilePath = gameQarEntry.FilePath, Compressed = gameQarEntry.Compressed, Hash = gameQarEntry.Hash });
            }

            buildData.GameFpkEntries = BuildFpkList(GameArchiveDir);

            return buildData;
        }

        internal static List<ModFpkEntry> BuildFpkList(string SearchDir)
        {
            // build list of all files in directory
            List<string> allFiles = Directory.GetFiles(SearchDir, "*.*", SearchOption.AllDirectories).ToList();
            List<string> fpkFiles = new List<string>();
            List<ModFpkEntry> BuildFpkList = new List<ModFpkEntry>();

            // find fpk/fpkd files
            foreach(string file in allFiles)
            {
                string fileExt = GetFileExtension(file);
                if(fileExt == "fpk" || fileExt == "fpkd")
                {
                    fpkFiles.Add(file);
                }
            }

            // unpack each archive and add to file list
            foreach(string fpkFile in fpkFiles)
            {
                GzsTool.GzsTool.Run(fpkFile); // unpack fpk
                FpkFile gzsFpkXml = new FpkFile();
                gzsFpkXml.LoadFromFile(fpkFile + ".xml");
                string fpkFileName = fpkFile.Substring(fpkFile.LastIndexOf("\\Assets\\")).Replace("\\","/"); // name of fpk for fpk list

                foreach (FpkEntry fpkFileEntry in gzsFpkXml.FpkEntries)
                {
                    BuildFpkList.Add(new ModFpkEntry() { FilePath = fpkFileEntry.FilePath, FpkFile = fpkFileName });
                }

                File.Delete(fpkFile + ".xml");
                Directory.Delete(fpkFile.Replace(".", "_"), true);
            }

            //   remove existing directory
            //   unpack fpk
            //   generate data for files
            //   remove temp data
            // return data
            return BuildFpkList;
        }

        internal static string GetFileExtension(string FilePath)
        {
            return FilePath.Substring(FilePath.LastIndexOf(".") + 1).ToLower();
        }

        internal static string HashFile(string Filename)
        {
            byte[] hashBytes;
            using (var hashMD5 = MD5.Create())
            {
                using (var stream = File.OpenRead(Filename))
                {
                    hashBytes = hashMD5.ComputeHash(stream);
                }
            }

            StringBuilder hashBuilder = new StringBuilder(hashBytes.Length * 2);

            for (int i = 0; i < hashBytes.Length; i++)
                hashBuilder.Append(hashBytes[i].ToString("X2"));

            return hashBuilder.ToString();
        }
    }
}