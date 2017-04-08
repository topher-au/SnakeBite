using GzsTool.Core.Fpk;
using GzsTool.Core.Qar;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SnakeBite
{
    internal static class ModManager
    {
        internal static string OnePath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat"; } }
        internal static string ZeroPath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\00.dat"; } }
        internal static string GameDir { get { return Properties.Settings.Default.InstallPath; } }

        // SYNC makebite
        static string ExternalDirName = "GameDir";
        internal static List<string> ignoreFileList = new List<string>(new string[] {
            "mgsvtpp.exe",
            "mgsvmgo.exe",
            "steam_api64.dll",
            "steam_appid.txt",
            "version_info.txt",
            "chunk0.dat",
            "chunk1.dat",
            "chunk2.dat",
            "chunk3.dat",
            "chunk0.dat",
            "texture0.dat",
            "texture1.dat",
            "texture2.dat",
            "texture3.dat",
            "texture4.dat",
            "00.dat",
            "01.dat",
            "snakebite.xml"
        });

        internal static List<string> ignoreExtList = new List<string>(new string[] {
            ".exe",
            ".dll",
            ".dat",
        });

        public static bool InstallMod(string ModFile,bool skipCleanup=false)
        {
            CleanupFolders(skipCleanup);

            Debug.LogLine(String.Format("[Install] Installation started: {0}", ModFile), Debug.LogLevel.Basic);

            // Extract game archive
            var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working");

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

            Debug.LogLine("[Install] Checking existing game data", Debug.LogLevel.Basic);

            // Check for FPKs in 00.dat
            foreach (string fpk in modFpks)
            {
                string datFile = zeroFiles.FirstOrDefault(file => Tools.CompareHashes(file, fpk));
                if (datFile != null)
                {
                    if (mergeFpks.Contains(Tools.ToQarPath(datFile))) continue;
                    mergeFpks.Add(fpk);

                    MergeFiles.Add(new ModQarEntry() { FilePath = fpk, SourceType = FileSource.Merged, SourceName = "00.dat" });
                }
            }

            var gameData = GzsLib.ReadBaseData();

            var oneFiles = gameData.FileList.FindAll(entry => entry.QarFile == "01.dat");
            var baseFiles = gameData.FileList.FindAll(entry => entry.QarFile != "01.dat");

            // Check for FPKs in 01.dat
            foreach (string fpk in modFpks)
            {
                GameFile file = oneFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                if (file != null)
                {
                    if (mergeFpks.Contains(Tools.ToQarPath(file.FilePath))) continue;

                    // Create destination directory
                    string destDirectory = Path.Combine("_working", Path.GetDirectoryName(Tools.ToWinPath(file.FilePath)));
                    if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);

                    // Extract file into dat directory
                    var ex = GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\0\\01.dat"), file.FileHash, Path.Combine("_working", Tools.ToWinPath(file.FilePath)));
                    mergeFpks.Add(Tools.ToQarPath(file.FilePath));

                    MergeFiles.Add(new ModQarEntry() { FilePath = file.FilePath, SourceType = FileSource.Merged, SourceName = "01.dat" });

                    if (zeroFiles.FirstOrDefault(datFile => Tools.CompareHashes(datFile, file.FilePath)) == null)
                    {
                        zeroFiles.Add(Tools.ToWinPath(file.FilePath));
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

                    if (zeroFiles.FirstOrDefault(datFile => Tools.CompareHashes(datFile, file.FilePath)) == null)
                    {
                        zeroFiles.Add(Tools.ToWinPath(file.FilePath));
                    }
                }
            }

            Debug.LogLine(String.Format("[Install] Merging {0} FPK files", MergeFiles.Count), Debug.LogLevel.Basic);

            var g = SettingsManager.GetGameData();

            // Merge FPK files
            foreach (ModQarEntry gf in MergeFiles)
            {
                Debug.LogLine(String.Format("[Install] Starting merge: {0} ({1})", gf.FilePath, gf.SourceName), Debug.LogLevel.Debug);
                // Extract game FPK
                string fpkDatPath = zeroFiles.FirstOrDefault(file => Tools.CompareHashes(file, gf.FilePath));
                string fpkPath = Path.Combine("_working", Tools.ToWinPath(fpkDatPath));
                var gameFpk = GzsLib.ExtractArchive<FpkFile>(fpkPath, "_gamefpk");

                // Extract mod FPK
                List<string> exFpk = null;
                try {
                    exFpk = GzsLib.ExtractArchive<FpkFile>(Path.Combine("_extr", Tools.ToWinPath(gf.FilePath)), "_modfpk");
                } catch (System.IO.FileNotFoundException e) {
                    Debug.LogLine(String.Format("[Install] Possible mismatch between snakebite gztool dictionary mod {0} was packed with and current snakebite gztool dictionary.", ModFile));
                    throw e;
                }

                // Add file to gamedata info
                var q = g.GameQarEntries.FirstOrDefault(entry => entry.FilePath == gf.FilePath);
                if (q == null) g.GameQarEntries.Add(new ModQarEntry() { FilePath = Tools.ToQarPath(gf.FilePath), SourceType = gf.SourceType, SourceName = gf.SourceName, Hash = Tools.NameToHash(gf.FilePath) });

                foreach (string f in gameFpk)
                {
                    var c = exFpk.FirstOrDefault(entry => Tools.CompareHashes(entry, f));
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

                    Debug.LogLine(String.Format("[Install] Copying file: {0}", fileName), Debug.LogLevel.All);

                    if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
                    File.Copy(sourceFile, destFile, true);
                    if (!gameFpk.Contains(fileName)) gameFpk.Add(Tools.ToQarPath(fileName));
                }

                // Rebuild game FPK
                GzsLib.WriteFpkArchive(fpkPath, "_gamefpk", gameFpk);
                if (!zeroFiles.Contains(Tools.ToWinPath(gf.FilePath))) zeroFiles.Add(Tools.ToWinPath(gf.FilePath));
                Directory.Delete("_modfpk", true);
                Directory.Delete("_gamefpk", true);
                Debug.LogLine(String.Format("[Install] Merge complete"), Debug.LogLevel.Debug);
            }

            Debug.LogLine("[Install] Copying game dir files", Debug.LogLevel.Basic);
            foreach (ModFileEntry fileEntry in metaData.ModFileEntries) {
                bool skipFile = false;
                foreach (string ignoreFile in ignoreFileList) {
                    if (fileEntry.FilePath.Contains(ignoreFile)) {
                        skipFile = true;
                    }
                }
                foreach (string ignoreExt in ignoreExtList) {
                    if (fileEntry.FilePath.Contains(ignoreExt)) {
                        skipFile = true;
                    }
                }

                if (skipFile == false) {
                    string sourceFile = Path.Combine("_extr", ExternalDirName, Tools.ToWinPath(fileEntry.FilePath));
                    string destFile = Path.Combine(GameDir, Tools.ToWinPath(fileEntry.FilePath));

                    Debug.LogLine(String.Format("[Install] Copying file: {0}", destFile), Debug.LogLevel.All);

                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(sourceFile, destFile, true);

                    g.GameFileEntries.Add(fileEntry);
                }
            }

            SettingsManager.SetGameData(g);

            Debug.LogLine("[Install] Copying remaining mod files", Debug.LogLevel.Basic);

            // Copy files for 01.dat, ignoring merged FPKs
            foreach (ModQarEntry modEntry in metaData.ModQarEntries)
            {
                if (!zeroFiles.Contains(Tools.ToWinPath(modEntry.FilePath))) zeroFiles.Add(Tools.ToWinPath(modEntry.FilePath));

                if (modEntry.FilePath.Contains(".fpk"))
                    if (mergeFpks.Count(fpk => Tools.CompareHashes(fpk, modEntry.FilePath)) > 0)
                        continue;

                string sourceFile = Path.Combine("_extr", Tools.ToWinPath(modEntry.FilePath));
                string destFile = Path.Combine("_working", Tools.ToWinPath(modEntry.FilePath));
                string destDir = Path.GetDirectoryName(destFile);

                Debug.LogLine(String.Format("[Install] Copying file: {0}", modEntry.FilePath), Debug.LogLevel.All);
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                File.Copy(sourceFile, destFile, true);
            }

            // Rebuild 00.dat
            Debug.LogLine("[Install] Rebuilding game archive", Debug.LogLevel.Basic);
            GzsLib.WriteQarArchive(ZeroPath, "_working", zeroFiles, 3150048);

            SettingsManager.UpdateDatHash();

            SettingsManager.AddMod(metaData);

            if (!skipCleanup) {
            CleanupDatabase();
            }

            if (!skipCleanup) {
                CleanupFolders(skipCleanup);
            }

            Debug.LogLine("[Install] Installation finished", Debug.LogLevel.Basic);

            return true;
        }

        public static bool UninstallMod(ModEntry mod)
        {
            Debug.LogLine(String.Format("[Uninstall] Uninstall started: {0}", mod.Name), Debug.LogLevel.Basic);

            //allready logs
            CleanupFolders();

            Debug.LogLine("[Uninstall] Extracting 00.dat to _working", Debug.LogLevel.Basic);
            var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working");

            Debug.LogLine("[Uninstall] Building list of fpks in mod", Debug.LogLevel.Basic);
            List<string> modFpks = new List<string>();
            foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
            {
                if (!modFpks.Contains(fpkEntry.FpkFile)) modFpks.Add(fpkEntry.FpkFile);
            }
            //tex: the above wont catch empty fpks (fpkds require a fpk, which can be empty)
            foreach (ModQarEntry fpkEntry in mod.ModQarEntries) {
                if (fpkEntry.FilePath.Contains(".fpk")) {
                    if (!modFpks.Contains(fpkEntry.FilePath)) modFpks.Add(fpkEntry.FilePath);
                }
            }

            Debug.LogLine("[Uninstall] Reading snakebite.xml", Debug.LogLevel.Basic);
            var gameData = SettingsManager.GetGameData();

            Debug.LogLine("[Uninstall] Removing game dir file entries", Debug.LogLevel.Basic);
            List<string> fileEntryDirs = new List<string>();
            foreach (ModFileEntry fileEntry in mod.ModFileEntries) {
                bool skipFile = false;
                foreach (string ignoreFile in ignoreFileList) {
                    if (fileEntry.FilePath.Contains(ignoreFile)) {
                        skipFile = true;
                    }
                }
                foreach (string ignoreExt in ignoreExtList) {
                    if (fileEntry.FilePath.Contains(ignoreExt)) {
                        skipFile = true;
                    }
                }
                if (skipFile == false) {
                    //tex TODO hash check?
                    string destFile = Path.Combine(GameDir, Tools.ToWinPath(fileEntry.FilePath));
                    string dir = Path.GetDirectoryName(destFile);
                    if (!fileEntryDirs.Contains(dir)) {
                        fileEntryDirs.Add(dir);
                    }
                    if (File.Exists(destFile)) {
                        Debug.LogLine(String.Format("[Uninstall] deleting file: {0}", destFile), Debug.LogLevel.All);
                        try {
                            File.Delete(destFile);
                        } catch (IOException e) {
                            Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                        }
                    }
                }
                foreach (string dir in fileEntryDirs) {
                    if (Directory.Exists(dir) && Directory.GetFiles(dir).Length==0) {
                        Debug.LogLine(String.Format("[Uninstall] deleting folder: {0}", dir), Debug.LogLevel.All);
                        try {
                            Directory.Delete(dir, true);
                        } catch (IOException e) {
                            Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                        }
                    }
                }
                gameData.GameFileEntries.RemoveAll(file => Tools.CompareHashes(file.FilePath, fileEntry.FilePath));
            }
            foreach (string dir in fileEntryDirs) {

            }

            Debug.LogLine("[Uninstall] Processing fpk entries", Debug.LogLevel.Basic);
            foreach (string fpk in modFpks)
            {
                Debug.LogLine(String.Format("[Uninstall] Processing {0}", fpk), Debug.LogLevel.Basic);
                //Extract fpk/d
                string fpkName = Path.GetFileName(fpk);
                string fpkDatPath = zeroFiles.FirstOrDefault(file => Tools.CompareHashes(file, fpk));//NMC internal path
                if (fpkDatPath == null) continue;
                List<string> fpkFiles = GzsLib.ExtractArchive<FpkFile>(Path.Combine("_working", Tools.ToWinPath(fpkDatPath)), "_modfpk");

                // Remove all mod fpk files from fpkFiles
                foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
                {
                    //tex OFF Debug.LogLine(String.Format("[Uninstall] Removing {1}\\{0}", Tools.ToWinPath(fpkEntry.FilePath), fpkName), Debug.LogLevel.Debug);
                    fpkFiles.RemoveAll(file => Tools.ToQarPath(file) == Tools.ToQarPath(fpkEntry.FilePath));
                }

                var gameFpks = gameData.GameFpkEntries.ToList();

                // Remove all merged files from fpkFiles and gameData list
                foreach (ModFpkEntry gameFpkFile in gameFpks)
                {
                    if (Tools.ToQarPath(gameFpkFile.FpkFile) == Tools.ToQarPath(fpk) && gameFpkFile.SourceType == FileSource.Merged)
                    {
                        // OFF Debug.LogLine(String.Format("[Uninstall] Removing merged file {0}", gameFpkFile.FilePath));
                        fpkFiles.RemoveAll(entry => entry == gameFpkFile.FilePath);
                        gameData.GameFpkEntries.Remove(gameFpkFile);
                    }
                }

                // remove fpk if no files left
                if (fpkFiles.Count == 0)
                {
                    Debug.LogLine(String.Format("[Uninstall] {0} now has no modded files, removing from list", fpkName), Debug.LogLevel.Debug);
                    zeroFiles.RemoveAll(file => Tools.CompareHashes(file, fpk));
                    gameData.GameQarEntries.RemoveAll(file => Tools.CompareHashes(file.FilePath, fpk));
                }
                else
                {
                    Debug.LogLine(String.Format("[Uninstall] Rebuilding {0}", fpk), Debug.LogLevel.Debug);
                    // rebuild fpk from base file
                    var baseData = GzsLib.ReadBaseData();

                    var oneFiles = baseData.FileList.FindAll(entry => entry.QarFile == "01.dat");
                    var baseFiles = baseData.FileList.FindAll(entry => entry.QarFile != "01.dat");

                    // Check for FPKs in 00.dat first
                    GameFile file = oneFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                    if (file != null)
                    {
                        // Extract base FPK files
                        GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\0\\01.dat"), file.FileHash, "_working\\temp.fpk");
                        var gameFpk = GzsLib.ExtractArchive<FpkFile>("_working\\temp.fpk", "_gamefpk");

                        // Add merged base files to game file database
                        var mCount = 0;
                        foreach (var fpkF in gameFpk)
                        {
                            if (!fpkFiles.Contains(fpkF))
                            {
                                gameData.GameFpkEntries.Add(new ModFpkEntry() { FpkFile = fpk, FilePath = fpkF, SourceType = FileSource.Merged, SourceName = file.QarFile });
                                mCount++;
                            }
                        }

                        Debug.LogLine(String.Format("[Uninstall] {0} files restored from {1}", mCount, file.QarFile), Debug.LogLevel.Debug);

                        // Copy remaining files over base FPK
                        foreach (string mFile in fpkFiles)
                        {
                            string fDir = Path.GetDirectoryName(mFile);
                            if (!Directory.Exists(Path.Combine("_gamefpk", fDir))) Directory.CreateDirectory(Path.Combine("_gamefpk", fDir));
                            Debug.LogLine(String.Format("[Uninstall] Merging existing file: {0}", mFile));
                            File.Copy(Path.Combine("_modfpk", mFile), Path.Combine(Path.Combine("_gamefpk", mFile)), true);
                            if (!gameFpk.Contains(mFile)) gameFpk.Add(mFile);
                        }

                        // Rebuild FPK
                        GzsLib.WriteFpkArchive(Path.Combine("_working", Tools.ToWinPath(fpkDatPath)), "_gamefpk", gameFpk);
                        Directory.Delete("_gamefpk", true);
                        Directory.Delete("_modfpk", true);
                        continue; // don't check base data if it's in 01
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
                            if (!fpkFiles.Contains(fpkF))
                            {
                                gameData.GameFpkEntries.Add(new ModFpkEntry() { FpkFile = fpk, FilePath = fpkF, SourceType = FileSource.Merged, SourceName = file.QarFile });
                                mCount++;
                            }
                        }

                        Debug.LogLine(String.Format("[Uninstall] {0} files restored from {1}", mCount, file.QarFile), Debug.LogLevel.Debug);
                        // Copy remaining files over base FPK
                        foreach (string mFile in fpkFiles)
                        {
                            string fDir = Path.GetDirectoryName(mFile);
                            if (!Directory.Exists(Path.Combine("_gamefpk", fDir))) Directory.CreateDirectory(Path.Combine("_gamefpk", fDir));
                            Debug.LogLine(String.Format("[Uninstall] Merging existing file: {0}", mFile));
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

            //write out snakebite.xml, at this point the mods qar and fpk entries should have been removed, but the mods modentry is still there
            Debug.LogLine("[Uninstall] Saving snakebite.xml", Debug.LogLevel.Debug);
            SettingsManager.SetGameData(gameData);

            Debug.LogLine("[Uninstall] Remove all mod files from 00.dat files list", Debug.LogLevel.Debug);
            foreach (ModQarEntry qarEntry in mod.ModQarEntries)
            {
                string fExt = Path.GetExtension(qarEntry.FilePath);
                if (!fExt.Contains(".fpk"))
                {
                    zeroFiles.RemoveAll(file => Tools.CompareHashes(file, qarEntry.FilePath));
                }
            }

            Debug.LogLine("[Uninstall] Rebuilding 00.dat", Debug.LogLevel.Basic);
            GzsLib.WriteQarArchive(ZeroPath, "_working", zeroFiles, 3150048);

            Debug.LogLine("[Uninstall] Updating 00.dat hash", Debug.LogLevel.Basic);
            SettingsManager.UpdateDatHash();

            Debug.LogLine("[Uninstall] Removing mod entry from snakebite.xml", Debug.LogLevel.Basic);
            SettingsManager.RemoveMod(mod);

            CleanupDatabase();

            //Debug.LogLine("CleanupFolders", Debug.LogLevel.Basic); //allready logs
            CleanupFolders();
            Debug.LogLine("[Uninstall] Uninstall complete", Debug.LogLevel.Basic);

            return true;
        }

        public static void MergeAndCleanup()
        {
            MoveDatFiles();
            CleanupDatabase();
        }

        public static void MoveDatFiles()
        {
            Debug.LogLine("[DatMerge] System data merge started", Debug.LogLevel.Debug);

            List<string> zeroList = new List<string>();

            try {
                zeroList = GzsLib.ListArchiveContents<QarFile>(ZeroPath);
            }
            catch (Exception e) {
                Debug.LogLine(String.Format("[Error] GzsLib.ListArchiveContents exception: {0}",e.Message), Debug.LogLevel.Debug);
                throw e;
            }
            var modQarFiles = SettingsManager.GetModQarFiles();

            int moveCount = 0;
            foreach (string zeroFile in zeroList)
            {
                if (zeroFile == "foxpatch.dat") continue;
                if (modQarFiles.Contains(Tools.ToQarPath(zeroFile))) continue;
                moveCount++;
            }

            if (moveCount > 0)
            {
                CleanupFolders();

                Debug.LogLine("[DatMerge] Extracting 00.dat", Debug.LogLevel.Debug);
                // Extract 00.dat
                var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_extr");
                Debug.LogLine("[DatMerge] Extracting 01.dat", Debug.LogLevel.Debug);
                // Extract 01.dat
                var oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_working");

                var zeroOut = zeroFiles.ToList();

                Debug.LogLine(string.Format("[DatMerge] Moving {0} system files", moveCount), Debug.LogLevel.Debug);
                // Move files from 00 to 01 (excluding foxpatch.dat)
                foreach (string zeroFile in zeroFiles)
                {
                    if (zeroFile == "foxpatch.dat") continue;
                    if (modQarFiles.Contains(Tools.ToQarPath(zeroFile))) continue;
                    string sourceName = Path.Combine("_extr", Tools.ToWinPath(zeroFile));
                    string destName = Path.Combine("_working", Tools.ToWinPath(zeroFile));
                    string destFolder = Path.GetDirectoryName(destName);

                    Debug.LogLine(string.Format("[DatMerge] Moving system file {0}", zeroFile));
                    if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                    File.Move(sourceName, destName);

                    zeroOut.Remove(zeroFile);
                    oneFiles.Add(zeroFile);
                }

                Debug.LogLine("[DatMerge] Rebuilding game archives", Debug.LogLevel.Debug);

                // Rebuild 01.dat with files
                GzsLib.WriteQarArchive(OnePath, "_working", oneFiles, 3150048);

                // Rebuild 00.dat
                GzsLib.WriteQarArchive(ZeroPath, "_extr", zeroOut, 3150304);

                SettingsManager.UpdateDatHash();

                CleanupFolders();

                Debug.LogLine("[DatMerge] Merge finished", Debug.LogLevel.Debug);
            }
            else
            {
                Debug.LogLine("[DatMerge] No files to merge, aborting", Debug.LogLevel.Debug);
            }
        }

        public static void CleanupDatabase()
        {
            Debug.LogLine("[Cleanup] Database cleanup started", Debug.LogLevel.Basic);

            // Retrieve installation data
            var mods = SettingsManager.GetInstalledMods();
            var game = SettingsManager.GetGameData();
            var zeroFiles = GzsLib.ListArchiveContents<QarFile>(ZeroPath);

            Debug.LogLine("[Cleanup] Removing duplicate file entries", Debug.LogLevel.Debug);
            // Remove duplicate file entries
            var cleanFiles = zeroFiles.ToList();
            foreach (string file in zeroFiles)
            {
                while (cleanFiles.Count(entry => entry == file) > 1)
                {
                    cleanFiles.Remove(file);
                    Debug.LogLine(String.Format("[Cleanup] Found duplicate file in 00.dat: {0}", file), Debug.LogLevel.Debug);
                }
            }

            Debug.LogLine("[Cleanup] Examining FPK archives", Debug.LogLevel.Debug);
            var GameFpks = game.GameFpkEntries.ToList(); ;
            // Search for FPKs in game data
            var fpkFiles = cleanFiles.FindAll(entry => entry.Contains(".fpk"));
            foreach (string fpkFile in fpkFiles)
            {
                string fpkName = Path.GetFileName(fpkFile);
                // Extract FPK from archive
                Debug.LogLine(String.Format("[Cleanup] Examining {0}", fpkName));
                GzsLib.ExtractFile<QarFile>(ZeroPath, fpkFile, fpkName);

                // Read FPK contents
                var fpkContent = GzsLib.ListArchiveContents<FpkFile>(fpkName);

                // Add contents to game FPK list
                foreach (var c in fpkContent)
                {
                    if (GameFpks.Count(entry => Tools.CompareNames(entry.FilePath, c) && Tools.CompareHashes(entry.FpkFile, fpkFile)) == 0)
                    {
                        GameFpks.Add(new ModFpkEntry() { FpkFile = fpkFile, FilePath = c });
                    }
                }

                File.Delete(fpkName);
            }

            Debug.LogLine("[Cleanup] Checking installed mods", Debug.LogLevel.Debug);
            Debug.LogLine("[Cleanup] Removing all installed mod data from game data list", Debug.LogLevel.Debug);
            foreach (var mod in mods)
            {
                foreach (var qarEntry in mod.ModQarEntries)
                {
                    cleanFiles.RemoveAll(file => Tools.CompareHashes(file, qarEntry.FilePath));
                }

                foreach (var fpkEntry in mod.ModFpkEntries)
                {
                    GameFpks.RemoveAll(fpk => Tools.CompareHashes(fpk.FpkFile, fpkEntry.FpkFile) && Tools.ToQarPath(fpk.FilePath) == Tools.ToQarPath(fpkEntry.FilePath));
                }
            }

            Debug.LogLine("[Cleanup] Checking mod QAR files against game files", Debug.LogLevel.Debug);
            foreach (var s in cleanFiles)
            {
                if (game.GameQarEntries.Count(entry => Tools.CompareHashes(entry.FilePath, s)) == 0)
                {
                    game.GameQarEntries.Add(new ModQarEntry() { FilePath = Tools.ToQarPath(s), SourceType = FileSource.System, Hash = Tools.NameToHash(s) });
                }
            }

            game.GameFpkEntries = GameFpks;

            SettingsManager.SetGameData(game);
        }

        internal static Version GetMGSVersion()
        {
            // Get MGSV executable version
            var versionInfo = FileVersionInfo.GetVersionInfo(Properties.Settings.Default.InstallPath + "\\mgsvtpp.exe");
            return new Version(versionInfo.ProductVersion);
        }

        internal static Version GetSBVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }

        private static void CleanupFolders(bool skipCleanup=false)
        {
            if (Directory.Exists("_working")) Directory.Delete("_working", true);
            if (Directory.Exists("_extr")) Directory.Delete("_extr", true);
            if (Directory.Exists("_gamefpk")) Directory.Delete("_gamefpk", true);
            if (Directory.Exists("_modfpk")) Directory.Delete("_modfpk", true);
            Debug.LogLine("[Mod] Cleaning up snakebite work folders.");

            bool directoryExists = true;
            while (directoryExists) {
                Thread.Sleep(100);
                directoryExists = false;
                if (Directory.Exists("_working")) directoryExists = true;
                if (Directory.Exists("_extr")) directoryExists = true;
                if (Directory.Exists("_gamefpk")) directoryExists = true;
                if (Directory.Exists("_modfpk")) directoryExists = true;
            }
        }

        public static bool CheckConflicts(string ModFile, bool ignoreConflicts = false)
        {
            var metaData = Tools.ReadMetaData(ModFile);
            if (metaData == null) return false;

            if (!SettingsManager.DisableConflictCheck && !ignoreConflicts)
            {
                // check version conflicts
                var SBVersion = ModManager.GetSBVersion();
                var MGSVersion = ModManager.GetMGSVersion();

                Version modSBVersion = new Version();
                Version modMGSVersion = new Version();
                try
                {
                    modSBVersion = metaData.SBVersion.AsVersion();
                    modMGSVersion = metaData.MGSVersion.AsVersion();
                }
                catch
                {
                    MessageBox.Show(String.Format("The selected version of {0} was created with an older version of SnakeBite and is no longer compatible, please download the latest version and try again.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }


                // Check if mod requires SB update
                if (modSBVersion > SBVersion)
                {
                    MessageBox.Show(String.Format("{0} requires SnakeBite version {1} or newer. Please follow the link on the Settings page to get the latest version.", metaData.Name,metaData.Version), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (modSBVersion < new Version(0, 8, 0, 0)) // 0.8.0.0
                {
                    MessageBox.Show(String.Format("The selected version of {0} was created with an older version of SnakeBite and is no longer compatible, please download the latest version and try again.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Check MGS version compatibility
                if (MGSVersion != modMGSVersion && modMGSVersion != new Version(0, 0, 0, 0))
                {
                    if (MGSVersion > modMGSVersion && modMGSVersion > new Version(0, 0, 0, 0))
                    {
                        var contInstall = MessageBox.Show(String.Format("{0} appears to be for an older version of MGSV. It is recommended that you at least check for an updated version before installing.\n\nContinue installation?", metaData.Name, modMGSVersion, MGSVersion), "Game version mismatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (contInstall == DialogResult.No) return false;
                    }
                    if (MGSVersion < modMGSVersion)
                    {
                        MessageBox.Show(String.Format("{0} requires MGSV version {1}, but your installation is version {2}. Please update MGSV and try again.", metaData.Name, modMGSVersion, MGSVersion), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                Debug.LogLine(String.Format("[Mod] Checking conflicts for {0}", metaData.Name));
                int confCounter = 0;
                // search installed mods for conflicts
                var mods = SettingsManager.GetInstalledMods();
                List<string> conflictingMods = new List<string>();
                int confIndex = -1;
                foreach (ModEntry mod in mods) // iterate through installed mods
                {
                    foreach (ModFileEntry fileEntry in metaData.ModFileEntries) // iterate external files from new mod
                    {
                        ModFileEntry conflicts = mod.ModFileEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FilePath, fileEntry.FilePath));
                        if (conflicts != null) {
                            if (confIndex == -1) confIndex = mods.IndexOf(mod);
                            if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                            Debug.LogLine(String.Format("[{0}] Conflict in 00.dat: {1}", mod.Name, conflicts.FilePath));
                            confCounter++;
                        }
                    }

                    foreach (ModQarEntry qarEntry in metaData.ModQarEntries) // iterate qar files from new mod
                    {
                        if (qarEntry.FilePath.Contains(".fpk")) continue;
                        ModQarEntry conflicts = mod.ModQarEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FilePath, qarEntry.FilePath));
                        if (conflicts != null)
                        {
                            if (confIndex == -1) confIndex = mods.IndexOf(mod);
                            if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                            Debug.LogLine(String.Format("[{0}] Conflict in 00.dat: {1}", mod.Name, conflicts.FilePath));
                            confCounter++;
                        }
                    }

                    foreach (ModFpkEntry fpkEntry in metaData.ModFpkEntries) // iterate fpk files from new mod
                    {
                        ModFpkEntry conflicts = mod.ModFpkEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FpkFile, fpkEntry.FpkFile) &&
                                                                                               Tools.CompareHashes(entry.FilePath, fpkEntry.FilePath));
                        if (conflicts != null)
                        {
                            if (confIndex == -1) confIndex = mods.IndexOf(mod);
                            if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                            Debug.LogLine(String.Format("[{0}] Conflict in {2}: {1}", mod.Name, conflicts.FilePath, Path.GetFileName(conflicts.FpkFile)));
                            confCounter++;
                        }
                    }
                }

                // if the mod conflicts, display message

                if (conflictingMods.Count > 0)
                {
                    Debug.LogLine(String.Format("[Mod] Found {0} conflicts", confCounter));
                    string msgboxtext = "The selected mod conflicts with these mods:\n";
                    foreach (string Conflict in conflictingMods)
                    {
                        msgboxtext += Conflict + "\n";
                    }
                    msgboxtext += "\nMore information regarding the conflicts has been output to the logfile. Double click the version number shown in the Launcher to view the current logfile.";
                    MessageBox.Show(msgboxtext, "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                Debug.LogLine("[Mod] No conflicts found");

                bool sysConflict = false;
                // check for system file conflicts
                var gameData = SettingsManager.GetGameData();
                foreach (ModQarEntry gameQarFile in gameData.GameQarEntries.FindAll(entry => entry.SourceType == FileSource.System))
                {
                    if (metaData.ModQarEntries.Count(entry => Tools.ToQarPath(entry.FilePath) == Tools.ToQarPath(gameQarFile.FilePath)) > 0) sysConflict = true;
                }

                foreach (ModFpkEntry gameFpkFile in gameData.GameFpkEntries.FindAll(entry => entry.SourceType == FileSource.System))
                {
                    if (metaData.ModFpkEntries.Count(entry => entry.FilePath == gameFpkFile.FilePath && entry.FpkFile == gameFpkFile.FpkFile) > 0) sysConflict = true;
                }
                if (sysConflict)
                {
                    //tex TODO: figure out what it's actually checking and how this can be corrupted
                    string msgboxtext = "The selected mod conflicts with existing MGSV system files,\n";
                    msgboxtext += "or the snakebite.xml base entries has become corrupt.\n";
                    msgboxtext += "Please use the Restore Original Game Files option in Snakebite settings and re-run snakebite\n";
                    MessageBox.Show(msgboxtext, "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                DialogResult confirmInstall = MessageBox.Show(String.Format("You are about to install {0}, continue?", metaData.Name), "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmInstall == DialogResult.No) return false;
            }
            return true;
        }
    }
}