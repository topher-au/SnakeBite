using GzsTool.Core.Fpk;
using GzsTool.Core.Qar;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static SnakeBite.GamePaths;

namespace SnakeBite
{
    internal static class InstallManager
    {
        private static List<string> cleanupFolders = new List<string> {
            "_working0",
            "_working1",
            "_working2",//LEGACY
            "_extr",
            "_build",
            "_gameFpk",
            "_modfpk",
        };

        public static bool InstallMods(List<string> ModFiles, bool skipCleanup = false)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Debug.LogLine("[Install] Start", Debug.LogLevel.Basic);
            GzsLib.LoadDictionaries();
            List<ModEntry> installEntryList = new List<ModEntry>();

            // assume bad metadatas have been filtered out
            foreach(string modFile in ModFiles) installEntryList.Add(Tools.ReadMetaData(modFile));

            var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working0");
            List<string> oneFiles = null;
            bool hasFtexs = foundLooseFtexs(installEntryList); 
            if (hasFtexs)
            {
                oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_working1");
            }

            SettingsManager manager = new SettingsManager(SnakeBiteSettings + "test.xml");
            var gameData = manager.GetGameData(); 

            var zeroFilesHashSet = new HashSet<string>(zeroFiles);

            Debug.LogLine("[Install] Building gameFiles lists", Debug.LogLevel.Basic);
            var zeroGameFiles = GzsLib.GetQarGameFiles(ZeroPath);
            var baseGameFiles = GzsLib.ReadBaseData();

            var allQarGameFiles = new List<Dictionary<ulong, GameFile>>();
            allQarGameFiles.AddRange(baseGameFiles);
            AddToSettings(installEntryList, manager, allQarGameFiles);

