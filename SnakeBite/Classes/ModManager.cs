using GzsTool;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GzsTool.Core.Utility;
using GzsTool.Core.Qar;
using GzsTool.Core.Fpk;
using SnakeBite.GzsTool;
using SnakeBite;

namespace SnakeBite
{
    internal static class ModManager
    {
        internal static string GameDir { get { return Properties.Settings.Default.InstallPath; } }
        internal static string ExtractedDatDir { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01_dat"; } }
        internal static string DatPath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat"; } }
        internal static string DatXmlPath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat.xml"; } }
        //private static BackupManager backupManager = new BackupManager();

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

        internal static int GetSBVersion()
        {
            // Get SB app version
            string assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return Convert.ToInt32(assemblyVersion.Replace(".", ""));
        }

        internal static int GetMGSVersion()
        {
            // Get MGSV executable version
            var versionInfo = FileVersionInfo.GetVersionInfo(Properties.Settings.Default.InstallPath + "\\mgsvtpp.exe");
            string version = versionInfo.ProductVersion;
            return Convert.ToInt32(version.Replace(".", ""));
        }
        private static void CleanupDirectory()
        {
            if (Directory.Exists("_working")) Directory.Delete("_working", true);
            if (Directory.Exists("_extr")) Directory.Delete("_extr", true);
            if (Directory.Exists("_gamefpk")) Directory.Delete("_gamefpk", true);
            if (Directory.Exists("_modfpk")) Directory.Delete("_modfpk", true);
        }

        public static bool InstallMod2(string ModFile)
        {
            List<ModEntry> installedMods = GetInstalledMods();

            CleanupDirectory();

            // Extract game archive
            var datFiles = GzsLib.ExtractArchive<QarFile>(DatPath, "_working");

            // Extract mod data
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(ModFile, "_extr", "(.*?)");

            // Load mod metadata
            ModEntry metaData = new ModEntry("_extr\\metadata.xml");

            // Build a list of FPKs contained in mod
            List<string> modFpks = new List<string>();
            foreach(ModFpkEntry fpkEntry in metaData.ModFpkEntries)
            {
                if (!modFpks.Contains(fpkEntry.FpkFile)) modFpks.Add(fpkEntry.FpkFile);
            }

            List<string> mergeFpks = new List<string>();

            // Check for FPKs in 01.dat
            foreach(string fpk in modFpks)
            {
                string datFile = datFiles.FirstOrDefault(file => Tools.NameToHash(file) == Tools.NameToHash(fpk));
                if(datFile != null && !mergeFpks.Contains(fpk))
                {
                    mergeFpks.Add(fpk);
                }
            }

            var gameData = GzsLib.ReadBaseData();

            var zeroFiles = gameData.FileList.FindAll(entry => entry.QarFile == "0\\00.dat");
            var baseFiles = gameData.FileList.FindAll(entry => entry.QarFile != "0\\00.dat");

            // Check for FPKs in 00.dat
            foreach(string fpk in modFpks)
            {
                GameFile file = zeroFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                if(file != null && !mergeFpks.Contains(fpk))
                {
                    // Create destination directory
                    string destDirectory = Path.Combine("_working", Path.GetDirectoryName(Tools.ToWinPath(file.FilePath)));
                    if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);
                    // Extract file into dat directory
                    GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\0\\00.dat"), file.FileHash, Path.Combine("_working", Tools.ToWinPath(fpk)));
                    mergeFpks.Add(fpk);
                }
            }

            // Check for FPKs in base data
            foreach (string fpk in modFpks)
            {
                GameFile file = baseFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                if (file != null && !mergeFpks.Contains(fpk))
                {
                    // Create destination directory
                    string destDirectory = Path.Combine("_working", Path.GetDirectoryName(Tools.ToWinPath(file.FilePath)));
                    if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);
                    // Extract file into dat directory
                    GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\" + file.QarFile), file.FileHash, Path.Combine("_working", Tools.ToWinPath(fpk)));
                    mergeFpks.Add(fpk);
                }
            }

