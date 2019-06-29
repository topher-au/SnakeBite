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
    class UninstallManager
    {
        private static SettingsManager SBBuildManager = new SettingsManager(SnakeBiteSettings + build_ext);

        public static bool UninstallMods(CheckedListBox.CheckedIndexCollection modIndices, bool skipCleanup = false) // Uninstalls mods based on their indices in the list
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.LogLine("[Uninstall] Start", Debug.LogLevel.Basic);

            // initial cleanup
            ModManager.ClearBuildFiles(ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath);
            ModManager.ClearSBGameDir();
            ModManager.CleanupFolders();

            // backup preset build
            if (Properties.Settings.Default.AutosaveRevertPreset == true)
            {
                Debug.LogLine("[Uninstall] Saving RevertChanges.MGSVPreset.SB_Build", Debug.LogLevel.Basic);
                PresetManager.SavePreset(SavePresetPath + build_ext);
            }
            else
            {
                Debug.LogLine("[Uninstall] Skipping RevertChanges.MGSVPreset Save", Debug.LogLevel.Basic);
            }

            GzsLib.LoadDictionaries();
            File.Copy(SnakeBiteSettings, SnakeBiteSettings + build_ext, true);
            List<ModEntry> mods = SBBuildManager.GetInstalledMods();
            Debug.LogLine("[Uninstall] Skipping RevertChanges.MGSVPreset Save", Debug.LogLevel.Basic);
            List<ModEntry> selectedMods = new List<ModEntry>();
            foreach (int index in modIndices)
            {
                ModEntry mod = mods[index];
                selectedMods.Add(mod);
            }

            List<string> zeroFiles = new List<string>();
            bool hasQarZero = ModManager.hasQarZeroFiles(selectedMods);
            if (hasQarZero)
            {
                // if necessary, extracts 00.dat and creates a list of filenames, which is pruned throughout the uninstall process and repacked at the end.
                Debug.LogLine("[Uninstall] Extracting 00.dat to _working0", Debug.LogLevel.Basic);
                zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working0"); 

            }

            List<string> oneFiles = null;
            bool hasFtexs = ModManager.foundLooseFtexs(selectedMods);
            if (hasFtexs)
            {
                // if necessary, extracts 01.dat and creates a list of filenames similar to zeroFiles. only textures are pruned from the list.
                Debug.LogLine("[Uninstall] Extracting 01.dat to _working1", Debug.LogLevel.Basic);
                oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_working1");
            }

            //end of qar extraction
            GameData gameData = SBBuildManager.GetGameData();
            ModManager.ValidateGameData(ref gameData, ref zeroFiles);

            Debug.LogLine("[Uninstall] Building gameFiles lists", Debug.LogLevel.Basic);
            var baseGameFiles = GzsLib.ReadBaseData();
            try
            {
                ModManager.PrepGameDirFiles();
                // begin uninstall
                UninstallMods(selectedMods, ref zeroFiles, ref oneFiles);

                if(hasQarZero)
                {
                    Debug.LogLine("[Uninstall] Rebuilding 00.dat", Debug.LogLevel.Basic);
                    zeroFiles.Sort();
                    GzsLib.WriteQarArchive(ZeroPath + build_ext, "_working0", zeroFiles, GzsLib.zeroFlags);
                }

                if (hasFtexs)
                {
                    Debug.LogLine("[Install] Rebuilding 01.dat", Debug.LogLevel.Basic);
                    oneFiles.Sort();
                    GzsLib.WriteQarArchive(OnePath + build_ext, "_working1", oneFiles, GzsLib.oneFlags);
                }
                // end of qar rebuild
                
                // overwrite old mod data
                ModManager.PromoteGameDirFiles();
                ModManager.PromoteBuildFiles(ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath);
                
                if (!skipCleanup)
                {
                    ModManager.CleanupFolders();
                    ModManager.ClearSBGameDir();
                }

                Debug.LogLine("[Uninstall] Uninstall complete", Debug.LogLevel.Basic);
                stopwatch.Stop();
                Debug.LogLine($"[Uninstall] Uninstall took {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogLine("[Uninstall] Exception: " + e, Debug.LogLevel.Basic);
                stopwatch.Stop();
                Debug.LogLine($"[Uninstall] Uninstall failed at {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
                MessageBox.Show("An error has occurred during the uninstallation process and SnakeBite could not uninstall the selected mod(s).\nException: " + e);

                // clean up failed files
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
        }//UninstallMod batch

        private static void UninstallMods(List<ModEntry> uninstallMods, ref List<string> zeroFiles, ref List<string> oneFilesList)
        {
            Debug.LogLine(String.Format("[Uninstall] Bulk uninstall started"), Debug.LogLevel.Basic);
            List<string> fullRemoveQarPaths; // for qar files that can be removed altogether
            List<ModQarEntry> partialEditQarEntries; // for qar files that need to be rebuilt using base archives
            List<ModFpkEntry> partialRemoveFpkEntries; // of the qar files that need to be rebuilt, these are the fpk files that will be removed from them

            Debug.LogLine("[Uninstall] Building fpk removal lists", Debug.LogLevel.Basic);
            GetFpkRemovalLists(uninstallMods, out fullRemoveQarPaths, out partialEditQarEntries, out partialRemoveFpkEntries);

            Debug.LogLine("[Uninstall] Unmerging any fpk entries", Debug.LogLevel.Basic);
            UnmergePackFiles(partialEditQarEntries, partialRemoveFpkEntries);

            Debug.LogLine("[Uninstall] Removing any unmodified fpk entries", Debug.LogLevel.Basic);
            RemoveDemoddedQars(ref zeroFiles, fullRemoveQarPaths);

            GameData gameData = SBBuildManager.GetGameData();
            foreach (ModEntry uninstallMod in uninstallMods)
            {
                SBBuildManager.RemoveMod(uninstallMod);

                Debug.LogLine(string.Format("[Uninstall] Removing any game dir file entries for {0}", uninstallMod.Name), Debug.LogLevel.Basic);
                UninstallGameDirEntries(uninstallMod, ref gameData);

                Debug.LogLine(String.Format("[Uninstall] Removing any loose textures for {0}", uninstallMod.Name), Debug.LogLevel.Basic);
                UninstallLooseFtexs(uninstallMod, ref oneFilesList, ref gameData);
            }
            SBBuildManager.SetGameData(gameData);
        }

        private static void GetFpkRemovalLists(List<ModEntry> uninstallMods, out List<string> fullRemoveQarPaths, out List<ModQarEntry> partialEditQarEntries, out List<ModFpkEntry> partialEditFpkEntries)
        {
            // add all of the to-be-uninstalled fpk entries to a list
            List<string> fpkFilesToUninstall = new List<string>();
            foreach (ModEntry modToUninstall in uninstallMods)
            {
                foreach (ModFpkEntry fpkToUninstall in modToUninstall.ModFpkEntries)
                {
                    fpkFilesToUninstall.Add(fpkToUninstall.FilePath + fpkToUninstall.FpkFile);
                }
            }

            List<string> remainingModQarPaths = new List<string>();
            foreach (ModEntry mod in SBBuildManager.GetInstalledMods())
            {
                if (uninstallMods.Any(e => e.Name == mod.Name)) continue;
                foreach (ModFpkEntry remainingFpk in mod.ModFpkEntries)
                {
                    string remainingFpkString = remainingFpk.FilePath + remainingFpk.FpkFile;
                    if (fpkFilesToUninstall.Any(entry => (entry == remainingFpkString)))
                    {
                        //Debug.LogLine("(found duplicate, not adding to remaining fpkfiles: " + remainingFpk.FilePath);
                        continue;
                    }

                    if (!remainingModQarPaths.Contains(remainingFpk.FpkFile))
                    {
                        //Debug.LogLine("(found in mods not marked for uninstallation) new qar added to remainingmodqarpaths: " + remainingFpk.FpkFile);
                        remainingModQarPaths.Add(remainingFpk.FpkFile);
                    }
                }
            }

            //Build lists for two categories of qar files:
            fullRemoveQarPaths = new List<string>(); // for files that can be removed altogether
            partialEditQarEntries = new List<ModQarEntry>(); // for files that need to be edited using base archives
            partialEditFpkEntries = new List<ModFpkEntry>(); // Packed files within partialEditQarEntries that either need to be repaired (overwritten by a vanilla file) or removed from the Qar

            GameData gameData = SBBuildManager.GetGameData();
            // loop through every to-be-uninstalled mod to sort each Qar file into one of the two categories
            foreach (ModEntry uninstallMod in uninstallMods)
            {
                foreach (ModQarEntry uninstallQarEntry in uninstallMod.ModQarEntries)
                {
                    string uninstallQarFilePath = uninstallQarEntry.FilePath;
                    if (partialEditQarEntries.Any(entry => entry.FilePath == uninstallQarFilePath) || fullRemoveQarPaths.Contains(uninstallQarFilePath)) continue; // a to-be-uninstalled mod has already categorised this file

                    if (!(uninstallQarFilePath.EndsWith(".fpk") || uninstallQarFilePath.EndsWith(".fpkd")))
                    {
                        fullRemoveQarPaths.Add(uninstallQarFilePath);
                        //Debug.LogLine("(file is not a .fpk or .fpkd) new fullRemoveQarPath: " + uninstallQarFilePath);
                        continue;
                    }

                    ModQarEntry existingGameQar = gameData.GameQarEntries.FirstOrDefault(entry => entry.FilePath == uninstallQarFilePath); // switches over to gameData QarEntries, since they know the vanilla archives of the original Qar file.
                    if (existingGameQar == null) existingGameQar = uninstallQarEntry; // Qar entries that aren't merged with vanilla qar entries (non-native fpk/fpkd files)

                    if (remainingModQarPaths.Contains(uninstallQarFilePath))
                    {
                        partialEditQarEntries.Add(existingGameQar);
                        //Debug.LogLine("(Qar contained in remaining mod qar paths)New PartialEditQarEntry: " + existingGameQar);
                    }  // the file is being used in a not-to-be-uninstalled mod
                    else
                    {
                        fullRemoveQarPaths.Add(uninstallQarFilePath);
                        //Debug.LogLine("(Qar NOT contained in remaining mod qar paths)New FullRemoveQarPaths " + uninstallQarFilePath);
                    } // the file can be discarded now that no mod(s) have files for it
                }
                // For the files categorised as partially affected by the uninstall, list their packed files which need to be uninstalled
                foreach (ModQarEntry partialEditQarEntry in partialEditQarEntries)
                    foreach (ModFpkEntry modFpkEntry in uninstallMod.ModFpkEntries)
                    {
                        //Debug.LogLine("(Nested Foreach loop) PartialEditQarEntry: " + partialEditQarEntry.FilePath);
                        if (modFpkEntry.FpkFile == partialEditQarEntry.FilePath)
                        {
                            modFpkEntry.FpkFile = modFpkEntry.FpkFile;
                            partialEditFpkEntries.Add(modFpkEntry);
                            Debug.LogLine(string.Format("[RemovalList] Fpk flagged for removal: {0}", modFpkEntry.FilePath), Debug.LogLevel.Basic);
                        }
                    }

            }
        }

        private static void UnmergePackFiles(List<ModQarEntry> partialEditQarEntries, List<ModFpkEntry> partialRemoveFpkEntries)
        {
            GameData gameData = SBBuildManager.GetGameData();
            List<ModFpkEntry> addedRepairFpkEntries = new List<ModFpkEntry>();

            foreach (ModQarEntry partialEditQarEntry in partialEditQarEntries)
            {
                // create a list of fpk filepaths that need to be modified for the specific qar file (either restored to vanilla or removed from the pack)
                List<string> fpkPathsForThisQar = partialRemoveFpkEntries.Where(entry => entry.FpkFile == partialEditQarEntry.FilePath).Select(fpkEntry => Tools.ToWinPath(fpkEntry.FilePath)).ToList();

                // pull the vanilla qar file from the game archive, send to _gameFpk folder
                string winQarEntryPath = Tools.ToWinPath(partialEditQarEntry.FilePath);
                string gameQarPath = Path.Combine("_gameFpk", winQarEntryPath);
                if (partialEditQarEntry.SourceName != null)
                {
                    string vanillaArchivePath = Path.Combine(GameDir, "master\\" + partialEditQarEntry.SourceName);
                    //Debug.LogLine(string.Format("Pulling {0} from {1}", partialRemoveQarEntry.FilePath, partialRemoveQarEntry.SourceName));
                    GzsLib.ExtractFileByHash<QarFile>(vanillaArchivePath, partialEditQarEntry.Hash, gameQarPath);
                }
                // pull the modded qar file from _working0 (assumed to already exist when the uninstall process reads 00.dat), send to _build folder
                string workingZeroQarPath = Path.Combine("_working0", winQarEntryPath);
                List<string> moddedFpkFiles = GzsLib.ExtractArchive<FpkFile>(workingZeroQarPath, "_build");

                // split the fpk paths for this Qar into two categories:
                List<string> repairFilePathList = new List<string>(); // files that need to be repaired (aka overwritten by a vanilla file)
                if (partialEditQarEntry.SourceName != null)
                    repairFilePathList = GzsLib.ListArchiveContents<FpkFile>(gameQarPath).Intersect(fpkPathsForThisQar).ToList();
                List<string> removeFilePathList = fpkPathsForThisQar.Except(repairFilePathList).ToList(); // files that need to be removed (i.e. files that were non-native to the vanilla Qar)

                foreach (string repairFilePath in repairFilePathList)
                {
                    string fpkBuildPath = Path.Combine("_build", repairFilePath);
                    Debug.LogLine(string.Format("[Unmerge Fpk] Extracting repair file: {0}", repairFilePath), Debug.LogLevel.Basic);
                    GzsLib.ExtractFile<FpkFile>(gameQarPath, repairFilePath, fpkBuildPath); // overwrites modded fpk files
                    ModFpkEntry repairEntry = new ModFpkEntry
                    {
                        FpkFile = partialEditQarEntry.FilePath,
                        FilePath = repairFilePath, // this will be a window path
                        SourceType = FileSource.Merged,
                        SourceName = partialEditQarEntry.SourceName
                    };
                    gameData.GameFpkEntries.Add(repairEntry);
                    addedRepairFpkEntries.Add(repairEntry);
                }

                var buildFiles = moddedFpkFiles.Except(removeFilePathList).ToList();
                GzsLib.WriteFpkArchive(workingZeroQarPath, "_build", buildFiles); // writes the pack back to _working folder (leaving out the non-native fpk files)
                foreach (string removeFilePath in removeFilePathList)
                {
                    int indexToRemove = gameData.GameFpkEntries.FindIndex(entry => entry.FilePath == removeFilePath);
                    if (indexToRemove >= 0) gameData.GameFpkEntries.RemoveAt(indexToRemove);
                }
            }

            List<ModEntry> installedMods = SBBuildManager.GetInstalledMods();
            foreach (ModEntry installedMod in installedMods)
            {
                List<string> qarPathsFound = new List<string>();
                foreach (ModFpkEntry addedRepairEntry in addedRepairFpkEntries)
                {
                    if (installedMod.ModQarEntries.FirstOrDefault(entry => entry.FilePath == addedRepairEntry.FpkFile) == null) continue;

                    //Debug.LogLine(string.Format("checking {0} for {1} of {2}", installedMod.Name, addedRepairEntry.FilePath, addedRepairEntry.FpkFile));
                    if (installedMod.ModFpkEntries.RemoveAll(entry => entry.FilePath == Tools.ToQarPath(addedRepairEntry.FilePath) && entry.FpkFile == addedRepairEntry.FpkFile) > 0)
                    {
                        //Debug.LogLine(string.Format("found {0} of {1} in {2}", addedRepairEntry.FilePath, addedRepairEntry.FpkFile, installedMod.Name));
                        if (!qarPathsFound.Contains(addedRepairEntry.FpkFile))
                            qarPathsFound.Add(addedRepairEntry.FpkFile);
                    }
                }

                foreach (string qarPathFound in qarPathsFound)
                {
                    if (installedMod.ModFpkEntries.FirstOrDefault(entry => entry.FpkFile == qarPathFound) == null) //when the duplicate fpk file(s) were removed, there was nothing left in the modded qar.
                    {
                        //Debug.LogLine(string.Format("Removing {0} from {1}", qarPathFound, installedMod.Name));
                        installedMod.ModQarEntries.RemoveAll(entry => entry.FilePath == qarPathFound); // filters the qar file out of the list
                    }
                }
            }
            SBBuildManager.SetInstalledMods(installedMods);
            SBBuildManager.SetGameData(gameData);
        }

        private static void RemoveDemoddedQars(ref List<string> zeroFiles, List<string> fullRemoveQarPaths)
        {
            // Remove the specified files from the mod ecosystem since they no longer contain modded data
            zeroFiles = zeroFiles.Except(fullRemoveQarPaths.Select(entry => Tools.ToWinPath(entry))).ToList();
            List<ModEntry> installedMods = SBBuildManager.GetInstalledMods();
            GameData gameData = SBBuildManager.GetGameData();
            foreach(string fullRemoveQarPath in fullRemoveQarPaths)
            {
                //Debug.LogLine(string.Format("DEMODDED QAR: {0}", fullRemoveQarPath), Debug.LogLevel.Basic);
                int indexToRemove = gameData.GameQarEntries.FindIndex(entry => entry.FilePath == fullRemoveQarPath);
                if (indexToRemove >= 0) gameData.GameQarEntries.RemoveAt(indexToRemove);
                gameData.GameFpkEntries = gameData.GameFpkEntries.Where(entry => entry.FpkFile != fullRemoveQarPath).ToList();
                foreach (ModEntry installedMod in installedMods)
                {
                    indexToRemove = installedMod.ModQarEntries.FindIndex(entry => entry.FilePath == fullRemoveQarPath);
                    if (indexToRemove >= 0) installedMod.ModQarEntries.RemoveAt(indexToRemove);
                    installedMod.ModFpkEntries = installedMod.ModFpkEntries.Where(entry => entry.FpkFile != fullRemoveQarPath).ToList();
                }
            }
            SBBuildManager.SetInstalledMods(installedMods);
            SBBuildManager.SetGameData(gameData);

        }

        private static void UninstallGameDirEntries(ModEntry mod, ref GameData gameData)
        {
            HashSet<string> fileEntryDirs = new HashSet<string>();
            foreach (ModFileEntry fileEntry in mod.ModFileEntries) //checks all of current mod's files
            {
                string destFile = Path.Combine(GameDirSB_Build, Tools.ToWinPath(fileEntry.FilePath)); //create the filepath to the file in question
                string dir = Path.GetDirectoryName(destFile); //filepath of the directory containing the file
                fileEntryDirs.Add(dir); //the directory is added to the list of fileentrydirectories
                if (File.Exists(destFile)) // attempt to delete the file in question
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
                gameData.GameFileEntries.RemoveAll(file => Tools.CompareHashes(file.FilePath, fileEntry.FilePath)); //remove all mentions of the destFile from snakebite.xml
            }//foreach ModFileEntries
            foreach (string fileEntryDir in fileEntryDirs) //all the directories that have had files deleted within them
            {
                if (Directory.Exists(fileEntryDir) && Directory.GetFiles(fileEntryDir).Length == 0) // if the directory has not yet been deleted and there are no more files inside the directory
                {
                    Debug.LogLine(String.Format("[Uninstall] deleting empty folder: {0}", fileEntryDir), Debug.LogLevel.All);
                    try
                    {
                        Directory.Delete(fileEntryDir); //attempt to delete the empty directory [NOT RECURSIVE]
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                    }
                }
            }//foreach fileEntryDirs
        }//UninstallGameDirEntries

        private static void UninstallLooseFtexs(ModEntry mod, ref List<string> oneFilesList, ref GameData gameData)
        {
            foreach (ModQarEntry qarEntry in mod.ModQarEntries) // check all qar entries in current mod
            {
                if (qarEntry.FilePath.Contains(".ftex"))
                { // if the file is an ftex or ftexs
                    string destFile = Path.Combine("_working1", qarEntry.FilePath);
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
        }//UninstallLooseFtexs
    }
}
