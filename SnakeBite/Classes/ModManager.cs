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

        internal static List<ModEntry> GetInstalledMods()
        {
            Settings settingsXml = new Settings();
            settingsXml.LoadSettings();

            return settingsXml.ModEntries;
        }
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

        // gets information about the existing 01.dat archive
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

            Directory.Delete(GameArchiveDir, true);
            File.Delete(GameArchiveXmlPath);

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
                string fpkFileName = fpkFile.Substring(GameArchiveDir.Length).Replace("\\","/"); // name of fpk for fpk list

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

        internal static string GetAssetDir(string Path)
        {
            string bsPath = DoBackSlashes(Path);
            return bsPath.Substring(bsPath.LastIndexOf("/Assets"));
        }

        internal static string DoBackSlashes(string str)
        {
            string DoBackSlashes = str.Replace("\\", "/");
            return DoBackSlashes;
        }

        internal static string DoForwardSlashes(string str)
        {
            string DoForwardSlashes = str.Replace("/", "\\");
            return DoForwardSlashes;
        }

        internal static string GetFileExtension(string FilePath)
        {
            return FilePath.Substring(FilePath.LastIndexOf(".") + 1).ToLower();
        }

        // validates 01.dat MD5 against previous hash
        internal static bool CheckDatHash()
        {
            string md5old = Properties.Settings.Default.DatHash;
            if(HashFile(GameArchivePath) != md5old) return false;
            return true;
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

        public static bool InstallMod(string ModFile)
        {
            if (!ModManager.ValidInstallPath) return false; // no valid install specified

            if (!File.Exists(GameArchivePath)) return false; 

            // extract existing DAT file
            GzsTool.GzsTool.Run(GameArchivePath);

            // import existing DAT xml
            QarFile qarXml = new QarFile();
            qarXml.LoadFromFile(GameArchiveXmlPath);

            // extract mod files to temp folder
            if (Directory.Exists("_temp")) Directory.Delete("_temp", true);
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(ModFile, "_temp", "(.*?)");

            // load mod metadata
            ModEntry modMetadata = new ModEntry();
            modMetadata.ReadFromFile("_temp\\metadata.xml");
            File.Delete("_temp\\metadata.xml");

            // check for fpk merges
            List<ModFpkEntry> installedFpkFiles = new List<ModFpkEntry>();
            foreach (QarEntry gzsQar in qarXml.QarEntries)
            {
                string qarExt = gzsQar.FilePath.Substring(gzsQar.FilePath.LastIndexOf(".") + 1).ToLower();
                if (qarExt == "fpk" || qarExt == "fpkd")
                {
                    // extract FPK and add files to list
                    string fpkFile = GameArchiveDir + "\\" + gzsQar.FilePath;
                    string fpkDir = fpkFile.Replace(".", "_");
                    GzsTool.GzsTool.Run(fpkFile);

                    List<string> fpkFiles = Directory.GetFiles(fpkDir, "*.*", SearchOption.AllDirectories).ToList();
                    foreach (string file in fpkFiles)
                    {
                        string fpkFilePath = fpkFile.Substring(GameArchiveDir.Length);
                        fpkFilePath =  fpkFilePath.Replace("\\", "/");

                        string fpkPath = file.Substring(file.LastIndexOf("Assets\\"));
                        fpkPath = "/" + fpkPath.Replace("\\", "/");

                        installedFpkFiles.Add(new ModFpkEntry() { FilePath = fpkFilePath, FpkFile = fpkPath });
                    }
                }
            }

            // compare lists and build merge fpk list
            List<string> mergeFpks = new List<string>();
            foreach (ModFpkEntry installedFpk in installedFpkFiles)
            {
                foreach (ModEntry mod in GetInstalledMods())
                {
                    if (mod.ModFpkEntries.Find(entry => entry.FpkFile == installedFpk.FpkFile) != null) // if the mod has an fpk that should be merged with an installed fpk
                    {
                        if (!mergeFpks.Contains(installedFpk.FpkFile))
                        {
                            mergeFpks.Add(installedFpk.FpkFile);
                        }
                    }
                }
            }

            if (mergeFpks.Count > 0)
            {
                // merge fpks
                foreach (string fpkFile in mergeFpks)
                {

                    string fpkPath = DoForwardSlashes(fpkFile);
                    string gameFpkPath = GameArchiveDir + fpkPath;
                    string gameFpkDir = GameArchiveDir + "\\master\\0\\01\\" + fpkPath.Replace(".", "_");
                    string modFpkPath = "_temp\\" + fpkPath;
                    string modFpkDir = "_temp\\" + fpkPath.Replace(".", "_");

                    GzsTool.GzsTool.Run(gameFpkPath);
                    GzsTool.GzsTool.Run(modFpkPath);

                    // load existing xml data
                    FpkFile fpkXml = new FpkFile();
                    fpkXml.LoadFromFile(gameFpkPath + ".xml");

                    // generate list of files to move and add to xml
                    List<string> filesToMove = new List<string>();
                    foreach (ModFpkEntry file in modMetadata.ModFpkEntries.FindAll(entry => entry.FpkFile == fpkFile))
                    {
                        filesToMove.Add(file.FilePath.Replace("/", "\\"));
                        if (fpkXml.FpkEntries.Count(entry => entry.FilePath == file.FilePath) == 0)
                        {
                            // insert new fpk entries as required
                            fpkXml.FpkEntries.Add(new FpkEntry() { FilePath = fpkFile });
                        }
                    }

                    // create directories and move files
                    foreach (string file in filesToMove)
                    {
                        string fileDir = (gameFpkDir + file).Substring(0, (gameFpkDir + file).LastIndexOf("\\"));
                        if (!Directory.Exists(fileDir))
                        {
                            Directory.CreateDirectory(fileDir);
                        }
                        File.Copy(modFpkDir + file, gameFpkDir + file, true);
                    }

                    fpkXml.WriteToFile(gameFpkPath + ".xml");
                    GzsTool.GzsTool.Run(gameFpkPath + ".xml");
                }
            }

            // copy files for new DAT
            foreach (ModQarEntry modQarFile in modMetadata.ModQarEntries)
            {
                string fileName = "/" + modQarFile.FilePath.Replace("\\", "/");
                string fileDir = (GameArchiveDir + modQarFile.FilePath.Replace("/","\\")).Substring(0, (GameArchiveDir  + modQarFile.FilePath).LastIndexOf("/"));

                // if file is not already in QAR, add it
                if (qarXml.QarEntries.Count(entry => entry.FilePath == modQarFile.FilePath) == 0)
                {
                        qarXml.QarEntries.Add(new QarEntry() { FilePath = modQarFile.FilePath, Compressed = modQarFile.Compressed, Hash=modQarFile.Hash });  
                }

                // copy all files that weren't merged FPKS
                if (!mergeFpks.Contains(fileName))
                {
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }
                    File.Copy("_temp\\" + modQarFile.FilePath.Replace("/","\\"), ModManager.GameArchiveDir + modQarFile.FilePath.Replace("/", "\\"), true);

                }

            }

            // build XML for new DAT
            qarXml.WriteToFile(GameArchiveXmlPath);

            // build new DAT
            GzsTool.GzsTool.Run(GameArchiveXmlPath);

            // remove temp files
            Directory.Delete("_temp", true);
            Directory.Delete(GameArchiveDir, true);
            File.Delete(GameArchiveXmlPath);

            return true;
        }

        public static bool UninstallMod(ModEntry mod)
        {
            // extract 01.dat
            if (!ModManager.ValidInstallPath) return false; // no valid install specified

            if (!File.Exists(GameArchivePath)) return false;

            GzsTool.GzsTool.Run(GameArchivePath);


            // load xml data
            QarFile datXml = new QarFile();
            datXml.LoadFromFile(GameArchiveXmlPath);

            // check for fpks
            List<string> modFpks = new List<string>();
            foreach(ModFpkEntry modFpkFile in mod.ModFpkEntries)
            {
                if(datXml.QarEntries.Count(entry => entry.FilePath == modFpkFile.FpkFile) > 0 && !modFpks.Contains(modFpkFile.FpkFile))
                {
                    modFpks.Add(modFpkFile.FpkFile);
                }
            }

            // do the fpks thing
            // unpack fpk
            foreach(string fpkFile in modFpks)
            {
                // check if fpk file exists in game data
                if(File.Exists(GameArchiveDir + fpkFile.Replace("/","\\")))
                {
                    // extract fpk
                    GzsTool.GzsTool.Run(GameArchiveDir + fpkFile.Replace("/", "\\"));

                    string fpkDir = GameArchiveDir + fpkFile.Replace(".", "_").Replace("/", "\\");
                    FpkFile fpkXml = new FpkFile();
                    fpkXml.LoadFromFile(GameArchiveDir + fpkFile + ".xml");

                    // check if any files left in fpk
                    List<FpkEntry> fpkFiles = fpkXml.FpkEntries.ToList();
                    foreach(FpkEntry fpkSubFile in fpkXml.FpkEntries)
                    {
                        fpkFiles.RemoveAll(entry => entry.FilePath == fpkSubFile.FilePath);
                    }

                    // if not, remove it
                    if(fpkFiles.Count == 0)
                    {
                        // delete fpk from dat XML
                        datXml.QarEntries.RemoveAll(entry => entry.FilePath == fpkFile);
                    }
                }
            }

            // remove mod files from xml
            foreach(ModQarEntry modQEntry in mod.ModQarEntries)
            {
                // delete files, fpks that were de-merged will be ignored
                if(!modFpks.Contains(modQEntry.FilePath))
                {
                    datXml.QarEntries.RemoveAll(entry => entry.FilePath == modQEntry.FilePath);
                }
            }

            // rebuild 01.dat
            datXml.WriteToFile(GameArchiveXmlPath);

            GzsTool.GzsTool.Run(GameArchiveXmlPath);

            return true;
        }

    }



}