            // Merge FPK files
            foreach(string fpk in mergeFpks)
            {
                // Extract game FPK
                string fpkPath = Path.Combine("_working", Tools.ToWinPath(fpk));
                var gameFpk = GzsLib.ExtractArchive<FpkFile>(fpkPath, "_gamefpk");

                // Extract mod FPK
                GzsLib.ExtractArchive<FpkFile>(Path.Combine("_extr", Tools.ToWinPath(fpk)), "_modfpk");

                // Merge contents
                foreach (string fileName in Directory.GetFiles("_modfpk", "*.*", SearchOption.AllDirectories))
                {
                    string fileFpkName = fileName.Substring(8);
                    string fileDir = (Path.Combine("_gamefpk", Path.GetDirectoryName(fileName).Substring(8)));

                    if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
                    File.Copy(fileName, Path.Combine(fileDir, Path.GetFileName(fileName)), true);
                    if (!gameFpk.Contains(fileFpkName)) gameFpk.Add(fileFpkName);
                }

                // Rebuild game FPK
                GzsLib.WriteFpkArchive(fpkPath, "_gamefpk", gameFpk);
                if (!datFiles.Contains(Tools.ToWinPath(fpk))) datFiles.Add(Tools.ToWinPath(fpk));
                Directory.Delete("_modfpk", true);
                Directory.Delete("_gamefpk", true);
            }

            // Copy files for 01.dat, ignoring merged FPKs
            List<ModQarEntry> qarEntries = metaData.ModQarEntries.FindAll(entry => !mergeFpks.Contains(entry.FilePath));
            foreach(ModQarEntry modEntry in qarEntries)
            {
                string sourceFile = Path.Combine("_extr", Tools.ToWinPath(modEntry.FilePath));
                string destFile = Path.Combine("_working", Tools.ToWinPath(modEntry.FilePath));
                string destDir = Path.GetDirectoryName(destFile);

                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                File.Copy(sourceFile, destFile, true);

                if (!datFiles.Contains(modEntry.FilePath)) datFiles.Add(Tools.ToWinPath(modEntry.FilePath));
            }

            // Rebuild 01.dat
            GzsLib.WriteQarArchive(DatPath, "_working", datFiles, 3150048);

            CleanupDirectory();

            return true;
        }

