using GzsTool;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SnakeBite
{
    internal static class ModManager
    {
        internal static string GameDir { get { return Properties.Settings.Default.InstallPath; } }
        internal static string ExtractedDatDir { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01_dat"; } }
        internal static string DatPath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat"; } }
        internal static string DatXmlPath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat.xml"; } }
        private static BackupManager backupManager = new BackupManager();

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

        public static bool InstallMod(string ModFile)
        {
            if (!ModManager.ValidInstallPath) return false; // no valid install specified

            if (!File.Exists(DatPath)) return false;

            // extract existing DAT file
            ExtractGameArchive();

            Settings oSet = new Settings();
            oSet.Load();

            // import existing DAT xml
            QarFile qarXml = new QarFile();
            qarXml.ReadXml(DatXmlPath);

            // extract mod files to temp folder
            if (Directory.Exists("_temp")) Directory.Delete("_temp", true);
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(ModFile, "_temp", "(.*?)");

            // load mod metadata
            ModEntry modMetadata = new ModEntry();
            modMetadata.ReadFromFile("_temp\\metadata.xml");
            File.Delete("_temp\\metadata.xml");

            // check for any FPKs installed into 01.dat to be merged
            List<ModFpkEntry> installedFpkFiles = new List<ModFpkEntry>();
            foreach (QarEntry gzsQar in qarXml.QarEntries)
            {
                string qarExt = gzsQar.FilePath.Substring(gzsQar.FilePath.LastIndexOf(".") + 1).ToLower();
                if (qarExt == "fpk" || qarExt == "fpkd")
                {
                    // extract FPK and add files to list
                    string fpkFile = ExtractedDatDir + Tools.ToWinPath(gzsQar.FilePath);
                    string fpkDir = fpkFile.Replace(".", "_");

                    if (modMetadata.ModFpkEntries.Count(entry => Tools.ToQarPath(entry.FpkFile) == Tools.ToQarPath(gzsQar.FilePath)) > 0)
                    {
                        GzsApp.Run(fpkFile);

                        List<string> fpkFiles = Directory.GetFiles(fpkDir, "*.*", SearchOption.AllDirectories).ToList();
                        foreach (string file in fpkFiles)
                        {
                            string fpkFilePath = fpkFile.Substring(ExtractedDatDir.Length);
                            fpkFilePath = fpkFilePath.Replace("\\", "/");

                            string fpkPath = file.Substring(file.LastIndexOf("Assets\\"));
                            fpkPath = "/" + fpkPath.Replace("\\", "/");

                            installedFpkFiles.Add(new ModFpkEntry() { FilePath = fpkPath, FpkFile = fpkFilePath });
                        }
                    }
                }
            }

            List<string> mergeFpks = new List<string>();

            // check base game QARs for FPKS
            GameFiles gFiles = new GameFiles();
            gFiles.Load("gamedata.xml");
            foreach (ModFpkEntry modFpk in modMetadata.ModFpkEntries)
            {
                ulong fileHash = Hashing.HashFileNameWithExtension(modFpk.FpkFile);
                GameFile gFile = gFiles.FileList.FirstOrDefault(entry => entry.FileHash == fileHash); // see if fpk is part of base data

                if (gFile != null)
                {
                    if (!mergeFpks.Contains(modFpk.FpkFile))
                    {
                        string gQar = Path.Combine(ModManager.GameDir, "master", gFile.QarFile);
                        string gQarFile = Path.Combine(ModManager.GameDir, "master", gFile.QarFile.Replace(".", "_"), Tools.ToWinPath(modFpk.FpkFile).TrimStart('\\'));
                        string gDestFile = Path.Combine(ModManager.ExtractedDatDir, Tools.ToWinPath(modFpk.FpkFile).TrimStart('\\'));
                        string gDestDir = Path.GetDirectoryName(gDestFile);

                        GzsApp.ExtractSingle(gQar, fileHash);

                        if (!Directory.Exists(gDestDir)) Directory.CreateDirectory(gDestDir);
                        File.Copy(gQarFile, gDestFile, true); // copy base file

                        mergeFpks.Add(modFpk.FpkFile);
                    }
                }
            }

            // compare lists and build merge fpk list

            foreach (ModFpkEntry installedFpk in installedFpkFiles)
            {
                foreach (ModEntry mod in GetInstalledMods())
                {
                    if (mod.ModFpkEntries.FirstOrDefault(entry => entry.FpkFile == installedFpk.FpkFile) != null) // if the mod has an fpk that should be merged with an installed fpk
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
                    string fpkPath = Tools.ToWinPath(fpkFile);
                    string gameFpkPath = ExtractedDatDir + fpkPath;
                    string gameFpkDir = ExtractedDatDir + fpkPath.Replace(".", "_");
                    string modFpkPath = "_temp" + Tools.ToWinPath(fpkPath);
                    string modFpkDir = "_temp" + Tools.ToWinPath(fpkPath.Replace(".", "_"));

                    GzsApp.Run(modFpkPath, false);
                    GzsApp.Run(gameFpkPath);

                    // load existing xml data
                    FpkFile fpkXml = new FpkFile();
                    fpkXml.ReadXml(gameFpkPath + ".xml");

                    // generate list of files to move and add to xml
                    List<string> filesToMove = new List<string>();
                    foreach (ModFpkEntry file in modMetadata.ModFpkEntries.FindAll(entry => entry.FpkFile == fpkFile))
                    {
                        filesToMove.Add(Tools.ToWinPath(file.FilePath));

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
                        if (File.Exists(gameFpkDir + Tools.ToWinPath(file)))
                        {
                            backupManager.Load();
                            BackupFile backup = backupManager.backupData.BackupFiles.FirstOrDefault(backupFile => backupFile.FilePath == Tools.ToQarPath(file) && backupFile.FpkFile == fpkFile);
                            if (backup == null)
                            {
                                // check if file is on system files list
                                if (oSet.GameData.GameFpkEntries.FirstOrDefault(fpk => fpk.FpkFile == fpkFile && fpk.FilePath == file) != null)
                                {
                                    // if so do backup
                                    backupManager.AddFile(gameFpkDir + Tools.ToWinPath(file), Tools.ToQarPath(file), fpkFile);
                                    backupManager.Save();
                                }
                            }
                        }
                        File.Copy(modFpkDir + file, gameFpkDir + file, true);
                    }

                    fpkXml.WriteXml(gameFpkPath + ".xml");
                    GzsApp.Run(gameFpkPath + ".xml");
                }
            }

            // copy files for new DAT
            foreach (ModQarEntry modQarFile in modMetadata.ModQarEntries)
            {
                string fileName = Tools.ToQarPath(modQarFile.FilePath);
                string fileDir = (ExtractedDatDir + modQarFile.FilePath.Replace("/", "\\")).Substring(0, (ExtractedDatDir + modQarFile.FilePath).LastIndexOf("/"));

                // if file is not already in QAR, add it
                if (qarXml.QarEntries.FirstOrDefault(entry => entry.FilePath == modQarFile.FilePath) == null)
                {
                    qarXml.QarEntries.Add(new QarEntry() { FilePath = modQarFile.FilePath, Compressed = modQarFile.Compressed, Hash = modQarFile.Hash });
                }

                // copy all files that weren't merged FPKS
                if (!mergeFpks.Contains(fileName))
                {
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }
                    if (File.Exists(ModManager.ExtractedDatDir + Tools.ToWinPath(modQarFile.FilePath)))
                    {
                        backupManager.Load();
                        BackupFile backup = backupManager.backupData.BackupFiles.FirstOrDefault(file => file.FilePath == modQarFile.FilePath);
                        if (backup == null)
                        {
                            if (oSet.GameData.GameQarEntries.FirstOrDefault(file => Tools.ToQarPath(file.FilePath) == Tools.ToQarPath(modQarFile.FilePath)) != null)
                            {
                                // system file, attempt to create backup
                                backupManager.AddFile(ModManager.ExtractedDatDir + Tools.ToWinPath(modQarFile.FilePath), modQarFile.FilePath);
                                backupManager.Save();
                            }
                        }
                    }
                    File.Copy("_temp" + Tools.ToWinPath(modQarFile.FilePath), ModManager.ExtractedDatDir + Tools.ToWinPath(modQarFile.FilePath), true);
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
            // extract 01.dat
            if (!ModManager.ValidInstallPath) return false; // no valid install specified

            if (!File.Exists(DatPath)) return false;

            ExtractGameArchive();

            // load xml data
            QarFile datXml = new QarFile();
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
            foreach (string fpkFile in modFpks)
            {
                // check if fpk file exists in game data
                if (File.Exists(ExtractedDatDir + Tools.ToWinPath(fpkFile)))
                {
                    // extract fpk
                    GzsApp.Run(ExtractedDatDir + Tools.ToWinPath(fpkFile));

                    string fpkDir = ExtractedDatDir + Tools.ToWinPath(fpkFile.Replace(".", "_"));
                    FpkFile fpkXml = new FpkFile(); // load fpk data
                    fpkXml.ReadXml(ExtractedDatDir + fpkFile + ".xml");

                    List<FpkEntry> fpkList = fpkXml.FpkEntries.ToList();
                    // check if any files left in fpk
                    foreach (FpkEntry fpkSubFile in fpkXml.FpkEntries)
                    {
                        backupManager.Load();
                        BackupFile bFile = backupManager.backupData.BackupFiles.FirstOrDefault(entry => entry.FilePath == fpkSubFile.FilePath && entry.FpkFile == fpkFile);
                        if (bFile != null)
                        {
                            // if a backup exists, restore it
                            backupManager.RestoreFile(bFile);
                        }
                        else
                        {
                            // remove file from fpk
                            fpkList.RemoveAll(entry => entry.FilePath == fpkSubFile.FilePath);
                        }
                    }
                    fpkXml.FpkEntries = fpkList;

                    // if not, remove it
                    if (fpkXml.FpkEntries.Count == 0)
                    {
                        // delete fpk from dat XML
                        datXml.QarEntries.RemoveAll(entry => entry.FilePath == fpkFile);
                    }
                    else
                    {
                        // rebuild fpk
                        fpkXml.WriteXml(ExtractedDatDir + fpkFile + ".xml");
                        GzsApp.Run(ExtractedDatDir + fpkFile + ".xml");
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
                FpkFile gzsFpkXml = new FpkFile();
                gzsFpkXml.ReadXml(fpkFile + ".xml");
                string fpkFileName = fpkFile.Substring(ExtractedDatDir.Length).Replace("\\", "/"); // name of fpk for fpk list

                foreach (FpkEntry fpkFileEntry in gzsFpkXml.FpkEntries)
                {
                    BuildFpkList.Add(new ModFpkEntry() { FilePath = fpkFileEntry.FilePath, FpkFile = fpkFileName });
                }
            }

            return BuildFpkList;
        }

        // validates 01.dat MD5 against previous hash
        internal static bool CheckDatHash()
        {
            string datHash = Tools.HashFile(DatPath);
            Settings oSet = new Settings();
            oSet.Load();
            string hashOld = oSet.GameData.DatHash;
            if (datHash != hashOld) return false;
            return true;
        }

        internal static void UpdateDatHash()
        {
            // updates dat file hash
            string datHash = Tools.HashFile(DatPath);
            Settings oSet = new Settings();
            oSet.Load();
            oSet.GameData.DatHash = datHash;
            oSet.Save();
        }

        internal static void CleanupModSettings()
        {
            // Load current settings
            Settings oSet = new Settings();
            oSet.Load();

            // Unpack game archive
            ExtractGameArchive();

            // Load archive data
            QarFile gameQar = new QarFile();
            gameQar.ReadXml(DatXmlPath);

            // recurse through all installed mods
            foreach (ModEntry mod in oSet.ModEntries)
            {
                List<string> remQar = new List<string>(); // list of files to remove
                foreach (ModQarEntry modQarFile in mod.ModQarEntries) // check all mod files
                {
                    if (!File.Exists(ExtractedDatDir + Tools.ToWinPath(modQarFile.FilePath)))
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

        internal static void ExtractGameArchive()
        {
            // extract 01.dat archive using GzsTool
            GzsApp.Run(DatPath);
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
            if (Directory.Exists(ExtractedDatDir)) Directory.Delete(ExtractedDatDir, true);
            ExtractGameArchive();

            if (!Directory.Exists(ExtractedDatDir)) return null;

            GameData buildData = new GameData();

            // Extract game archive and load data
            QarFile gameQarXml = new QarFile();
            gameQarXml.ReadXml(DatXmlPath);

            // Load currently installed mods
            Settings oSet = new Settings();
            oSet.Load();

            foreach (QarEntry gameQarEntry in gameQarXml.QarEntries)
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
                    buildData.GameFpkEntries.RemoveAll(entry => entry.FilePath == modFpkFile.FilePath && entry.FpkFile == modFpkFile.FpkFile);
                }
                foreach (ModQarEntry modQarFile in mod.ModQarEntries)
                {
                    buildData.GameQarEntries.RemoveAll(entry => entry.FilePath == modQarFile.FilePath);
                }
            }

            if (copyBackup)
            {
                foreach (ModFpkEntry fpkEntry in buildData.GameFpkEntries)
                {
                    string fpkDir = Tools.ToWinPath(fpkEntry.FpkFile.Replace(".", "_"));
                    backupManager.AddFile(ExtractedDatDir + fpkDir + Tools.ToWinPath(fpkEntry.FilePath), fpkEntry.FilePath, fpkEntry.FpkFile);
                }

                foreach (ModQarEntry qarEntry in buildData.GameQarEntries)
                {
                    backupManager.AddFile(ExtractedDatDir + Tools.ToWinPath(qarEntry.FilePath), Tools.ToQarPath(qarEntry.FilePath));
                }
                backupManager.Save();
            }

            DeleteGameArchive();

            return buildData;
        }
    }
}