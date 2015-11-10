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
        internal static string GameDir { get { return Properties.Settings.Default.InstallPath; } }

        public static bool InstallMod(string ModFile)
        {
            CleanupFolders();

            Debug.LogLine(String.Format("[Mod] Installation started: {0}", ModFile), Debug.LogLevel.Basic);

            // Extract game archive
            var oneFiles = GzsLib.ExtractArchive<QarFile>(DatPath, "_working");
            //oneFiles = FixModFilenames(oneFiles, "_working");

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
            List<ModQarEntry> MergeFiles = new List<ModQarEntry>();

            Debug.LogLine("[Mod] Checking existing game data", Debug.LogLevel.Basic);

            // Check for FPKs in 01.dat
            foreach (string fpk in modFpks)
            {
                string datFile = oneFiles.FirstOrDefault(file => Tools.NameToHash(file) == Tools.NameToHash(fpk));
                if (datFile != null)
                {
                    if (mergeFpks.Contains(Tools.ToQarPath(datFile))) continue;
                    mergeFpks.Add(fpk);

                    MergeFiles.Add(new ModQarEntry() { FilePath = fpk, SourceType = FileSource.Merged, SourceName= "01.dat" });
                }
            }

            var gameData = GzsLib.ReadBaseData();

            var zeroFiles = gameData.FileList.FindAll(entry => entry.QarFile == "00.dat");
            var baseFiles = gameData.FileList.FindAll(entry => entry.QarFile != "00.dat");

            // Check for FPKs in 00.dat
            foreach (string fpk in modFpks)
            {
                GameFile file = zeroFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                if (file != null)
                {
                    if (mergeFpks.Contains(Tools.ToQarPath(file.FilePath))) continue;

                    // Create destination directory
                    string destDirectory = Path.Combine("_working", Path.GetDirectoryName(Tools.ToWinPath(file.FilePath)));
                    if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);

                    // Extract file into dat directory
                    var ex = GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\0\\00.dat"), file.FileHash, Path.Combine("_working", Tools.ToWinPath(file.FilePath)));
                    mergeFpks.Add(Tools.ToQarPath(file.FilePath));

                    MergeFiles.Add(new ModQarEntry() { FilePath = file.FilePath, SourceType = FileSource.Merged, SourceName = "00.dat" });

                    if (oneFiles.FirstOrDefault(datFile => Tools.NameToHash(datFile) == Tools.NameToHash(file.FilePath)) == null)
                    {
                        oneFiles.Add(Tools.ToWinPath(file.FilePath));
                    }
                }
            }

            // Check for FPKs in base data
            foreach (string fpk in modFpks)
            {
                GameFile file = baseFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                if (file != null)
                {
                    if (mergeFpks.Contains(Tools.ToQarPath(file.FilePath))) continue;

                    // Create destination directory
                    string destDirectory = Path.Combine("_working", Path.GetDirectoryName(Tools.ToWinPath(file.FilePath)));
                    if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);

                    // Extract file into dat directory
                    var ex = GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\" + file.QarFile), file.FileHash, Path.Combine("_working", Tools.ToWinPath(file.FilePath)));
                    mergeFpks.Add(Tools.ToQarPath(file.FilePath));
                    MergeFiles.Add(new ModQarEntry() { FilePath = file.FilePath, SourceType = FileSource.Merged, SourceName = file.QarFile });

                    if (oneFiles.FirstOrDefault(datFile => Tools.NameToHash(datFile) == Tools.NameToHash(file.FilePath)) == null)
                    {
                        oneFiles.Add(Tools.ToWinPath(file.FilePath));
                    }
                }
            }

            Debug.LogLine(String.Format("[Mod] Merging {0} FPK files", MergeFiles.Count), Debug.LogLevel.Basic);

            var g = SettingsManager.GetGameData();

            // Merge FPK files
            foreach (ModQarEntry gf in MergeFiles)
            {
                Debug.LogLine(String.Format("[Mod] Starting merge: {0} ({1})", gf.FilePath, gf.SourceName), Debug.LogLevel.Debug);
                // Extract game FPK
                string fpkDatPath = oneFiles.FirstOrDefault(file => Tools.NameToHash(file) == Tools.NameToHash(gf.FilePath));
                string fpkPath = Path.Combine("_working", Tools.ToWinPath(fpkDatPath));
                var gameFpk = GzsLib.ExtractArchive<FpkFile>(fpkPath, "_gamefpk");

                // Extract mod FPK
                var exFpk = GzsLib.ExtractArchive<FpkFile>(Path.Combine("_extr", Tools.ToWinPath(gf.FilePath)), "_modfpk");

                // Add file to gamedata info
                var q = g.GameQarEntries.FirstOrDefault(entry => entry.FilePath == gf.FilePath);
                if (q == null) g.GameQarEntries.Add(new ModQarEntry() { FilePath = Tools.ToQarPath(gf.FilePath), SourceType = gf.SourceType, SourceName = gf.SourceName, Hash = Tools.NameToHash(gf.FilePath) });

                foreach (string f in gameFpk)
                {
                    var c = exFpk.FirstOrDefault(entry => Tools.NameToHash(entry) == Tools.NameToHash(f));
                    if (c == null)
                    {
                        if (g.GameFpkEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FpkFile, gf.FilePath) && Tools.CompareNames(entry.FilePath, f)) == null)
                        {
                            g.GameFpkEntries.Add(new ModFpkEntry() { FpkFile = Tools.ToQarPath(gf.FilePath), FilePath = f, SourceType = gf.SourceType, SourceName = gf.SourceName });
                        }
                    }
                }

                // Merge contents
                foreach (string fileName in exFpk)
                {
                    string fileDir = (Path.Combine("_gamefpk", Path.GetDirectoryName(fileName)));
                    string sourceFile = Path.Combine("_modfpk", fileName);
                    string destFile = Path.Combine("_gamefpk", fileName);

                    Debug.LogLine(String.Format("[Mod] Copying file: {0}", fileName), Debug.LogLevel.All);

                    if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
                    File.Copy(sourceFile, destFile, true);
                    if (!gameFpk.Contains(fileName)) gameFpk.Add(Tools.ToQarPath(fileName));
                }

                // Rebuild game FPK
                GzsLib.WriteFpkArchive(fpkPath, "_gamefpk", gameFpk);
                if (!oneFiles.Contains(Tools.ToWinPath(gf.FilePath))) oneFiles.Add(Tools.ToWinPath(gf.FilePath));
                Directory.Delete("_modfpk", true);
                Directory.Delete("_gamefpk", true);
                Debug.LogLine(String.Format("[Mod] Merge complete"), Debug.LogLevel.Debug);
            }

            SettingsManager.SetGameData(g);

            Debug.LogLine("[Mod] Copying remaining mod files", Debug.LogLevel.Basic);

            // Copy files for 01.dat, ignoring merged FPKs
            foreach (ModQarEntry modEntry in metaData.ModQarEntries)
            {
                if (!oneFiles.Contains(Tools.ToWinPath(modEntry.FilePath))) oneFiles.Add(Tools.ToWinPath(modEntry.FilePath));

                if (modEntry.FilePath.Contains(".fpk"))
                    if (mergeFpks.Count(fpk => Tools.NameToHash(fpk) == Tools.NameToHash(modEntry.FilePath)) > 0)
                        continue;

                string sourceFile = Path.Combine("_extr", Tools.ToWinPath(modEntry.FilePath));
                string destFile = Path.Combine("_working", Tools.ToWinPath(modEntry.FilePath));
                string destDir = Path.GetDirectoryName(destFile);

                Debug.LogLine(String.Format("[Mod] Copying file: {0}", modEntry.FilePath), Debug.LogLevel.All);
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                File.Copy(sourceFile, destFile, true);
            }

            // Rebuild 01.dat
            Debug.LogLine("[Mod] Rebuilding game archive", Debug.LogLevel.Basic);
            GzsLib.WriteQarArchive(DatPath, "_working", oneFiles, 3150048);

            SettingsManager.UpdateDatHash();

            SettingsManager.AddMod(metaData);

            Debug.LogLine("[Mod] Running database cleanup", Debug.LogLevel.Debug);
            CleanupDatabase();

            CleanupFolders();

            Debug.LogLine("[Mod] Installation finished", Debug.LogLevel.Basic);

            return true;
        }

        public static bool UninstallMod(ModEntry mod)
        {
            Debug.LogLine(String.Format("[Mod] Uninstall started: {0}", mod.Name), Debug.LogLevel.Basic);

            CleanupFolders();

            // Extract game archive
            var datFiles = GzsLib.ExtractArchive<QarFile>(DatPath, "_working");

            // List all FPKs in mod
            List<string> modFpks = new List<string>();
            foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
            {
                if (!modFpks.Contains(fpkEntry.FpkFile)) modFpks.Add(fpkEntry.FpkFile);
            }

            var gameData = SettingsManager.GetGameData();

            // Extract FPK
            foreach (string fpk in modFpks)
            {
                string fpkName = Path.GetFileName(fpk);
                string fpkDatPath = datFiles.First(file => Tools.NameToHash(file) == Tools.NameToHash(fpk));
                var fpkFile = GzsLib.ExtractArchive<FpkFile>(Path.Combine("_working", Tools.ToWinPath(fpkDatPath)), "_modfpk");

                // Remove all mod fpk files from fpk
                foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
                {
                    Debug.LogLine(String.Format("[Mod] Removing {1}\\{0}", Tools.ToWinPath(fpkEntry.FilePath), fpkName), Debug.LogLevel.Debug);
                    fpkFile.RemoveAll(file => Tools.ToQarPath(file) == Tools.ToQarPath(fpkEntry.FilePath));
                }

                var gameFpks = gameData.GameFpkEntries.ToList();

                // remove all merged files from fpk
                foreach (ModFpkEntry gameFpkFile in gameFpks)
                {
                    if (Tools.ToQarPath(gameFpkFile.FpkFile) == Tools.ToQarPath(fpk) && gameFpkFile.SourceType == FileSource.Merged)
                    {
                        Debug.LogLine(String.Format("[Mod] Removing merged file {0}", gameFpkFile.FilePath));
                        fpkFile.RemoveAll(entry => entry == gameFpkFile.FilePath);
                        gameData.GameFpkEntries.Remove(gameFpkFile);
                    }
                }

                // remove fpk if no files left
                if (fpkFile.Count == 0)
                {
                    Debug.LogLine(String.Format("[Mod] {0} is empty, removing", fpkName), Debug.LogLevel.Debug);
                    datFiles.RemoveAll(file => Tools.NameToHash(file) == Tools.NameToHash(fpk));
                    gameData.GameQarEntries.RemoveAll(file => Tools.NameToHash(file.FilePath) == Tools.NameToHash(fpk));
                }

                else
                {
                    Debug.LogLine(String.Format("[Mod] Rebuilding {0}", fpk), Debug.LogLevel.Debug);
                    // rebuild fpk from base file
                    var baseData = GzsLib.ReadBaseData();

                    var zeroFiles = baseData.FileList.FindAll(entry => entry.QarFile == "00.dat");
                    var baseFiles = baseData.FileList.FindAll(entry => entry.QarFile != "00.dat");

                    // Check for FPKs in 00.dat first
                    GameFile file = zeroFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                    if (file != null)
                    {

                        // Extract base FPK files
                        GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\0\\00.dat"), file.FileHash, "_working\\temp.fpk");
                        var gameFpk = GzsLib.ExtractArchive<FpkFile>("_working\\temp.fpk", "_gamefpk");

                        // Add merged base files to game file database
                        var mCount = 0;
                        foreach(var fpkF in gameFpk)
                        {
                            if(!fpkFile.Contains(fpkF))
                            {
                                gameData.GameFpkEntries.Add(new ModFpkEntry() { FpkFile = fpk, FilePath = fpkF, SourceType = FileSource.Merged, SourceName = file.QarFile });
                                mCount++;
                            }
                        }

                        Debug.LogLine(String.Format("[Mod] {0} files restored from {1}", mCount, file.QarFile), Debug.LogLevel.Debug);

                        // Copy remaining files over base FPK
                        foreach (string mFile in fpkFile)
                        {
                            string fDir = Path.GetDirectoryName(mFile);
                            if (!Directory.Exists(Path.Combine("_gamefpk", fDir))) Directory.CreateDirectory(Path.Combine("_gamefpk", fDir));
                            Debug.LogLine(String.Format("[Mod] Merging existing file: {0}", mFile));
                            File.Copy(Path.Combine("_modfpk", mFile), Path.Combine(Path.Combine("_gamefpk", mFile)), true);
                            if (!gameFpk.Contains(mFile)) gameFpk.Add(mFile);
                        }

                        // Rebuild FPK
                        GzsLib.WriteFpkArchive(Path.Combine("_working", Tools.ToWinPath(fpkDatPath)), "_gamefpk", gameFpk);
                        Directory.Delete("_gamefpk", true);
                        Directory.Delete("_modfpk", true);
                        continue; // don't check base data if it's in 00
                    }

                    // check base files for FPK
                    file = baseFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                    if (file != null)
                    {
                        // Extract base FPK files
                        GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\" + file.QarFile), file.FileHash, "_working\\temp.fpk");
                        var gameFpk = GzsLib.ExtractArchive<FpkFile>("_working\\temp.fpk", "_gamefpk");

                        // Add merged base files to game file database
                        var mCount = 0;
                        foreach (var fpkF in gameFpk)
                        {
                            if (!fpkFile.Contains(fpkF))
                            {
                                gameData.GameFpkEntries.Add(new ModFpkEntry() { FpkFile = fpk, FilePath = fpkF, SourceType = FileSource.Merged, SourceName = file.QarFile });
                                mCount++;
                            }
                        }

                        Debug.LogLine(String.Format("[Mod] {0} files restored from {1}", mCount, file.QarFile), Debug.LogLevel.Debug);
                        // Copy remaining files over base FPK
                        foreach (string mFile in fpkFile)
                        {
                            string fDir = Path.GetDirectoryName(mFile);
                            if (!Directory.Exists(Path.Combine("_gamefpk", fDir))) Directory.CreateDirectory(Path.Combine("_gamefpk", fDir));
                            Debug.LogLine(String.Format("[Mod] Merging existing file: {0}", mFile));
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

            SettingsManager.SetGameData(gameData);

            // Remove all mod files from 01.dat
            foreach (ModQarEntry qarEntry in mod.ModQarEntries)
            {
                string fExt = Path.GetExtension(qarEntry.FilePath);
                if (!fExt.Contains(".fpk"))
                {
                    datFiles.RemoveAll(file => Tools.NameToHash(file) == Tools.NameToHash(qarEntry.FilePath));
                }
            }

            Debug.LogLine("[Mod] Rebuilding game archive", Debug.LogLevel.Basic);
            // Rebuild 01.dat
            GzsLib.WriteQarArchive(DatPath, "_working", datFiles, 3150048);
            SettingsManager.UpdateDatHash();

            SettingsManager.RemoveMod(mod);

            CleanupDatabase();

            CleanupFolders();
            Debug.LogLine("[Mod] Uninstall complete", Debug.LogLevel.Basic);

            return true;
        }

        public static void CleanupDatabase()
        {
            Debug.LogLine("[Cleanup] Database cleanup started", Debug.LogLevel.Basic);

            // Retrieve installation data
            var mods = SettingsManager.GetInstalledMods();
            var game = SettingsManager.GetGameData();
            var datFiles = GzsLib.ListArchiveContents<QarFile>(DatPath);

            Debug.LogLine("[Cleanup] Checking duplicate file entried", Debug.LogLevel.Debug);
            // Remove duplicate file entries
            var cleanFiles = datFiles.ToList();
            foreach(string file in datFiles)
            {
                while(cleanFiles.Count(entry => entry == file) > 1)
                {
                    cleanFiles.Remove(file);
                    Debug.LogLine(String.Format("[Cleanup] Found duplicate entry: {0}", file), Debug.LogLevel.Debug);
                }
            }

            var GameFpks = game.GameFpkEntries.ToList(); ;

            // Search for FPKs in game data
            var fpkFiles = cleanFiles.FindAll(entry => entry.Contains(".fpk"));
            foreach(string fpkFile in fpkFiles)
            {
                // Extract FPK from archive
                Debug.LogLine(String.Format("[Cleanup] Extract and read {0}", fpkFile), Debug.LogLevel.All);
                GzsLib.ExtractFile<QarFile>(DatPath, fpkFile, "_temp.fpk");

                // Read FPK contents
                var fpkContent = GzsLib.ListArchiveContents<FpkFile>("_temp.fpk");
                
                // Add contents to game FPK list
                foreach (var c in fpkContent)
                {
                    if(GameFpks.Count(entry => Tools.CompareNames(entry.FilePath, c) && Tools.CompareHashes(entry.FpkFile, fpkFile)) == 0)
                    {
                        GameFpks.Add(new ModFpkEntry() { FpkFile = fpkFile, FilePath = c });
                    }
                }

                File.Delete("_temp.fpk");
            }

            Debug.LogLine("[Cleanup] Checking installed mods", Debug.LogLevel.Debug);
            // Remove all installed mod data from game data list
            foreach (var mod in mods)
            {
                foreach(var qarEntry in mod.ModQarEntries)
                {
                    cleanFiles.RemoveAll(file => Tools.NameToHash(file) == Tools.NameToHash(qarEntry.FilePath));
                }

                foreach (var fpkEntry in mod.ModFpkEntries)
                {
                    GameFpks.RemoveAll(fpk => Tools.NameToHash(fpk.FpkFile) == Tools.NameToHash(fpkEntry.FpkFile) && Tools.ToQarPath(fpk.FilePath) == Tools.ToQarPath(fpkEntry.FilePath));
                }
            }

            // Check mod QAR files against game files
            foreach(var s in cleanFiles)
            {
                if(game.GameQarEntries.Count(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(s)) == 0)
                {
                    game.GameQarEntries.Add(new ModQarEntry() { FilePath = Tools.ToQarPath(s), SourceType = FileSource.System, Hash = Tools.NameToHash(s) });
                }
            }

            game.GameFpkEntries = GameFpks;

            SettingsManager.SetGameData(game);

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

        private static void CleanupFolders()
        {
            if (Directory.Exists("_working")) Directory.Delete("_working", true);
            if (Directory.Exists("_extr")) Directory.Delete("_extr", true);
            if (Directory.Exists("_gamefpk")) Directory.Delete("_gamefpk", true);
            if (Directory.Exists("_modfpk")) Directory.Delete("_modfpk", true);
        }
    }
}