        public static bool InstallMod(string ModFile, bool removeData = true)
        {
            // extract existing DAT file
            if(!ExtractGameArchive()) return false;

            Settings oSet = new Settings();
            oSet.Load();

            // import existing DAT xml
            QarFileXml qarXml = new QarFileXml();
            qarXml.ReadXml(DatXmlPath);

            // extract mod files to temp folder
            if (Directory.Exists("_temp")) Directory.Delete("_temp", true);
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(ModFile, "_temp", "(.*?)");

            // load mod metadata
            ModEntry modMetaData = new ModEntry();
            modMetaData.ReadFromFile("_temp\\metadata.xml");
            File.Delete("_temp\\metadata.xml");

            // check for any FPKs already in 01.dat that need to be merged
            List<string> fpksToMerge = new List<string>();
            foreach (QarEntryXml qarEntry in qarXml.QarEntries)
            {
                string qarExt = Path.GetExtension(qarEntry.FilePath);
                if (qarExt == ".fpk" || qarExt == ".fpkd")
                {
                    // if mod metadata contains fpks that already exist
                    ModFpkEntry m = modMetaData.ModFpkEntries.FirstOrDefault(fpkEntry => Tools.NameToHash(fpkEntry.FpkFile) == Tools.NameToHash(qarEntry.FilePath));
                    if (m != null)
                    {
                        fpksToMerge.Add(Tools.ToQarPath(m.FpkFile));
                    }
                }
            }

            // check base game QARs for FPKS
            GameFiles gameFiles = new GameFiles();
            gameFiles.Load("gamedata.xml");

            foreach (ModFpkEntry Entry in modMetaData.ModFpkEntries)
            {
                ulong fileHash = Tools.NameToHash(Entry.FpkFile);
                GameFile gameFile = gameFiles.FileList.FirstOrDefault(entry => entry.FileHash == fileHash); // see if fpk is part of base data

                if (gameFile != null)
                {
                    if (!fpksToMerge.Contains(Entry.FpkFile)) // if the file was already merged don't add it
                    {
                        // Locate base QAR archive
                        string baseQar = Path.Combine(ModManager.GameDir, "master", gameFile.QarFile);

                        // Extract single file from archive
                        string singleFile = ExtractGameFile(fileHash); 
                        
                        string destFilePath = Path.Combine(ModManager.ExtractedDatDir, Tools.ToWinPath(singleFile));
                        string destFileDir = Path.GetDirectoryName(destFilePath);

                        if (!Directory.Exists(destFileDir)) Directory.CreateDirectory(destFileDir);
                        File.Copy(singleFile, destFilePath, true); // copy base file

                        fpksToMerge.Add(singleFile);
                    }
                }
            }



            if (fpksToMerge.Count > 0)
            {
                // merge fpks
                foreach (string fpkFile in fpksToMerge)
                {
                    string fpkPath = Tools.ToWinPath(fpkFile);
                    string gameFpkPath = Path.Combine(ExtractedDatDir,fpkPath);
                    string gameFpkDir = Path.Combine(ExtractedDatDir,fpkPath.Replace(".", "_"));

                    ModFpkEntry fpkEntry = modMetaData.ModFpkEntries.First(entry => Tools.NameToHash(entry.FpkFile) == Tools.NameToHash(fpkPath));

                    string modFpkPath = Path.Combine("_temp", Tools.ToWinPath(fpkEntry.FpkFile));
                    string modFpkDir = Path.Combine("_temp", Tools.ToWinPath(fpkEntry.FpkFile).Replace(".", "_"));

                    GzsApp.Run(modFpkPath);
                    GzsApp.Run(gameFpkPath);

                    // load existing xml data
                    FpkFileXml fpkXml = new FpkFileXml();
                    fpkXml.ReadXml(gameFpkPath + ".xml");

                    // generate list of files to move and add to xml
                    List<string> filesToMove = new List<string>();
                    foreach (ModFpkEntry file in modMetaData.ModFpkEntries.FindAll(entry => Tools.NameToHash(entry.FpkFile) == Tools.NameToHash(fpkFile)))
                    {
                        filesToMove.Add(file.FilePath);

                        FpkEntryXml eFpk = fpkXml.FpkEntries.FirstOrDefault(entry => entry.FilePath == file.FilePath);
                        if (eFpk == null)
                        {
                            // insert new fpk entries as required
                            fpkXml.FpkEntries.Add(new FpkEntryXml() { FilePath = file.FilePath });
                        }
                    }

                    // create directories and move files
                    foreach (string file in filesToMove)
                    {
                        string fileDir = Path.GetDirectoryName(Path.Combine(gameFpkDir, Tools.ToWinPath(file)));
                        if (!Directory.Exists(fileDir))
                        {
                            Directory.CreateDirectory(fileDir);
                        }
                        File.Copy(Path.Combine(modFpkDir, Tools.ToWinPath(file)), Path.Combine(gameFpkDir, Tools.ToWinPath(file)), true);
                    }

                    fpkXml.WriteXml(gameFpkPath + ".xml");
                    GzsApp.Run(gameFpkPath + ".xml");
                }
            }

            // copy files for new DAT
            foreach (ModQarEntry modQarFile in modMetaData.ModQarEntries)
            {
                string fileName = Tools.ToQarPath(modQarFile.FilePath);
                string fileDir = (ExtractedDatDir + modQarFile.FilePath.Replace("/", "\\")).Substring(0, (ExtractedDatDir + modQarFile.FilePath).LastIndexOf("/"));

                // if file is not already in QAR, add it

                QarEntryXml qE = qarXml.QarEntries.FirstOrDefault(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(modQarFile.FilePath));
                if (qE == null)
                {
                    string fpk = fpksToMerge.FirstOrDefault(afpk => Tools.NameToHash(afpk) == Tools.NameToHash(modQarFile.FilePath));
                    qarXml.QarEntries.Add(new QarEntryXml() { FilePath = Tools.ToQarPath(fpk), Compressed = modQarFile.Compressed, Hash = modQarFile.Hash });
                }

                // copy all files that weren't merged FPKS

                if (fpksToMerge.Count(entry => Tools.NameToHash(entry) == Tools.NameToHash(fileName)) == 0)
                {
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }
                    File.Copy(Path.Combine("_temp",Tools.ToWinPath(modQarFile.FilePath)), Path.Combine(ModManager.ExtractedDatDir,Tools.ToWinPath(modQarFile.FilePath)), true);
                }
            }