            try
            {
                /*
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
                */
                stopwatch.Stop();
                Debug.LogLine($"[Install] Installation finished in {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
                return true;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogLine($"[Install] Installation failed at {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
                Debug.LogLine("[Install] Exception: " + e, Debug.LogLevel.Basic);
                return false;
            }
        }

        private static void AddToSettings(List<ModEntry> installEntryList, SettingsManager manager, List<Dictionary<ulong, GameFile>> allQarGameFiles)
        {
            GameData gameData = manager.GetGameData();
            List<string> newModQarEntries = new List<string>();

            foreach (ModEntry modToInstall in installEntryList)
            {
                manager.AddMod(modToInstall);
                foreach (ModQarEntry modQarEntry in modToInstall.ModQarEntries) // add qar entries (fpk, fpkd) to GameData if they don't already exist
                {
                    if (!(modQarEntry.FilePath.EndsWith(".fpk") || modQarEntry.FilePath.EndsWith(".fpkd"))) continue;

                    ModQarEntry existingModQarEntry = gameData.GameQarEntries.FirstOrDefault(e => e.FilePath == modQarEntry.FilePath);
                    if (existingModQarEntry == null) // the qar does not yet exist. the program will need to pull repair files from the base archives and add them to the fpkentries gamedata list
                    {
                        //Debug.LogLine(string.Format("new ModQarEntry: {0}, {1}", Tools.ToQarPath(modQarEntry.FilePath), modQarEntry.SourceName));
                        newModQarEntries.Add(modQarEntry.FilePath);
                    }
                    else { }// the qar already exists and two or more mods are using the same qar file. the fpk entries for this qar file are already listed in the modFpkEntries 
                }

                foreach (ModFileEntry modFileEntry in modToInstall.ModFileEntries)
                {
                    ModFileEntry existingFileEntry = gameData.GameFileEntries.FirstOrDefault(e => e.FilePath == modFileEntry.FilePath);
                    if (existingFileEntry == null) gameData.GameFileEntries.Add(modFileEntry);
                }
            }
            //Debug.LogLine(string.Format("Foreach nest 1 complete"));
            List<ModFpkEntry> newModFpkEntries = new List<ModFpkEntry>();
            foreach (ModEntry modToInstall in installEntryList)
            {
                foreach (ModFpkEntry modFpkEntry in modToInstall.ModFpkEntries)
                {
                    //Debug.LogLine(string.Format("Checking out {0} from {1}", modFpkEntry.FilePath, modFpkEntry.FpkFile));

                    if(newModQarEntries.Contains(modFpkEntry.FpkFile)) // if the entry's fpkfile is contained in the newmodqarentries, that means it isn't already part of the snakebite environment
                    {
                        //Debug.LogLine(string.Format("Adding repair files around {0}", modFpkEntry.FilePath));
                        newModFpkEntries.Add(modFpkEntry);
                    }
                    else
                    {
                        //Debug.LogLine(string.Format("Removing {0} from gameFpkEntries so it will only be listed in the mod's entries", modFpkEntry.FilePath));
                        int indexToRemove = gameData.GameFpkEntries.FindIndex(m => m.FilePath == modFpkEntry.FilePath); // this will remove the gamedata's listing of the file under fpkentries (repair entries), so the filepath will only be listed in the modentry
                        if (indexToRemove >= 0) gameData.GameFpkEntries.RemoveAt(indexToRemove);
                    }
                }
            }
            //Debug.LogLine(string.Format("Foreach nest 2 complete"));
            HashSet<ulong> mergeFpkHashes = new HashSet<ulong>();
            List<ModFpkEntry> repairFpkEntries = new List<ModFpkEntry>();
            foreach (ModFpkEntry newFpkEntry in newModFpkEntries) // this will add the fpkentry listings (repair entries) required to merge the modded files with the old game files.
            {
                //Debug.LogLine(string.Format("checking {0} for repairs", newFpkEntry.FilePath));
                ulong packHash = Tools.NameToHash(newFpkEntry.FpkFile);
                if (mergeFpkHashes.Contains(packHash)) continue; // the process has already plucked this particular fpk file and extracted its contents

                foreach (var archiveQarGameFiles in allQarGameFiles)
                {
                    //Debug.LogLine(string.Format("checking archive for an existing qar file"));
                    if (archiveQarGameFiles.Count > 0)
                    {
                        GameFile existingPack = null;
                        archiveQarGameFiles.TryGetValue(packHash, out existingPack); // checks every archive (except 00) to see if the particular fpk file already exists
                        if (existingPack != null) // the qar file exists 
                        {
                            //Debug.LogLine(string.Format("Qar file {0} found in {1}. adding to gameqarentries", existingPack.FilePath, existingPack.QarFile));
                            mergeFpkHashes.Add(packHash);
                            gameData.GameQarEntries.Add(new ModQarEntry{
                                FilePath = existingPack.FilePath,
                                SourceType = FileSource.Merged,
                                SourceName = existingPack.QarFile,
                                Hash = existingPack.FileHash
                            });

                            string windowsFilePath = Tools.ToWinPath(existingPack.FilePath);
                            // Create destination directory
                            string destDirectory = Path.Combine("test", Path.GetDirectoryName(windowsFilePath));
                            if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);
                            
                            string sourceArchive = Path.Combine(GameDir, "master\\" + existingPack.QarFile);
                            string workingPath = Path.Combine("test", windowsFilePath);
                            GzsLib.ExtractFileByHash<QarFile>(sourceArchive, existingPack.FileHash, workingPath); // extracts the specific .fpk from the game data

                            string unpackDirName = string.Format("{0}\\{1}_{2}", Path.GetDirectoryName(workingPath), Path.GetFileNameWithoutExtension(workingPath), Path.GetExtension(workingPath).Remove(0, 1));
                            foreach (string extractedFile in GzsLib.ExtractArchive<FpkFile>(workingPath, unpackDirName))  // extracts the filepaths inside the fpk
                            {
                                repairFpkEntries.Add(new ModFpkEntry {
                                    FpkFile = existingPack.FilePath,
                                    FilePath = extractedFile,
                                    SourceType = FileSource.Merged,
                                    SourceName = existingPack.QarFile
                                });
                                //Debug.LogLine(string.Format("File Extracted: {0}", extractedFile));
                            }
                            break;
                        }
                    }
                }
            }
            //Debug.LogLine(string.Format("Foreach nest 3 complete"));
            foreach (ModFpkEntry newFpkEntry in newModFpkEntries) // finally, strips away mod entries from repair entries
            {
                //Debug.LogLine(string.Format("checking to remove {0} from gamefpkentries", Tools.ToWinPath(newFpkEntry.FilePath)));
                int indexToRemove = repairFpkEntries.FindIndex(m => m.FilePath == Tools.ToWinPath(newFpkEntry.FilePath));
                if (indexToRemove >= 0) repairFpkEntries.RemoveAt(indexToRemove);
            }
            gameData.GameFpkEntries.AddRange(repairFpkEntries);

            manager.SetGameData(gameData);
        }

        public static bool foundLooseFtexs(List<ModEntry> EntryList) // returns true if any mods in the list contain a loose texture file which was installed to 01
        {
            foreach (ModEntry modfile in EntryList)
            {
                foreach (ModQarEntry qarFile in modfile.ModQarEntries)
                {
                    if (qarFile.FilePath.Contains(".ftex") || qarFile.FilePath.Contains(".ftexs"))
                        return true;
                }
            }
            return false;
        }
    }
}
