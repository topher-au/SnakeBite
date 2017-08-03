using GzsTool.Core.Fpk;
using GzsTool.Core.Qar;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.Forms;
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
        internal static string t7Path { get { return Properties.Settings.Default.InstallPath + "\\master\\a_texture7.dat"; } }
        internal static string c7Path { get { return Properties.Settings.Default.InstallPath + "\\master\\a_chunk7.dat"; } }
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
            ".dat",
        });

        public static bool InstallMod(List<string> ModFiles, bool skipCleanup = false) // Installs a list of mod filenames
        {
            CleanupFolders(skipCleanup);
            // Extract game archive
            var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working1");
            
            List<string> oneFilesList = null;
            bool hasFtexs = foundLooseFtexs(ModFiles);
            if (hasFtexs)
            {
                oneFilesList = GzsLib.ExtractArchive<QarFile>(OnePath, "_working2");
            }

            SettingsManager manager = new SettingsManager(GameDir);
            // end of extraction

            foreach (string ModFile in ModFiles)
            {
                Debug.LogLine(String.Format("[Install] Installation started: {0}", ModFile), Debug.LogLevel.Basic); 
                
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
                        string destDirectory = Path.Combine("_working1", Path.GetDirectoryName(Tools.ToWinPath(file.FilePath)));
                        if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);

                        // Extract file into dat directory
                        var ex = GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\0\\01.dat"), file.FileHash, Path.Combine("_working1", Tools.ToWinPath(file.FilePath)));
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
                        string destDirectory = Path.Combine("_working1", Path.GetDirectoryName(Tools.ToWinPath(file.FilePath)));
                        if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);

                        // Extract file into dat directory
                        var ex = GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\" + file.QarFile), file.FileHash, Path.Combine("_working1", Tools.ToWinPath(file.FilePath)));
                        mergeFpks.Add(Tools.ToQarPath(file.FilePath));
                        MergeFiles.Add(new ModQarEntry() { FilePath = file.FilePath, SourceType = FileSource.Merged, SourceName = file.QarFile });

                        if (zeroFiles.FirstOrDefault(datFile => Tools.CompareHashes(datFile, file.FilePath)) == null)
                        {
                            zeroFiles.Add(Tools.ToWinPath(file.FilePath));
                        }
                    }
                }

                Debug.LogLine(String.Format("[Install] Merging {0} FPK files", MergeFiles.Count), Debug.LogLevel.Basic);

                var g = manager.GetGameData();

                // Merge FPK files
                foreach (ModQarEntry gf in MergeFiles)
                {
                    Debug.LogLine(String.Format("[Install] Starting merge: {0} ({1})", gf.FilePath, gf.SourceName), Debug.LogLevel.Debug);
                    // Extract game FPK
                    string fpkDatPath = zeroFiles.FirstOrDefault(file => Tools.CompareHashes(file, gf.FilePath));
                    string fpkPath = Path.Combine("_working1", Tools.ToWinPath(fpkDatPath));
                    var gameFpk = GzsLib.ExtractArchive<FpkFile>(fpkPath, "_gamefpk");

                    // Extract mod FPK
                    List<string> exFpk = null;
                    try
                    {
                        exFpk = GzsLib.ExtractArchive<FpkFile>(Path.Combine("_extr", Tools.ToWinPath(gf.FilePath)), "_modfpk");
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
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
                    try
                    {
                        Directory.Delete("_modfpk", true);
                        Directory.Delete("_gamefpk", true);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                    }
                    Debug.LogLine(String.Format("[Install] Merge complete"), Debug.LogLevel.Debug);
                }
                //move external files to game directory
                Debug.LogLine("[Install] Copying game dir files", Debug.LogLevel.Basic);
                foreach (ModFileEntry fileEntry in metaData.ModFileEntries)
                {
                    bool skipFile = false;
                    foreach (string ignoreFile in ignoreFileList)
                    {
                        if (fileEntry.FilePath.Contains(ignoreFile))
                        {
                            skipFile = true;
                        }
                    }
                    foreach (string ignoreExt in ignoreExtList)
                    {
                        if (fileEntry.FilePath.Contains(ignoreExt))
                        {
                            skipFile = true;
                        }
                    }

                    if (skipFile == false)
                    {
                        string sourceFile = Path.Combine("_extr", ExternalDirName, Tools.ToWinPath(fileEntry.FilePath));
                        string destFile = Path.Combine(GameDir, Tools.ToWinPath(fileEntry.FilePath));

                        Debug.LogLine(String.Format("[Install] Copying file: {0}", destFile), Debug.LogLevel.All);

                        Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                        File.Copy(sourceFile, destFile, true);

                        g.GameFileEntries.Add(fileEntry);
                    }
                }

                manager.SetGameData(g);
                
                // copy loose texture files to 01.dat
                Debug.LogLine("[Install] Copying loose textures to 01.", Debug.LogLevel.Basic);
                foreach (ModQarEntry modEntry in metaData.ModQarEntries)
                {
                    if (modEntry.FilePath.Contains(".ftex"))
                    {
                        if (!oneFilesList.Contains(Tools.ToWinPath(modEntry.FilePath)))
                            oneFilesList.Add(Tools.ToWinPath(modEntry.FilePath));

                        string sourceFile = Path.Combine("_extr", Tools.ToWinPath(modEntry.FilePath));
                        string destFile = Path.Combine("_working2", Tools.ToWinPath(modEntry.FilePath));
                        string destDir = Path.GetDirectoryName(destFile);

                        Debug.LogLine(String.Format("[Install] Copying texture file: {0}", modEntry.FilePath), Debug.LogLevel.All);
                        if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                        File.Copy(sourceFile, destFile, true);
                    }
                }

                // Copy (non-texture) files for 01.dat, ignoring merged FPKs
                Debug.LogLine("[Install] Copying remaining mod files", Debug.LogLevel.Basic);
                foreach (ModQarEntry modEntry in metaData.ModQarEntries)
                {
                    if (!modEntry.FilePath.Contains(".ftex"))
                    {
                        if (!zeroFiles.Contains(Tools.ToWinPath(modEntry.FilePath))) zeroFiles.Add(Tools.ToWinPath(modEntry.FilePath));

                        if (modEntry.FilePath.Contains(".fpk"))
                            if (mergeFpks.Count(fpk => Tools.CompareHashes(fpk, modEntry.FilePath)) > 0)
                                continue;

                        string sourceFile = Path.Combine("_extr", Tools.ToWinPath(modEntry.FilePath));
                        string destFile = Path.Combine("_working1", Tools.ToWinPath(modEntry.FilePath));
                        string destDir = Path.GetDirectoryName(destFile);

                        Debug.LogLine(String.Format("[Install] Copying file: {0}", modEntry.FilePath), Debug.LogLevel.All);
                        if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                        File.Copy(sourceFile, destFile, true);
                    }
                }
                metaData = new ModEntry("_extr\\metadata.xml");
                manager.AddMod(metaData);
            }
            // Rebuild 00.dat
            Debug.LogLine("[Install] Rebuilding 00.dat", Debug.LogLevel.Basic);
            GzsLib.WriteQarArchive(ZeroPath, "_working1", zeroFiles, 3150048);

            if (hasFtexs)
            {
                Debug.LogLine("[Install] Rebuilding 01.dat", Debug.LogLevel.Basic);
                GzsLib.WriteQarArchive(OnePath, "_working2", oneFilesList, 3150048);
            }

            if (!skipCleanup) {
            CleanupDatabase();
            }

            if (!skipCleanup) {
                CleanupFolders(skipCleanup);
            }

            manager.UpdateDatHash();
            Debug.LogLine("[Install] Installation finished", Debug.LogLevel.Basic);

            return true;
        }

        public static bool UninstallMod(CheckedListBox.CheckedIndexCollection modIndices) // Uninstalls mods based on their indices in the list
        {
            SettingsManager manager = new SettingsManager(GameDir);
            List<ModEntry> mods = manager.GetInstalledMods();

            //allready logs
            CleanupFolders();

            Debug.LogLine("[Uninstall] Extracting 00.dat to _working1", Debug.LogLevel.Basic);
            var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working1"); // extracts 00.dat and creates a list of filenames, which is pruned throughout the uninstall process and repacked at the end.
            List<string> oneFilesList = null;
            bool hasFtexs = foundLooseFtexs(modIndices);
            
            if (hasFtexs)
            {
                oneFilesList = GzsLib.ExtractArchive<QarFile>(OnePath, "_working2"); // if necessary, extracts 01.dat and creates a list of filenames similar to zeroFiles. only textures are pruned from the list.
            }
            //end of qar extraction

            foreach (int index in modIndices)
            {
                ModEntry mod = mods[index];

                Debug.LogLine(String.Format("[Uninstall] Uninstall started: {0}", mod.Name), Debug.LogLevel.Basic);
                Debug.LogLine("[Uninstall] Building list of fpks in mod", Debug.LogLevel.Basic);
                List<string> modFpks = new List<string>();
                foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
                {
                    if (!modFpks.Contains(fpkEntry.FpkFile)) modFpks.Add(fpkEntry.FpkFile);
                }
                //tex: the above wont catch empty fpks (fpkds require a fpk, which can be empty)
                foreach (ModQarEntry fpkEntry in mod.ModQarEntries)
                {
                    if (fpkEntry.FilePath.Contains(".fpk"))
                    {
                        if (!modFpks.Contains(fpkEntry.FilePath)) modFpks.Add(fpkEntry.FilePath);//modfkps now has every fpk file and filepath for the current mod
                    }
                }

                Debug.LogLine("[Uninstall] Reading snakebite.xml", Debug.LogLevel.Basic);
                GameData gameData = manager.GetGameData(); //retrieves snakebite.xml information for lists of current installed

                Debug.LogLine("[Uninstall] Removing game dir file entries", Debug.LogLevel.Basic);
                List<string> fileEntryDirs = new List<string>();
                foreach (ModFileEntry fileEntry in mod.ModFileEntries) //checks all of current mod's files
                {
                    bool skipFile = false;
                    foreach (string ignoreFile in ignoreFileList) //marks files that shouldn't be added to the uninstallation list
                    {
                        if (fileEntry.FilePath.Contains(ignoreFile))
                        {
                            skipFile = true;
                        }
                    }
                    foreach (string ignoreExt in ignoreExtList)
                    {
                        if (fileEntry.FilePath.Contains(ignoreExt))
                        {
                            skipFile = true;
                        }
                    }
                    if (skipFile == false) //if it hasn't been flagged to be skipped:
                    {
                        //tex TODO hash check?
                        string destFile = Path.Combine(GameDir, Tools.ToWinPath(fileEntry.FilePath)); //create the filepath to the file in question
                        string dir = Path.GetDirectoryName(destFile); //filepath of the directory containing the file
                        if (!fileEntryDirs.Contains(dir))
                        {
                            fileEntryDirs.Add(dir); //the directory is added to the list of fileentrydirectories
                        }
                        if (File.Exists(destFile)) // attempt to delete the file in question
                        {
                            Debug.LogLine(String.Format("[Uninstall] deleting file: {0}", destFile), Debug.LogLevel.All);
                            try
                            {
                                File.Delete(destFile); // deletes the specified file
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                            }
                        }
                    }
                    foreach (string dir in fileEntryDirs) //all the directories that have had files deleted within them
                    {
                        if (Directory.Exists(dir) && Directory.GetFiles(dir).Length == 0) // if the directory has not yet been deleted and there are no more files inside the directory
                        {
                            Debug.LogLine(String.Format("[Uninstall] deleting folder: {0}", dir), Debug.LogLevel.All);
                            try
                            {
                                Directory.Delete(dir, true); //attempt to delete the empty directory
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                            }
                        }
                    }
                    gameData.GameFileEntries.RemoveAll(file => Tools.CompareHashes(file.FilePath, fileEntry.FilePath)); //remove all mentions of the destFile from snakebite.xml
                }

                Debug.LogLine(String.Format("[Uninstall] Removing any loose textures in {0}", mod.Name), Debug.LogLevel.Basic); // begin loose texture check for current mod.
                fileEntryDirs = new List<string>();
                foreach (ModQarEntry qarEntry in mod.ModQarEntries) // check all qar entries in current mod
                {
                    if(qarEntry.FilePath.Contains(".ftex")) { // if the file is an ftex or ftexs
                        string destFile = Path.Combine("_working2", qarEntry.FilePath);
                        if (File.Exists(destFile)) // check if the file exists in the extracted 01
                        {
                            try
                            {
                                File.Delete(destFile); // deletes the specified file
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                            }
                        }
                        gameData.GameQarEntries.RemoveAll(file => Tools.CompareHashes(file.FilePath, qarEntry.FilePath)); //remove all mentions of the deleted texture from snakebite.xml
                        oneFilesList.RemoveAll(file => Tools.CompareHashes(file, qarEntry.FilePath)); // removes all mentions of deleted texture from 01.dat's repack list
                    }
                }

                    Debug.LogLine("[Uninstall] Processing fpk entries", Debug.LogLevel.Basic);
                foreach (string fpk in modFpks)
                {
                    Debug.LogLine(String.Format("[Uninstall] Processing {0}", fpk), Debug.LogLevel.Basic);
                    //Extract fpk/d
                    string fpkName = Path.GetFileName(fpk);
                    string fpkDatPath = zeroFiles.FirstOrDefault(file => Tools.CompareHashes(file, fpk));//NMC internal path
                    if (fpkDatPath == null) continue;
                    List<string> fpkFiles = GzsLib.ExtractArchive<FpkFile>(Path.Combine("_working1", Tools.ToWinPath(fpkDatPath)), "_modfpk");

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
                            GzsLib.WriteFpkArchive(Path.Combine("_working1", Tools.ToWinPath(fpkDatPath)), "_gamefpk", gameFpk);
                            try
                            {
                                Directory.Delete("_modfpk", true);
                                Directory.Delete("_gamefpk", true);
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                            }
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
                            GzsLib.WriteFpkArchive(Path.Combine("_working1", Tools.ToWinPath(fpk)), "_gamefpk", gameFpk);
                            try
                            {
                                Directory.Delete("_modfpk", true);
                                Directory.Delete("_gamefpk", true);
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                            }
                        }
                    }
                }

                //write out snakebite.xml, at this point the mods qar and fpk entries should have been removed, but the mods modentry is still there
                Debug.LogLine("[Uninstall] Saving snakebite.xml", Debug.LogLevel.Debug);
                manager.SetGameData(gameData);

                Debug.LogLine("[Uninstall] Remove all mod files from 00.dat files list", Debug.LogLevel.Debug);
                foreach (ModQarEntry qarEntry in mod.ModQarEntries)
                {
                    string fExt = Path.GetExtension(qarEntry.FilePath);
                    if (!fExt.Contains(".fpk"))
                    {
                        zeroFiles.RemoveAll(file => Tools.CompareHashes(file, qarEntry.FilePath));
                    }
                }
            }
            Debug.LogLine("[Uninstall] Rebuilding 00.dat", Debug.LogLevel.Basic);
            GzsLib.WriteQarArchive(ZeroPath, "_working1", zeroFiles, 3150048);

            if (hasFtexs)
            {
                Debug.LogLine("[Install] Rebuilding 01.dat", Debug.LogLevel.Basic);
                GzsLib.WriteQarArchive(OnePath, "_working2", oneFilesList, 3150048);
            }
            // end of qar repacking

            Debug.LogLine("[Uninstall] Updating 00.dat hash", Debug.LogLevel.Basic);
            manager.UpdateDatHash();

            foreach (int index in modIndices)
            {
                ModEntry mod = mods[index];
                Debug.LogLine("[Uninstall] Removing mod entry from snakebite.xml", Debug.LogLevel.Basic);
                manager.RemoveMod(mod);
            }

            CleanupDatabase();

            //Debug.LogLine("CleanupFolders", Debug.LogLevel.Basic); //allready logs
            CleanupFolders();
            Debug.LogLine("[Uninstall] Uninstall complete", Debug.LogLevel.Basic);

            return true;
        }

        public static bool foundLooseFtexs(List<string> ModFiles) // returns true if any mods in the list contain a loose texture file which was installed to 01
        {
            ModEntry metaData;
            foreach (string modfile in ModFiles)
            {
                metaData = Tools.ReadMetaData(modfile);
                foreach (ModQarEntry qarFile in metaData.ModQarEntries)
                {
                    if (qarFile.FilePath.Contains(".ftex"))
                        return true;
                }
            }
            return false;
        }

        public static bool foundLooseFtexs(CheckedListBox.CheckedIndexCollection modIndices) // returns true if any mods at the indices contain a loose texture file which was installed to 01
        {
            var mods = new SettingsManager(GameDir).GetInstalledMods();
            foreach (int index in modIndices)
            {
                ModEntry mod = mods[index];
                foreach (ModQarEntry qarFile in mod.ModQarEntries)
                {
                    if (qarFile.FilePath.Contains(".ftex"))
                        return true;
                }
            }
            return false;
        }

        public static void MergeAndCleanup() // move vanilla 00 files to 01, moves vanilla 01 textures to texture7, cleans snakebite.xml 
        {
            MoveDatFiles(); //moves vanilla 00 files into 01, excluding foxpatch.
            ModifyFoxfs(); // adds lines to foxfs in chunk0.
            CleanupDatabase();
        }

        public static void ModifyFoxfs() // edits the chunk/texture lines in foxfs.dat to accommodate a_chunk7 a_texture7, MGO and GZs data.
        {
            CleanupFolders();
            Debug.LogLine("[ModifyChunk0] Beginning modification of Chunk0.dat", Debug.LogLevel.Debug);

            string foxfsInPath = "foxfs.dat";
            string foxfsOutPath = "_extr\\foxfs.dat";
            string chunk0Path = Properties.Settings.Default.InstallPath + "\\master\\chunk0.dat";

            Debug.LogLine("[ModifyChunk0] Checking foxfs.dat", Debug.LogLevel.Debug);
            if (GzsLib.ExtractFile<QarFile>(chunk0Path, foxfsInPath, foxfsOutPath)) //extract foxfs alone, to save time if the changes are already made
            {
                if (!File.ReadAllText(foxfsOutPath).Contains("a_chunk7.dat")) // checks if there's an indication that it's modified
                {
                    Debug.LogLine("[ModifyChunk0] Extracting Chunk0.dat", Debug.LogLevel.Debug);
                    List<string> chunk0Files = GzsLib.ExtractArchive<QarFile>(chunk0Path, "_extr"); //extract chunk0 into _extr


                    string[] linesToAdd = new string[8]
                    {
                    "		<chunk id=\"0\" label=\"old\" qar=\"a_chunk7.dat\" textures=\"a_texture7.dat\"/>",
                    "		<chunk id=\"1\" label=\"cypr\" qar=\"chunk0.dat\" textures=\"texture0.dat\"/>",
                    "		<chunk id=\"2\" label=\"base\" qar=\"chunk1.dat\" textures=\"texture1.dat\"/>",
                    "		<chunk id=\"3\" label=\"afgh\" qar=\"chunk2.dat\" textures=\"texture2.dat\"/>",
                    "		<chunk id=\"4\" label=\"mtbs\" qar=\"chunk3.dat\" textures=\"texture3.dat\"/>",
                    "		<chunk id=\"5\" label=\"mafr\" qar=\"chunk4.dat\" textures=\"texture4.dat\"/>",
                    "		<chunk id=\"6\" label=\"mgo\" qar=\"chunk5_mgo0.dat\" textures=\"texture5_mgo0.dat\"/>",
                    "		<chunk id=\"7\" label=\"gzs\" qar=\"chunk6_gzs0.dat\" textures=\"texture6_gzs0.dat\"/>",
                    };
                    Debug.LogLine("[ModifyChunk0] Updating foxfs.dat", Debug.LogLevel.Debug);
                    var foxfsLine = File.ReadAllLines(foxfsOutPath).ToList();   // read the file
                    int startIndex = foxfsLine.IndexOf("		<chunk id=\"0\" label=\"cypr\" qar=\"chunk0.dat\" textures=\"texture0.dat\"/>");

                    foxfsLine.RemoveRange(startIndex, 6);
                    foxfsLine.InsertRange(startIndex, linesToAdd);

                    File.WriteAllLines(foxfsOutPath, foxfsLine); // write to file

                    Debug.LogLine("[ModifyChunk0] repacking chunk0.dat", Debug.LogLevel.Debug); //repack chunk0 with modified foxfs
                    GzsLib.WriteQarArchive(chunk0Path, "_extr", chunk0Files, 3146208);
                }
            }
            else Debug.LogLine("[ModifyChunk0] Process failed. Could not check foxfs.dat", Debug.LogLevel.Debug);

            Debug.LogLine("[ModifyChunk0] Modification Complete.", Debug.LogLevel.Debug);
            CleanupFolders();
        }

        public static void MoveDatFiles() // moves all vanilla 00.dat files, excluding foxpatch.dat, to 01.dat
        {
            SettingsManager manager = new SettingsManager(GameDir);
            var modQarFiles = manager.GetModQarFiles();
            string sourceName = null;
            string destName = null;
            string destFolder = null;

            CleanupFolders();

            if (manager.IsExpected0001DatHash())
            {   // first time setup or files have been revalidated.
                // lua files 00 -> 01,    texture files 01 -> texture7,   foxpatch 00 -> 00,   chunkfiles 00 -> chunk7
                Debug.LogLine("[DatMerge] First Time Setup Started", Debug.LogLevel.Debug);

                bool c7t7Exists = true;
                if (manager.SettingsExist()) manager.DeleteSettings();
                if (File.Exists(c7Path)) File.Delete(c7Path);
                if (File.Exists(t7Path)) File.Delete(t7Path);
                while (c7t7Exists)
                {
                    Thread.Sleep(100);
                    c7t7Exists = false;
                    if (File.Exists(c7Path)) c7t7Exists = true;
                    if (File.Exists(t7Path)) c7t7Exists = true;
                }

                List<string> zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_extr");
                List<string> chunk7Files = new List<string>();
                List<string> oneFiles = new List<string>();

                if (!File.Exists(destName)) File.Move(OnePath, t7Path); // just renames 01.dat to a_texture7.dat

                List<string> zeroOut = zeroFiles.ToList();

                foreach (string zeroFile in zeroFiles)
                {
                    if (zeroFile == "foxpatch.dat") continue;

                    sourceName = Path.Combine("_extr", Tools.ToWinPath(zeroFile));

                    if (zeroFile.Contains(".lua"))
                    {
                        destName = Path.Combine("_working1", Tools.ToWinPath(zeroFile)); // 00 -> 01
                        oneFiles.Add(zeroFile);
                    }
                    else
                    {
                        destName = Path.Combine("_working2", Tools.ToWinPath(zeroFile)); // 00 -> chunk7
                        chunk7Files.Add(zeroFile);
                    }
                    zeroOut.Remove(zeroFile);

                    destFolder = Path.GetDirectoryName(destName);
                    if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                    if (!File.Exists(destName)) File.Move(sourceName, destName);
                }

                // Rebuild 00.dat
                GzsLib.WriteQarArchive(ZeroPath, "_extr", zeroOut, 3150304);

                // Rebuild 01.dat with files
                GzsLib.WriteQarArchive(OnePath, "_working1", oneFiles, 3150048);

                // Build chunk7
                GzsLib.WriteQarArchive(c7Path, "_working2", chunk7Files, 3146208);

            }
            else
            {   // the "uncertainty" case.
                // 00 non-snakebite Files to 01,  01 lua files unchanged,   01 textures -> t7,   01 chunkfiles -> c7, 

                Debug.LogLine("[DatMerge] Dispersing files from 00 to 01, and then from 01 to a_chunk7 and a_texture7 if necessary.", Debug.LogLevel.Debug);
                List<string> oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_extr");
                List<string> zeroList = new List<string>();
                int moveCount = 0;


                try
                {
                    zeroList = GzsLib.ListArchiveContents<QarFile>(ZeroPath);
                }
                catch (Exception e)
                {
                    Debug.LogLine(String.Format("[Error] GzsLib.ListArchiveContents exception: {0}", e.Message), Debug.LogLevel.Debug);
                    throw e;
                }

                foreach (string zeroFile in zeroList)
                {
                    if (zeroFile == "foxpatch.dat") continue;
                    if (modQarFiles.Contains(Tools.ToQarPath(zeroFile)) || oneFiles.Contains(zeroFile)) continue;
                    if (oneFiles.Contains(zeroFile)) continue;
                    moveCount++;
                }
                if (moveCount > 0) //if any non-snakebite files exist in 00, move them to 01.
                {
                    Debug.LogLine("[DatMerge] Moving files to 01.dat.", Debug.LogLevel.Debug);
                    List<string> zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working1");
                    List<string> zeroOut = zeroFiles.ToList();

                    foreach (string zeroFile in zeroFiles)
                    {
                        if (zeroFile == "foxpatch.dat") continue;
                        if (modQarFiles.Contains(Tools.ToQarPath(zeroFile))) continue;
                        if (oneFiles.Contains(zeroFile)) { zeroOut.Remove(zeroFile); continue; } //if it already exists in 01 then there's nowhere for it to go.

                        sourceName = Path.Combine("_working1", Tools.ToWinPath(zeroFile));
                        destName = Path.Combine("_extr", Tools.ToWinPath(zeroFile));

                        oneFiles.Add(zeroFile);// 00 -> 01
                        zeroOut.Remove(zeroFile);

                        destFolder = Path.GetDirectoryName(destName);
                        if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                        if (!File.Exists(destName)) File.Move(sourceName, destName);
                    }

                    GzsLib.WriteQarArchive(ZeroPath, "_working1", zeroOut, 3150304); // rebuild 00 archive


                    Directory.Delete("_working1", true); // clean up _working1, to be used by texture7
                    while (Directory.Exists("_working1"))
                        Thread.Sleep(100);
                }

                moveCount = 0; // check if any files need to be moved to C7/T7
                int textureCount = 0;
                List<string> chunk7List = new List<string>();
                List<string> tex7List = new List<string>();

                try
                {
                    tex7List = GzsLib.ListArchiveContents<QarFile>(t7Path);
                    chunk7List = GzsLib.ListArchiveContents<QarFile>(c7Path);
                }
                catch (Exception e)
                {
                    Debug.LogLine(String.Format("[Error] GzsLib.ListArchiveContents exception: {0}", e.Message), Debug.LogLevel.Debug);
                    throw e;
                }

                foreach (string oneFile in oneFiles)
                {
                    if (modQarFiles.Contains(Tools.ToQarPath(oneFile)) || tex7List.Contains(oneFile) || chunk7List.Contains(oneFile)) continue;
                    if (oneFile.Contains(".lua")) continue; // lua files must stay in 01
                    if (oneFile.Contains(".ftex")) textureCount++;
                    moveCount++;
                }
                if (moveCount > 0)
                {
                    List<string> oneOut = oneFiles.ToList();

                    if (textureCount > 0) // if non-snakebite textures exist, move them to t7
                    {
                        Debug.LogLine("[DatMerge] Moving files to a_texture7.dat.", Debug.LogLevel.Debug);
                        List<string> texture7Files = new List<string>();
                        if (File.Exists(t7Path)) texture7Files = GzsLib.ExtractArchive<QarFile>(t7Path, "_working1");

                        foreach (string oneFile in oneFiles) // once 00 files have been moved, move 01 files into t7, c7.
                        {
                            if (modQarFiles.Contains(Tools.ToQarPath(oneFile))) continue;
                            if (oneFile.Contains(".ftex"))
                            {
                                sourceName = Path.Combine("_extr", Tools.ToWinPath(oneFile));
                                destName = Path.Combine("_working1", Tools.ToWinPath(oneFile)); // 01 -> texture7
                                destFolder = Path.GetDirectoryName(destName);

                                if (!texture7Files.Contains(oneFile)) texture7Files.Add(oneFile);
                                oneOut.Remove(oneFile);
                                if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                                if (!File.Exists(destName)) File.Move(sourceName, destName);
                            }
                        }
                        GzsLib.WriteQarArchive(t7Path, "_working1", texture7Files, 3150048);
                    }

                    oneFiles = oneOut.ToList(); // update oneFiles to remove any .ftex already found
                    if (oneFiles.Count > 0) // if any other files need to be moved, they go in chunk7
                    {
                        Debug.LogLine("[DatMerge] Moving files to a_chunk7.dat.", Debug.LogLevel.Debug);
                        List<string> chunk7Files = new List<string>();
                        if (File.Exists(c7Path)) chunk7Files = GzsLib.ExtractArchive<QarFile>(c7Path, "_working2");

                        foreach (string oneFile in oneFiles) // once 00 files have been moved, move 01 files into t7, c7.
                        {
                            if (modQarFiles.Contains(Tools.ToQarPath(oneFile))) continue;
                            if (oneFile.Contains(".lua")) continue;

                            sourceName = Path.Combine("_extr", Tools.ToWinPath(oneFile));
                            destName = Path.Combine("_working2", Tools.ToWinPath(oneFile)); // 00 -> chunk7
                            destFolder = Path.GetDirectoryName(destName);

                            if (!chunk7Files.Contains(oneFile)) chunk7Files.Add(oneFile);
                            oneOut.Remove(oneFile);


                            if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                            if (!File.Exists(destName)) File.Move(sourceName, destName);
                        }
                        GzsLib.WriteQarArchive(c7Path, "_working2", chunk7Files, 3146208); // rebuild chunk7 archive
                    }
                    GzsLib.WriteQarArchive(OnePath, "_extr", oneOut, 3150048); // rebuild 01 archive
                }
            }
            Debug.LogLine(String.Format("[DatMerge] Archive merging complete."), Debug.LogLevel.Debug);
            manager.UpdateDatHash();
            CleanupFolders();
        }
        public static void CleanupDatabase()
        {
            Debug.LogLine("[Cleanup] Database cleanup started", Debug.LogLevel.Basic);

            // Retrieve installation data
            SettingsManager manager = new SettingsManager(GameDir);
            var mods = manager.GetInstalledMods();
            var game = manager.GetGameData();
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
                try
                {
                    File.Delete(fpkName);
                }
                catch (IOException e)
                {
                    Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                }
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
            manager.SetGameData(game);
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

        private static void CleanupFolders(bool skipCleanup=false) // deletes the work folders which contain extracted files from 00/01
        {
            if (Directory.Exists("_working1")) Directory.Delete("_working1", true);
            if (Directory.Exists("_working2")) Directory.Delete("_working2", true);
            if (Directory.Exists("_extr")) Directory.Delete("_extr", true);
            if (Directory.Exists("_gamefpk")) Directory.Delete("_gamefpk", true);
            if (Directory.Exists("_modfpk")) Directory.Delete("_modfpk", true);
            Debug.LogLine("[Mod] Cleaning up snakebite work folders.");

            bool directoryExists = true;
            while (directoryExists) {
                Thread.Sleep(100);
                directoryExists = false;
                if (Directory.Exists("_working1")) directoryExists = true;
                if (Directory.Exists("_working2")) directoryExists = true;
                if (Directory.Exists("_extr")) directoryExists = true;
                if (Directory.Exists("_gamefpk")) directoryExists = true;
                if (Directory.Exists("_modfpk")) directoryExists = true;
            }
        }      
      
    }
}