using GzsTool.Core.Fpk;
using GzsTool.Core.Qar;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static SnakeBite.GamePaths;

namespace SnakeBite
{
    internal static class InstallManager
    {
        public static bool InstallMods(List<string> ModFiles, bool skipCleanup = false)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Debug.LogLine("[Install] Start", Debug.LogLevel.Basic);
            ModManager.ClearBuildFiles(ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath); // deletes any leftover sb_build files that might still be in the directory (ie from a mid-process shutdown) 
            ModManager.ClearSBGameDir(); // deletes the game directory sb_build
            ModManager.CleanupFolders(); // deletes the work folders which contain extracted files from 00/01

            if (Properties.Settings.Default.AutosaveRevertPreset == true)
            {
                Debug.LogLine("[Install] Saving RevertChanges.MGSVPreset.SB_Build", Debug.LogLevel.Basic);
                PresetManager.SavePreset(SavePresetPath + build_ext); // creates a backup preset file sb_build
            }
            else
            {
                Debug.LogLine("[Install] Skipping RevertChanges.MGSVPreset Save", Debug.LogLevel.Basic);
            }
            File.Copy(SnakeBiteSettings, SnakeBiteSettings + build_ext, true); // creates a settings sb_build

            GzsLib.LoadDictionaries();
            List<ModEntry> installEntryList = new List<ModEntry>();
            foreach(string modFile in ModFiles) installEntryList.Add(Tools.ReadMetaData(modFile));

            var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working0");
            List<string> oneFiles = null;
            bool hasFtexs = ModManager.foundLooseFtexs(installEntryList); 
            if (hasFtexs)
            {
                oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_working1");
            }

            SettingsManager manager = new SettingsManager(SnakeBiteSettings + build_ext);
            var gameData = manager.GetGameData();
            ModManager.ValidateGameData(ref gameData, ref zeroFiles);

            var zeroFilesHashSet = new HashSet<string>(zeroFiles);

            Debug.LogLine("[Install] Building gameFiles lists", Debug.LogLevel.Basic);
            var baseGameFiles = GzsLib.ReadBaseData();
            var allQarGameFiles = new List<Dictionary<ulong, GameFile>>();
            allQarGameFiles.AddRange(baseGameFiles);


