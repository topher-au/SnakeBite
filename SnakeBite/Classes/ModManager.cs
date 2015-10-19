using GzsTool;
using GzsTool.Core.Fpk;
using GzsTool.Core.Qar;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SnakeBite
{
    internal static class ModManager
    {
        internal static string DatPath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat"; } }
        internal static string DatXmlPath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat.xml"; } }
        internal static string ExtractedDatDir { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01_dat"; } }
        internal static string GameDir { get { return Properties.Settings.Default.InstallPath; } }

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

        public static bool InstallMod(string ModFile)
        {
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
            foreach (ModFpkEntry fpkEntry in metaData.ModFpkEntries)
            {
                if (!modFpks.Contains(fpkEntry.FpkFile)) modFpks.Add(fpkEntry.FpkFile);
            }

            List<string> mergeFpks = new List<string>();

            // Check for FPKs in 01.dat
            foreach (string fpk in modFpks)
            {
                string datFile = datFiles.FirstOrDefault(file => Tools.NameToHash(file) == Tools.NameToHash(fpk));
                if (datFile != null && !mergeFpks.Contains(fpk))
                {
                    mergeFpks.Add(fpk);
                }
            }

            var gameData = GzsLib.ReadBaseData();

            var zeroFiles = gameData.FileList.FindAll(entry => entry.QarFile == "0\\00.dat");
            var baseFiles = gameData.FileList.FindAll(entry => entry.QarFile != "0\\00.dat");

            // Check for FPKs in 00.dat
            foreach (string fpk in modFpks)
            {
                GameFile file = zeroFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                if (file != null && !mergeFpks.Contains(fpk))
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
            foreach (string fpk in mergeFpks)
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
            foreach (ModQarEntry modEntry in qarEntries)
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
            UpdateDatHash();
            CleanupDirectory();

            return true;
        }

        public static bool UninstallMod(ModEntry mod)
        {
            CleanupDirectory();

            // Extract game archive
            var datFiles = GzsLib.ExtractArchive<QarFile>(DatPath, "_working");

            // List all FPKs in mod
            List<string> modFpks = new List<string>();
            foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
            {
                if (!modFpks.Contains(fpkEntry.FpkFile)) modFpks.Add(fpkEntry.FpkFile);
            }

            // Extract FPK
            foreach (string fpk in modFpks)
            {
                var fpkFile = GzsLib.ExtractArchive<FpkFile>(Path.Combine("_working", Tools.ToWinPath(fpk)), "_modfpk");
                foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
                {
                    // Remove Mod Files
                    fpkFile.RemoveAll(file => Tools.ToQarPath(file) == Tools.ToQarPath(fpkEntry.FilePath));
                }
                if (fpkFile.Count == 0)
                {
                    datFiles.RemoveAll(file => Tools.NameToHash(file) == Tools.NameToHash(fpk));
                }
                else
                {
                    var gameData = GzsLib.ReadBaseData();

                    var zeroFiles = gameData.FileList.FindAll(entry => entry.QarFile == "0\\00.dat");
                    var baseFiles = gameData.FileList.FindAll(entry => entry.QarFile != "0\\00.dat");

                    // Check for FPKs in 00.dat first
                    GameFile file = zeroFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                    if (file != null)
                    {
                        // Extract base FPK files
                        GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\0\\00.dat"), file.FileHash, "_working\\temp.fpk");
                        var gameFpk = GzsLib.ExtractArchive<FpkFile>("_working\\temp.fpk", "_gamefpk");

                        // Copy remaining files over base FPK
                        foreach (string mFile in fpkFile)
                        {
                            string fDir = Path.GetDirectoryName(mFile);
                            if (!Directory.Exists(Path.Combine("_gamefpk", fDir))) Directory.CreateDirectory(Path.Combine("_gamefpk", fDir));
                            File.Copy(Path.Combine("_modfpk", mFile), Path.Combine(Path.Combine("_gamefpk", mFile)), true);
                            if (!gameFpk.Contains(mFile)) gameFpk.Add(mFile);
                        }

                        // Rebuild FPK
                        GzsLib.WriteFpkArchive(Path.Combine("_working", Tools.ToWinPath(fpk)), "_gamefpk", gameFpk);
                        continue; // don't check base data if it's in 00
                    }

                    // check base files for FPK
                    file = baseFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                    if (file != null)
                    {
                        // Extract base FPK files
                        GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\" + file.QarFile), file.FileHash, "_working\\temp.fpk");
                        var gameFpk = GzsLib.ExtractArchive<FpkFile>("_working\\temp.fpk", "_gamefpk");

                        // Copy remaining files over base FPK
                        foreach (string mFile in fpkFile)
                        {
                            string fDir = Path.GetDirectoryName(mFile);
                            if (!Directory.Exists(Path.Combine("_gamefpk", fDir))) Directory.CreateDirectory(Path.Combine("_gamefpk", fDir));
                            File.Copy(Path.Combine("_modfpk", mFile), Path.Combine(Path.Combine("_gamefpk", mFile)), true);
                            if (!gameFpk.Contains(mFile)) gameFpk.Add(mFile);
                        }

                        // Rebuild FPK
                        GzsLib.WriteFpkArchive(Path.Combine("_working", Tools.ToWinPath(fpk)), "_gamefpk", gameFpk);
                        Directory.Delete("_gamefpk", true);
                        Directory.Delete("_modfpk", true);
                    }
                }
            }

            // Remove all mod files from 01.dat
            foreach (ModQarEntry qarEntry in mod.ModQarEntries)
            {
                string fExt = Path.GetExtension(qarEntry.FilePath);
                if (!fExt.Contains(".fpk"))
                {
                    datFiles.RemoveAll(file => Tools.NameToHash(file) == Tools.NameToHash(qarEntry.FilePath));
                }
            }

            // Rebuild 01.dat
            GzsLib.WriteQarArchive(DatPath, "_working", datFiles, 3150048);
            CleanupDirectory();
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

        internal static void DoModdedFpk(string FpkFile)
        {
            string FilePath = Tools.ToQarPath(FpkFile);
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
                foreach (QarEntryXml Entry in datFile.QarEntries)
                {
                    foreach (ModEntry mod in settings.ModEntries)
                    {
                        ModQarEntry qe = mod.ModQarEntries.FirstOrDefault(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(Entry.FilePath));
                        if (qe != null)
                        {
                            string FileDir = Path.Combine(ExtractedDatDir, Path.GetDirectoryName(Tools.ToWinPath(qe.FilePath)));
                            if (FileDir == ExtractedDatDir) continue; // don't want to move named files to un-named files
                            if (!Directory.Exists(FileDir)) Directory.CreateDirectory(FileDir);
                            if (File.Exists(Path.Combine(ExtractedDatDir, Tools.ToWinPath(Entry.FilePath))))
                                File.Move(Path.Combine(ExtractedDatDir, Tools.ToWinPath(Entry.FilePath)),
                                          Path.Combine(ExtractedDatDir, Tools.ToWinPath(qe.FilePath)));
                            Entry.FilePath = qe.FilePath;
                            Entry.Hash = Tools.NameToHash(qe.FilePath);
                        }
                    }
                }

                datFile.WriteXml(DatXmlPath);

                return true;
            }
            catch
            {
                return false;
            }
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

        internal static int GetMGSVersion()
        {
            // Get MGSV executable version
            var versionInfo = FileVersionInfo.GetVersionInfo(Properties.Settings.Default.InstallPath + "\\mgsvtpp.exe");
            string version = versionInfo.ProductVersion;
            return Convert.ToInt32(version.Replace(".", ""));
        }

        internal static int GetSBVersion()
        {
            // Get SB app version
            string assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return Convert.ToInt32(assemblyVersion.Replace(".", ""));
        }
        // gets information about the existing 01.dat archive
        internal static GameData RebuildGameData(bool copyBackup = true)
        {
            if (!ExtractGameArchive()) return null;

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

        internal static void UpdateDatHash()
        {
            // updates dat file hash
            string datHash = Tools.GetMd5Hash(DatPath);
            Settings oSet = new Settings();
            oSet.Load();
            oSet.GameData.DatHash = datHash;
            oSet.Save();
        }

        private static void CleanupDirectory()
        {
            if (Directory.Exists("_working")) Directory.Delete("_working", true);
            if (Directory.Exists("_extr")) Directory.Delete("_extr", true);
            if (Directory.Exists("_gamefpk")) Directory.Delete("_gamefpk", true);
            if (Directory.Exists("_modfpk")) Directory.Delete("_modfpk", true);
        }
    }
}