﻿using GzsTool.Core.Fpk;
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


            List<string> zeroFiles = new List<string>();
            bool hasQarZero = ModManager.hasQarZeroFiles(installEntryList);
            if (hasQarZero)
            {
                zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working0");
            }

            List<string> oneFiles = null;
            bool hasFtexs = ModManager.foundLooseFtexs(installEntryList); 
            if (hasFtexs)
            {
                oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_working1");
            }

            SettingsManager SBBuildManager = new SettingsManager(SnakeBiteSettings + build_ext);
            var gameData = SBBuildManager.GetGameData();
            ModManager.ValidateGameData(ref gameData, ref zeroFiles);

            var zeroFilesHashSet = new HashSet<string>(zeroFiles);

            Debug.LogLine("[Install] Building gameFiles lists", Debug.LogLevel.Basic);
            var baseGameFiles = GzsLib.ReadBaseData();
            var allQarGameFiles = new List<Dictionary<ulong, GameFile>>();
            allQarGameFiles.AddRange(baseGameFiles);


            try
            {
                ModManager.PrepGameDirFiles();
                List<string> pullFromVanillas; List<string> pullFromMods; Dictionary<string, bool> pathUpdatesExist;

                Debug.LogLine("[Install] Writing FPK data to Settings", Debug.LogLevel.Basic);
                AddToSettingsFpk(installEntryList, SBBuildManager, allQarGameFiles, out pullFromVanillas, out pullFromMods, out pathUpdatesExist);
                InstallMods(ModFiles, SBBuildManager, pullFromVanillas, pullFromMods, ref zeroFilesHashSet, ref oneFiles, pathUpdatesExist);

                if (hasQarZero)
                {
                    zeroFiles = zeroFilesHashSet.ToList();
                    zeroFiles.Sort();
                    GzsLib.WriteQarArchive(ZeroPath + build_ext, "_working0", zeroFiles, GzsLib.zeroFlags);
                }
                if (hasFtexs)
                {
                    oneFiles.Sort();
                    GzsLib.WriteQarArchive(OnePath + build_ext, "_working1", oneFiles, GzsLib.oneFlags);
                }

                ModManager.PromoteGameDirFiles();
                ModManager.PromoteBuildFiles(ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath);

                if (!skipCleanup)
                {
                    ModManager.CleanupFolders();
                    ModManager.ClearSBGameDir();
                }

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

                bool restoreRetry = false;
                do
                {
                    try
                    {
                        ModManager.RestoreBackupGameDir(SBBuildManager);
                    }
                    catch (Exception f)
                    {
                        Debug.LogLine("[Uninstall] Exception: " + f, Debug.LogLevel.Basic);
                        restoreRetry = DialogResult.Retry == MessageBox.Show("SnakeBite could not restore Game Directory mod files due to the following exception: {f} \nWould you like to retry?", "Exception Occurred", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    }
                } while (restoreRetry);

                ModManager.ClearSBGameDir();
                return false;
            }
        }

        /// <summary>
        /// Merges the new mod files with the existing modded files and vanilla game files.
        /// The resulting file structure is the new _working0 folder to pack as 00.dat 
        /// </summary>
        private static void InstallMods(List<string> modFilePaths, SettingsManager manager, List<string> pullFromVanillas, List<string> pullFromMods, ref HashSet<string> zeroFiles, ref List<string> oneFilesList, Dictionary<string, bool> pathUpdatesExist)
        {
            //Assumption: modded packs have already been extracted to _working0 directory - qarEntryEditList
            //Assumption: vanilla packs have already been extracted to _gameFpk directory (during AddToSettings) - fpkEntryRetrievalList
            //theoretically there should be no qar overlap between the _gamefpk(vanilla) and _working0(modded) files
            FastZip unzipper = new FastZip();
            GameData gameData = manager.GetGameData();

            foreach (string modFilePath in modFilePaths) 
            {
                Debug.LogLine($"[Install] Installation started: {Path.GetFileName(modFilePath)}", Debug.LogLevel.Basic);
                
                Debug.LogLine($"[Install] Unzipping mod .mgsv ({Tools.GetFileSizeKB(modFilePath)} KB)", Debug.LogLevel.Basic);
                unzipper.ExtractZip(modFilePath, "_extr", "(.*?)");

                Debug.LogLine("[Install] Load mod metadata", Debug.LogLevel.Basic);
                ModEntry extractedModEntry = new ModEntry("_extr\\metadata.xml");
                if (pathUpdatesExist[extractedModEntry.Name])
                {
                    Debug.LogLine(string.Format("[Install] Checking for Qar path updates: {0}", extractedModEntry.Name), Debug.LogLevel.Basic);
                    foreach (ModQarEntry modQar in extractedModEntry.ModQarEntries.Where(entry => !entry.FilePath.StartsWith("/Assets/")))
                    {
                        string unhashedName = HashingExtended.UpdateName(modQar.FilePath);
                        if (unhashedName != null)
                        {
                            Debug.LogLine(string.Format("[Install] Update successful: {0} -> {1}", modQar.FilePath, unhashedName), Debug.LogLevel.Basic);

                            string workingOldPath = Path.Combine("_extr", Tools.ToWinPath(modQar.FilePath));
                            string workingNewPath = Path.Combine("_extr", Tools.ToWinPath(unhashedName));
                            if (!Directory.Exists(Path.GetDirectoryName(workingNewPath))) Directory.CreateDirectory(Path.GetDirectoryName(workingNewPath));
                            if (!File.Exists(workingNewPath)) File.Move(workingOldPath, workingNewPath);

                            modQar.FilePath = unhashedName;

                        }
                    }
                }
                    
                GzsLib.LoadModDictionary(extractedModEntry);
                ValidateModEntries(ref extractedModEntry);

                Debug.LogLine("[Install] Check mod FPKs against game .dat fpks", Debug.LogLevel.Basic);
                zeroFiles.UnionWith(MergePacks(extractedModEntry, pullFromVanillas, pullFromMods));
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

        private static HashSet<string> MergePacks(ModEntry extractedModEntry, List<string> pullFromVanillas, List<string> pullFromMods)
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
                        if (modQar.FilePath.EndsWith(".fpk") || modQar.FilePath.EndsWith(".fpkd"))
                            pullFromMods.Add(modQar.FilePath); // for merging non-native fpk files consecutively
                    }
                }

                if (existingQarSource != null)
                {
                    var pulledPack = GzsLib.ExtractArchive<FpkFile>(existingQarSource, "_build");
                    var extrPack = GzsLib.ExtractArchive<FpkFile>(modQarSource, "_build");
                    pulledPack = pulledPack.Union(extrPack).ToList();
                    //foreach(string file in extrPack) Debug.LogLine(string.Format("{0} is listed in the archive extr", file));
                    GzsLib.WriteFpkArchive(workingDestination, "_build", pulledPack);
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
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(sourceFile, destFile, true);

                    if (gameData.GameFileEntries.FirstOrDefault(e => e.FilePath == fileEntry.FilePath) == null)
                        gameData.GameFileEntries.Add(fileEntry);
                }
            }
        }//InstallGameDirFiles

        private static void AddToSettingsFpk(List<ModEntry> installEntryList, SettingsManager manager, List<Dictionary<ulong, GameFile>> allQarGameFiles, out List<string> PullFromVanillas, out List<string> pullFromMods, out Dictionary<string, bool> pathUpdatesExist)
        {
            GameData gameData = manager.GetGameData();
            pathUpdatesExist = new Dictionary<string, bool>();

            List<string> newModQarEntries = new List<string>();
            List<string> modQarFiles = manager.GetModQarFiles();
            pullFromMods = new List<string>();
            foreach (ModEntry modToInstall in installEntryList)
            {
                Dictionary<string, string> newNameDictionary = new Dictionary<string, string>();
                int foundUpdate = 0;
                foreach (ModQarEntry modQar in modToInstall.ModQarEntries.Where(entry => !entry.FilePath.StartsWith("/Assets/")))
                {
                    //Debug.LogLine(string.Format("Attempting to update Qar filename: {0}", modQar.FilePath), Debug.LogLevel.Basic);
                    string unhashedName = HashingExtended.UpdateName(modQar.FilePath);
                    if (unhashedName != null)
                    {
                        //Debug.LogLine(string.Format("Success: {0}", unhashedName), Debug.LogLevel.Basic);
                        newNameDictionary.Add(modQar.FilePath, unhashedName);
                        foundUpdate++;

                        modQar.FilePath = unhashedName;
                        if (!pathUpdatesExist.ContainsKey(modToInstall.Name))
                            pathUpdatesExist.Add(modToInstall.Name, true);
                        else
                            pathUpdatesExist[modToInstall.Name] = true;
                    }
                }
                if (foundUpdate > 0)
                {
                    foreach (ModFpkEntry modFpkEntry in modToInstall.ModFpkEntries)
                    {
                        string unHashedName;
                        if (newNameDictionary.TryGetValue(modFpkEntry.FpkFile, out unHashedName))
                            modFpkEntry.FpkFile = unHashedName;
                    }
                }
                else if(!pathUpdatesExist.ContainsKey(modToInstall.Name))
                        pathUpdatesExist.Add(modToInstall.Name, false);

                manager.AddMod(modToInstall);
                //foreach (ModQarEntry modqar in modToInstall.ModQarEntries) Debug.LogLine("Mod Qar in modToInstall: " + modqar.FilePath);
                foreach (ModQarEntry modQarEntry in modToInstall.ModQarEntries) // add qar entries (fpk, fpkd) to GameData if they don't already exist
                {
                    string modQarFilePath = modQarEntry.FilePath;
                    if (!(modQarFilePath.EndsWith(".fpk") || modQarFilePath.EndsWith(".fpkd"))) continue; // only pull for Qar's with Fpk's

                    if (modQarFiles.Any(entry => entry == modQarFilePath))
                    {
                        pullFromMods.Add(modQarFilePath);
                        //Debug.LogLine("Pulling from 00.dat: {0} " + modQarFilePath);
                    }
                    else if (!newModQarEntries.Contains(modQarFilePath))
                    {
                        newModQarEntries.Add(modQarFilePath);
                        //Debug.LogLine("Pulling from base archives: {0} " + modQarFilePath);
                    }

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
                            foreach (string listedFile in GzsLib.ListArchiveContents<FpkFile>(workingPath))
                            {
                                repairFpkEntries.Add(new ModFpkEntry {
                                    FpkFile = newFpkEntry.FpkFile,
                                    FilePath = listedFile,
                                    SourceType = FileSource.Merged,
                                    SourceName = existingPack.QarFile
                                });
                                //Debug.LogLine(string.Format("File Listed: {0} in {1}", extractedFile, newFpkEntry.FpkFile));
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
            gameData.GameFpkEntries = gameData.GameFpkEntries.Union(repairFpkEntries).ToList();
            manager.SetGameData(gameData);
        }
    }
}