            try
            {
                ModManager.WriteGameDirSbBuild();
                List<string> pullFromVanillas; List<string> pullFromMods;

                Debug.LogLine("[Install] Writing FPK data to Settings", Debug.LogLevel.Basic);
                AddToSettingsFpk(installEntryList, manager, allQarGameFiles, out pullFromVanillas, out pullFromMods);
                InstallMods(ModFiles, manager, pullFromVanillas, pullFromMods, ref zeroFilesHashSet, ref oneFiles);

                Debug.LogLine("[Install] Rebuilding 00.dat", Debug.LogLevel.Basic);
                zeroFiles = zeroFilesHashSet.ToList();
                zeroFiles.Sort();
                

                GzsLib.WriteQarArchive(ZeroPath + build_ext, "_working0", zeroFiles, GzsLib.zeroFlags);
                if (hasFtexs)
                {
                    Debug.LogLine("[Install] Rebuilding 01.dat", Debug.LogLevel.Basic);
                    oneFiles.Sort();
                    GzsLib.WriteQarArchive(OnePath + build_ext, "_working1", oneFiles, GzsLib.oneFlags);
                }
                if (!skipCleanup)
                {
                    //DEBUGNOW this takes a big chunk of time, see what of it is absolutenly nessesary 
                    //DEBUGNOW    CleanupDatabase();
                }
                if (!skipCleanup)
                {
                    ModManager.CleanupFolders();
                }
                ModManager.PromoteGameDirFiles();
                ModManager.PromoteBuildFiles(ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath);

                ModManager.ClearSBGameDir();
                stopwatch.Stop();
                Debug.LogLine($"[Install] Installation finished in {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
                return true;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogLine($"[Install] Installation failed at {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
                Debug.LogLine("[Install] Exception: " + e, Debug.LogLevel.Basic);
                MessageBox.Show("An error has occurred during the installation process and SnakeBite could not install the selected mod(s).\nException: " + e, "Mod(s) could not be installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ModManager.ClearBuildFiles(ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath);
                ModManager.CleanupFolders();
                ModManager.ClearSBGameDir();
                return false;
            }
        }

        /// <summary>
        /// Merges the new mod files with the existing modded files and vanilla game files.
        /// The resulting file structure is the new _working0 folder to pack as 00.dat 
        /// </summary>
        private static void InstallMods(List<string> modFilePaths, SettingsManager manager, List<string> pullFromVanillas, List<string> pullFromMods, ref HashSet<string> zeroFiles, ref List<string> oneFilesList)
        {
            //Assumption: modded packs have already been extracted to _working0 directory - qarEntryEditList
            //Assumption: vanilla packs have already been extracted to _gameFpk directory (during AddToSettings) - fpkEntryRetrievalList
            //theoretically there should be no qar overlap between the _gamefpk(vanilla) and _working0(modded) files
            FastZip unzipper = new FastZip();
            GameData gameData = manager.GetGameData();

            foreach (string modFilePath in modFilePaths) //for every qar file of every mod, if the file is in qarEntryEditList, extract from X and X. if file is in qarRetrievalFilePaths, extract y and y
            {
                Debug.LogLine($"[Install] Installation started: {modFilePath}", Debug.LogLevel.Basic);
                Debug.LogLine("[Install] Unzip mod .mgsv", Debug.LogLevel.Basic);
                unzipper.ExtractZip(modFilePath, "_extr", "(.*?)");

                Debug.LogLine("[Install] Load mod metadata", Debug.LogLevel.Basic);
                ModEntry extractedModEntry = new ModEntry("_extr\\metadata.xml");
                GzsLib.LoadModDictionary(extractedModEntry);
                ValidateModEntries(ref extractedModEntry);

                Debug.LogLine("[Install] Check mod FPKs against game .dat fpks", Debug.LogLevel.Basic);
                zeroFiles.UnionWith(MergeFpks(extractedModEntry, pullFromVanillas, pullFromMods));
                //foreach (string zeroFile in zeroFiles) Debug.LogLine(string.Format("Contained in zeroFiles: {0}", zeroFile));

                Debug.LogLine("[Install] Copying loose textures to 01.", Debug.LogLevel.Basic);
                InstallLooseFtexs(extractedModEntry, ref oneFilesList);

                Debug.LogLine("[Install] Copying game dir files", Debug.LogLevel.Basic);
                InstallGameDirFiles(extractedModEntry, ref gameData);
            }

            manager.SetGameData(gameData);
        }

        private static void ValidateModEntries(ref ModEntry modEntry)
        {
            Debug.LogLine("[ValidateModEntries] Validating qar entries", Debug.LogLevel.Basic);
            for (int i = modEntry.ModQarEntries.Count - 1; i >= 0; i--)
            {
                ModQarEntry qarEntry = modEntry.ModQarEntries[i];
                if (!GzsLib.IsExtensionValidForArchive(qarEntry.FilePath, ".dat"))
                {
                    Debug.LogLine($"[ValidateModEntries] Found invalid file entry {qarEntry.FilePath} for archive {qarEntry.SourceName}", Debug.LogLevel.Basic);
                    modEntry.ModQarEntries.RemoveAt(i);
                }
            }
            Debug.LogLine("[ValidateModEntries] Validating fpk entries", Debug.LogLevel.Basic);
            for (int i = modEntry.ModFpkEntries.Count - 1; i >= 0; i--)
            {
                ModFpkEntry fpkEntry = modEntry.ModFpkEntries[i];
                if (!GzsLib.IsExtensionValidForArchive(fpkEntry.FilePath, fpkEntry.FpkFile))
                {
                    Debug.LogLine($"[ValidateModEntries] Found invalid file entry {fpkEntry.FilePath} for archive {fpkEntry.FpkFile}", Debug.LogLevel.Basic);
                    modEntry.ModFpkEntries.RemoveAt(i);
                }
            }
        }

        private static HashSet<string> MergeFpks(ModEntry extractedModEntry, List<string> pullFromVanillas, List<string> pullFromMods)
        {
            HashSet<string> modQarZeroPaths = new HashSet<string>();
            foreach (ModQarEntry modQar in extractedModEntry.ModQarEntries)
            {
                string workingDestination = Path.Combine("_working0", Tools.ToWinPath(modQar.FilePath));
                if (!Directory.Exists(Path.GetDirectoryName(workingDestination))) Directory.CreateDirectory(Path.GetDirectoryName(workingDestination));
                string modQarSource = Path.Combine("_extr", Tools.ToWinPath(modQar.FilePath));
                string existingQarSource;

                if (pullFromMods.FirstOrDefault(e => e == modQar.FilePath) != null)
                {
                    //Debug.LogLine(string.Format("{0}'s Qar file '{1}' will pull from _working0 (modded)", extractedModEntry.Name, modQar.FilePath));
                    existingQarSource = workingDestination;
                }
                else
                {
                    int indexToRemove = pullFromVanillas.FindIndex(m => m == modQar.FilePath); // remove from retrievalfilepaths and add to editlist
                    if (indexToRemove >= 0)
                    {
                        //Debug.LogLine(string.Format("{0}'s Qar file '{1}' will pull from _gameFpk (fresh game file)", extractedModEntry.Name, modQar.FilePath));
                        existingQarSource = Path.Combine("_gameFpk", Tools.ToWinPath(modQar.FilePath));
                        pullFromVanillas.RemoveAt(indexToRemove); pullFromMods.Add(modQar.FilePath);
                    }
                    else
                    {
                        //Debug.LogLine(string.Format("{0}'s Qar file '{1}' is non-native or not mergeable", extractedModEntry.Name, modQar.FilePath));
                        existingQarSource = null;
                    }
                }

                if (existingQarSource != null)
                {
                    var pulledFpk = GzsLib.ExtractArchive<FpkFile>(existingQarSource, "_build");
                    var extrFpk = GzsLib.ExtractArchive<FpkFile>(modQarSource, "_build");
                    pulledFpk.Union(extrFpk);
                    GzsLib.WriteFpkArchive(workingDestination, "_build", pulledFpk);
                }
                else
                {
                    File.Copy(modQarSource, workingDestination, true);
                }

                if (!modQar.FilePath.Contains(".ftex"))
                {
                    //Debug.LogLine(string.Format("Adding {0}'s Qar file '{1}' to 00.dat", extractedModEntry.Name, modQar.FilePath));
                    modQarZeroPaths.Add(Tools.ToWinPath(modQar.FilePath));
                }
            }

            return modQarZeroPaths;
        }

        // i/o: _extr to _working1
        private static void InstallLooseFtexs(ModEntry modEntry, ref List<string> oneFilesList)
        {
            foreach (ModQarEntry modQarEntry in modEntry.ModQarEntries)
            {
                if (modQarEntry.FilePath.Contains(".ftex"))
                {
                    if (!oneFilesList.Contains(Tools.ToWinPath(modQarEntry.FilePath)))
                    {
                        oneFilesList.Add(Tools.ToWinPath(modQarEntry.FilePath));
                    }
                    string sourceFile = Path.Combine("_extr", Tools.ToWinPath(modQarEntry.FilePath));
                    string destFile = Path.Combine("_working1", Tools.ToWinPath(modQarEntry.FilePath));
                    string destDir = Path.GetDirectoryName(destFile);
                    Debug.LogLine(String.Format("[Install] Copying texture file: {0}", modQarEntry.FilePath), Debug.LogLevel.All);
                    if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                    File.Copy(sourceFile, destFile, true);
                }
            }
        }

        // i/o: _extr to GameDir
        private static void InstallGameDirFiles(ModEntry modEntry, ref GameData gameData)
        {
            foreach (ModFileEntry fileEntry in modEntry.ModFileEntries)
            {
                bool skipFile = false;
                foreach (string ignoreFile in Tools.ignoreFileList)
                {
                    if (fileEntry.FilePath.Contains(ignoreFile))
                    {
                        skipFile = true;
                    }
                }
                /*
                foreach (string ignoreExt in ignoreExtList)
                {
                    if (fileEntry.FilePath.Contains(ignoreExt))
                    {
                        skipFile = true;
                    }
                }
                */
                if (skipFile == false)
                {
                    string sourceFile = Path.Combine("_extr\\GameDir", Tools.ToWinPath(fileEntry.FilePath));
                    string destFile = Path.Combine(GameDirSB_Build, Tools.ToWinPath(fileEntry.FilePath));
                    Debug.LogLine(string.Format("[Install] Copying file: {0}", destFile), Debug.LogLevel.All);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(sourceFile, destFile, true);

                    if (gameData.GameFileEntries.FirstOrDefault(e => e.FilePath == fileEntry.FilePath) == null)
                        gameData.GameFileEntries.Add(fileEntry);
                }
            }
        }//InstallGameDirFiles

        private static void AddToSettingsFpk(List<ModEntry> installEntryList, SettingsManager manager, List<Dictionary<ulong, GameFile>> allQarGameFiles, out List<string> PullFromVanillas, out List<string> pullFromMods)
        {
            GameData gameData = manager.GetGameData();

            List<string> newModQarEntries = new List<string>();
            List<string> modQarFiles = manager.GetModQarFiles();
            pullFromMods = new List<string>();

            foreach (ModEntry modToInstall in installEntryList)
            {
                manager.AddMod(modToInstall);
                //foreach (ModQarEntry modqar in modToInstall.ModQarEntries) Debug.LogLine("Mod Qar in modToInstall: " + modqar.FilePath);
                foreach (ModQarEntry modQarEntry in modToInstall.ModQarEntries) // add qar entries (fpk, fpkd) to GameData if they don't already exist
                {
                    string modQarFilePath = modQarEntry.FilePath;
                    if (!(modQarFilePath.EndsWith(".fpk") || modQarFilePath.EndsWith(".fpkd")) || newModQarEntries.Contains(modQarFilePath)) continue;
                    if (pullFromMods.FirstOrDefault(e => e == modQarFilePath) != null) continue;

                    if (gameData.GameQarEntries.FirstOrDefault(e => e.Hash == modQarEntry.Hash) == null) // the qar file is not listed in the gameQarEntries..
                    {
                        if (modQarFiles.Contains(modQarFilePath)) // but it is listed in the QarEntries for an existing mod, therefore...
                            pullFromMods.Add(modQarFilePath); // the file is already managed in 00.dat as a non-native or unmergeable qar file. if the file is non-native, it needs to be merged with the existing 00.dat pack.
                        else
                            newModQarEntries.Add(modQarFilePath); // the file is completely new to SnakeBite and will need repair files from the base archives (if they exist)
                    }  
                    else pullFromMods.Add(modQarFilePath); // the qar is managed in 00.dat and one or more mods are using the same qar file. 
                }
            }
            //Debug.LogLine(string.Format("Foreach nest 1 complete"));
            List<ModFpkEntry> newModFpkEntries = new List<ModFpkEntry>();
            foreach (ModEntry modToInstall in installEntryList)
            {
                foreach (ModFpkEntry modFpkEntry in modToInstall.ModFpkEntries)
                {
                    //Debug.LogLine(string.Format("Checking out {0} from {1}", modFpkEntry.FilePath, modFpkEntry.FpkFile));

                    if(newModQarEntries.Contains(modFpkEntry.FpkFile)) // it isn't already part of the snakebite environment
                    {
                        //Debug.LogLine(string.Format("seeking repair files around {0}", modFpkEntry.FilePath));
                        newModFpkEntries.Add(modFpkEntry);
                    }
                    else
                    {
                        //Debug.LogLine(string.Format("Removing {0} from gameFpkEntries so it will only be listed in the mod's entries", modFpkEntry.FilePath));
                        int indexToRemove = gameData.GameFpkEntries.FindIndex(m => m.FilePath == Tools.ToWinPath(modFpkEntry.FilePath)); // this will remove the gamedata's listing of the file under fpkentries (repair entries), so the filepath will only be listed in the modentry
                        if (indexToRemove >= 0) gameData.GameFpkEntries.RemoveAt(indexToRemove);
                    }
                }
            }
            //Debug.LogLine(string.Format("Foreach nest 2 complete"));
            HashSet<ulong> mergeFpkHashes = new HashSet<ulong>();
            PullFromVanillas = new List<string>();
            var repairFpkEntries = new List<ModFpkEntry>();
            foreach (ModFpkEntry newFpkEntry in newModFpkEntries) // this will add the fpkentry listings (repair entries) to the settings xml
            {
                //Debug.LogLine(string.Format("checking {0} for repairs", newFpkEntry.FilePath));
                ulong packHash = Tools.NameToHash(newFpkEntry.FpkFile);
                if (mergeFpkHashes.Contains(packHash)) continue; // the process has already plucked this particular qar file

                foreach (var archiveQarGameFiles in allQarGameFiles) // check every archive (except 00) to see if the particular qar file already exists
                {
                    //Debug.LogLine(string.Format("checking archive for an existing qar file"));
                    if (archiveQarGameFiles.Count > 0)
                    {
                        GameFile existingPack = null;
                        archiveQarGameFiles.TryGetValue(packHash, out existingPack);
                        if (existingPack != null) // the qar file is found
                        {
                            //Debug.LogLine(string.Format("Qar file {0} found in {1}. adding to gameqarentries", newFpkEntry.FpkFile, existingPack.QarFile));
                            mergeFpkHashes.Add(packHash);
                            gameData.GameQarEntries.Add(new ModQarEntry{
                                FilePath = newFpkEntry.FpkFile,
                                SourceType = FileSource.Merged,
                                SourceName = existingPack.QarFile,
                                Hash = existingPack.FileHash
                            });
                            PullFromVanillas.Add(newFpkEntry.FpkFile);

                            string windowsFilePath = Tools.ToWinPath(newFpkEntry.FpkFile); // Extract the pack file from the vanilla game files, place into _gamefpk for future use
                            string sourceArchive = Path.Combine(GameDir, "master\\" + existingPack.QarFile);
                            string workingPath = Path.Combine("_gameFpk", windowsFilePath);
                            if (!Directory.Exists(Path.GetDirectoryName(workingPath))) Directory.CreateDirectory(Path.GetDirectoryName(workingPath));

                            GzsLib.ExtractFileByHash<QarFile>(sourceArchive, existingPack.FileHash, workingPath); // extracts the specific .fpk from the game data
                            foreach (string extractedFile in GzsLib.ListArchiveContents<FpkFile>(workingPath))
                            {
                                repairFpkEntries.Add(new ModFpkEntry {
                                    FpkFile = newFpkEntry.FpkFile,
                                    FilePath = extractedFile,
                                    SourceType = FileSource.Merged,
                                    SourceName = existingPack.QarFile
                                });
                                //Debug.LogLine(string.Format("File Extracted: {0} in {1}", extractedFile, newFpkEntry.FpkFile));
                            }
                            break;
                        }
                    }
                }
            }
            //Debug.LogLine(string.Format("Foreach nest 3 complete"));
            foreach (ModFpkEntry newFpkEntry in newModFpkEntries) // finally, strip away the modded entries from the repair entries
            {
                //Debug.LogLine(string.Format("checking to remove {0} from gamefpkentries", Tools.ToWinPath(newFpkEntry.FilePath)));
                int indexToRemove = repairFpkEntries.FindIndex(m => m.FilePath == Tools.ToWinPath(newFpkEntry.FilePath));
                if (indexToRemove >= 0) repairFpkEntries.RemoveAt(indexToRemove);
            }
            gameData.GameFpkEntries.AddRange(repairFpkEntries);
            manager.SetGameData(gameData);
        }
    }
}