            // build XML for new DAT
            qarXml.WriteXml(DatXmlPath);

            // build new DAT
            GzsApp.Run(DatXmlPath);

            // remove temp files
            Directory.Delete("_temp", true);
            Directory.Delete(ExtractedDatDir, true);
            File.Delete(DatXmlPath);

            UpdateDatHash();

            return true;
        }

        public static bool UninstallMod(ModEntry mod)
        {
            if (!ExtractGameArchive()) return false;

            // load xml data
            QarFileXml datXml = new QarFileXml();
            datXml.ReadXml(DatXmlPath);

            // check for fpks
            List<string> modFpks = new List<string>();
            foreach (ModFpkEntry modFpkFile in mod.ModFpkEntries)
            {
                if (datXml.QarEntries.Count(entry => entry.FilePath == modFpkFile.FpkFile) > 0 && !modFpks.Contains(modFpkFile.FpkFile))
                {
                    modFpks.Add(modFpkFile.FpkFile);
                }
            }

            // do the fpks thing
            // unpack fpk
            foreach (string modFpkFile in modFpks)
            {
                string fpkPath = Path.Combine(ExtractedDatDir, Tools.ToWinPath(modFpkFile));
                string fpkDir = Path.Combine(ExtractedDatDir, Tools.ToWinPath(modFpkFile.Replace(".", "_")));

                // check if fpk file exists in game data
                if (File.Exists(fpkPath))
                {
                    // extract fpk
                    GzsApp.Run(fpkPath);

                    FpkFileXml fpkXml = new FpkFileXml(); // load fpk data
                    fpkXml.ReadXml(fpkPath + ".xml");

                    List<FpkEntryXml> fpkList = fpkXml.FpkEntries.ToList();
                    // check if any files left in fpk
                    foreach (FpkEntryXml fpkSubFile in fpkXml.FpkEntries)
                    {
                            fpkList.RemoveAll(entry => entry.FilePath == (fpkSubFile.FilePath));
                    }
                    fpkXml.FpkEntries = fpkList;

                    // if not, remove it
                    if (fpkXml.FpkEntries.Count == 0)
                    {
                        // delete fpk from dat XML
                        datXml.QarEntries.RemoveAll(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(modFpkFile));
                    }
                    else
                    {
                        // rebuild fpk
                        fpkXml.WriteXml(ExtractedDatDir + modFpkFile + ".xml");
                        GzsApp.Run(ExtractedDatDir + modFpkFile + ".xml");
                    }
                }
            }

            // remove mod files from xml
            foreach (ModQarEntry modQEntry in mod.ModQarEntries)
            {
                // delete files, fpks that were de-merged will be ignored
                if (!modFpks.Contains(modQEntry.FilePath))
                {
                    datXml.QarEntries.RemoveAll(entry => entry.FilePath == modQEntry.FilePath);
                }
            }

            // rebuild 01.dat
            datXml.WriteXml(DatXmlPath);

            GzsApp.Run(DatXmlPath);

            DeleteGameArchive();

            UpdateDatHash();

            return true;
        }

        internal static List<ModFpkEntry> BuildFpkList(string SearchDir)
        {
            // build list of all files in directory
            List<string> allFiles = Directory.GetFiles(SearchDir, "*.*", SearchOption.AllDirectories).ToList();
            List<string> fpkFiles = new List<string>();
            List<ModFpkEntry> BuildFpkList = new List<ModFpkEntry>();

            // find fpk/fpkd files
            foreach (string file in allFiles)
            {
                string fileType = GetFileType(file);
                if (fileType == "fpk" || fileType == "fpkd")
                {
                    fpkFiles.Add(file);
                }
            }

            // unpack each archive and add to file list
            foreach (string fpkFile in fpkFiles)
            {
                GzsApp.Run(fpkFile); // unpack fpk
                FpkFileXml gzsFpkXml = new FpkFileXml();
                gzsFpkXml.ReadXml(fpkFile + ".xml");
                string fpkFileName = Tools.ToQarPath(fpkFile.Substring(ExtractedDatDir.Length)); // name of fpk for fpk list

                foreach (FpkEntryXml fpkFileEntry in gzsFpkXml.FpkEntries)
                {
                    BuildFpkList.Add(new ModFpkEntry() { FilePath = fpkFileEntry.FilePath, FpkFile = fpkFileName });
                }
            }

            return BuildFpkList;
        }

        // validates 01.dat MD5 against previous hash
        internal static bool CheckDatHash()
        {
            string datHash = Tools.GetMd5Hash(DatPath);
            Settings oSet = new Settings();
            oSet.Load();
            string hashOld = oSet.GameData.DatHash;
            if (datHash != hashOld) return false;
            return true;
        }

        internal static void UpdateDatHash()
        {
            // updates dat file hash
            string datHash = Tools.GetMd5Hash(DatPath);
            Settings oSet = new Settings();
            oSet.Load();
            oSet.GameData.DatHash = datHash;
            oSet.Save();
        }

        internal static void CleanupModSettings()
        {
            if (!ExtractGameArchive()) return;

            // Load current settings
            Settings oSet = new Settings();
            oSet.Load();

            // Load archive data
            QarFileXml gameQar = new QarFileXml();
            gameQar.ReadXml(DatXmlPath);

            // recurse through all installed mods
            foreach (ModEntry mod in oSet.ModEntries)
            {
                List<string> remQar = new List<string>(); // list of files to remove
                foreach (ModQarEntry modQarFile in mod.ModQarEntries) // check all mod files
                {
                    if (!File.Exists(Path.Combine(ExtractedDatDir, Tools.ToWinPath(modQarFile.FilePath))))
                    {
                        // if the file doesn't exist, it will be removed
                        remQar.Add(modQarFile.FilePath);
                    }
                }
                foreach (string remFile in remQar)
                {
                    mod.ModQarEntries.RemoveAll(entry => entry.FilePath == remFile); // remove files from db
                    mod.ModFpkEntries.RemoveAll(entry => entry.FpkFile == remFile); // fpks from db
                }
            }

            // remove empty mods
            oSet.ModEntries.RemoveAll(entry => entry.ModQarEntries.Count == 0 && entry.ModFpkEntries.Count == 0);
            oSet.Save();

            DeleteGameArchive();
        }

        internal static void DeleteGameArchive()
        {
            if (File.Exists(DatXmlPath)) File.Delete(DatXmlPath);
            if (Directory.Exists(ExtractedDatDir)) Directory.Delete(ExtractedDatDir, true);
        }

        internal static bool ExtractGameArchive()
        {
            // Check installation directory
            if (!ValidInstallPath) return false;

            try
            {
                // Remove existing directory
                if (Directory.Exists(ExtractedDatDir)) Directory.Delete(ExtractedDatDir, true);

                // Check 01.dat exists
                if (!File.Exists(DatPath)) return false;

                // Extract 01.dat archive using GzsTool
                GzsApp.Run(DatPath);

                QarFileXml datFile = new QarFileXml();
                datFile.ReadXml(DatXmlPath);

                Settings settings = new Settings();
                settings.Load();

                // fix up extracted mod filenames
                foreach(QarEntryXml Entry in datFile.QarEntries)
                {
                    foreach(ModEntry mod in settings.ModEntries)
                    {
                        ModQarEntry qe = mod.ModQarEntries.FirstOrDefault(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(Entry.FilePath));
                        if (qe != null)
                        {
                            string FileDir = Path.Combine(ExtractedDatDir, Path.GetDirectoryName(Tools.ToWinPath(qe.FilePath)));
                            if (FileDir == ExtractedDatDir) continue; // don't want to move named files to un-named files
                            if (!Directory.Exists(FileDir)) Directory.CreateDirectory(FileDir);
                            if(File.Exists(Path.Combine(ExtractedDatDir, Tools.ToWinPath(Entry.FilePath))))
                                      File.Move(Path.Combine(ExtractedDatDir, Tools.ToWinPath(Entry.FilePath)),
                                                Path.Combine(ExtractedDatDir, Tools.ToWinPath(qe.FilePath)));
                            Entry.FilePath = qe.FilePath;
                            Entry.Hash = Tools.NameToHash(qe.FilePath);
                        }
                    }
                }

                datFile.WriteXml(DatXmlPath);

                return true;
            } catch
            {
                return false;
            }
        }

        internal static string GetFileType(string FilePath)
        {
            return Path.GetExtension(FilePath).Substring(1);
        }

        internal static List<ModEntry> GetInstalledMods()
        {
            Settings settingsXml = new Settings();
            settingsXml.Load();

            return settingsXml.ModEntries;
        }

        // gets information about the existing 01.dat archive
        internal static GameData RebuildGameData(bool copyBackup = true)
        {
            if(!ExtractGameArchive()) return null;

            if (!Directory.Exists(ExtractedDatDir)) return null;

            GameData buildData = new GameData();

            // Extract game archive and load data
            QarFileXml gameQarXml = new QarFileXml();
            gameQarXml.ReadXml(DatXmlPath);

            // Load currently installed mods
            Settings oSet = new Settings();
            oSet.Load();

            foreach (QarEntryXml gameQarEntry in gameQarXml.QarEntries)
            {
                buildData.GameQarEntries.Add(new ModQarEntry() { FilePath = gameQarEntry.FilePath, Compressed = gameQarEntry.Compressed, Hash = gameQarEntry.Hash });
            }

            buildData.GameFpkEntries = BuildFpkList(ExtractedDatDir);

            // recurse through all installed mods
            foreach (ModEntry mod in oSet.ModEntries)
            {
                // check all files in mod against qar archive
                foreach (ModFpkEntry modFpkFile in mod.ModFpkEntries)
                {
                    buildData.GameFpkEntries.RemoveAll(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(modFpkFile.FilePath) && entry.FpkFile == modFpkFile.FpkFile);
                }
                foreach (ModQarEntry modQarFile in mod.ModQarEntries)
                {
                    buildData.GameQarEntries.RemoveAll(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(modQarFile.FilePath));
                }
            }

            return buildData;
        }

        internal static void DoModdedFpk(string FpkFile)
        {
            string FilePath = Tools.ToQarPath(FpkFile);

        }

        internal static string ExtractGameFile(ulong FileHash)
        {
            // check base game QARs for FPKS
            GameFiles gameFiles = new GameFiles();
            gameFiles.Load("gamedata.xml");

                GameFile gameFile = gameFiles.FileList.FirstOrDefault(entry => entry.FileHash == FileHash);

                if (gameFile != null)
                {
                        // Locate base QAR archive
                        string baseQar = Path.Combine(ModManager.GameDir, "master", gameFile.QarFile);

                        // Extract single file from archive
                        string singleFile = GzsApp.ExtractSingle(baseQar, FileHash);

                        string baseFilePath = Path.Combine(ModManager.GameDir, "master", gameFile.QarFile.Replace(".", "_"), Tools.ToWinPath(singleFile));
                        string destFilePath = Path.Combine(ModManager.ExtractedDatDir, Tools.ToWinPath(singleFile));
                        string destFileDir = Path.GetDirectoryName(destFilePath);

                return baseFilePath;
                }
            return null;
            }
        }

    }