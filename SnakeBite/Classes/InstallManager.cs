using GzsTool.Core.Fpk;
using GzsTool.Core.Qar;
using ICSharpCode.SharpZipLib.Zip;
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

            // assume bad metadatas have been filtered out during preinstall management
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

            List<ModFpkEntry> fpkEntryRetrievalList;
            List<ModQarEntry> qarEntryEditList;
            AddToSettings(installEntryList, manager, allQarGameFiles, out fpkEntryRetrievalList, out qarEntryEditList);

            DoMerges(ModFiles, fpkEntryRetrievalList, qarEntryEditList);

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

        /// <summary>
        /// Merges the new mod files with the existing modded files and vanilla game files.
        /// The resulting file structure is the new _working0 folder to pack as 00.dat 
        /// </summary>
        private static void DoMerges(List<string> modFilePaths, List<ModFpkEntry> fpkEntryRetrievalList, List<ModQarEntry> qarEntryEditList)
        {
            //Assumption: modded packs have already been extracted to _working0 directory
            //Assumption: vanilla packs have already been extracted to _gameFpk directory (during AddToSettings)
            //theoretically there should be no qar overlap between the _gamefpk and _working0 files
            List<string> qarRetrievalFilePaths = new List<string>();
            foreach(ModFpkEntry fpkEntry in fpkEntryRetrievalList)
            {
                if (!qarRetrievalFilePaths.Contains(fpkEntry.FpkFile)) qarRetrievalFilePaths.Add(fpkEntry.FpkFile);
            }

            FastZip unzipper = new FastZip();
            foreach (string modFilePath in modFilePaths)
            {
                unzipper.ExtractZip(modFilePath, "_extr", "(.*?)");
                ModEntry extractedModEntry = new ModEntry("_extr\\metadata.xml");

                foreach(ModQarEntry modQarEntry in qarEntryEditList)
                {
                    if (extractedModEntry.ModQarEntries.FirstOrDefault(e => e.FilePath == modQarEntry.FilePath) == null) continue;


                }

                foreach(string qarFilePath in qarRetrievalFilePaths)
                {

                }

            }

        }

        private static void AddToSettings(List<ModEntry> installEntryList, SettingsManager manager, List<Dictionary<ulong, GameFile>> allQarGameFiles, out List<ModFpkEntry> repairFpkEntries, out List<ModQarEntry> existingModQarList)
        {
            GameData gameData = manager.GetGameData();

            List<string> newModQarEntries = new List<string>();
            List<string> modQarFiles = manager.GetModQarFiles();
            existingModQarList = new List<ModQarEntry>();

            foreach (ModEntry modToInstall in installEntryList)
            {
                manager.AddMod(modToInstall);
                foreach (ModQarEntry modQarEntry in modToInstall.ModQarEntries) // add qar entries (fpk, fpkd) to GameData if they don't already exist
                {
                    string modQarFilePath = modQarEntry.FilePath;
                    if (!(modQarFilePath.EndsWith(".fpk") || modQarFilePath.EndsWith(".fpkd")) || newModQarEntries.Contains(modQarFilePath)) continue;
                    if (existingModQarList.FirstOrDefault(e => e.Hash == modQarEntry.Hash) != null) continue;

                    if (gameData.GameQarEntries.FirstOrDefault(e => e.Hash == modQarEntry.Hash) == null)// the qar may not yet exist in the snakebite environment. 
                    {
                        if (modQarFiles.Contains(modQarEntry.FilePath)) //alternatively: (modQarFiles.FirstOrDefault(e => e == modQarEntry.FilePath) != null)
                            existingModQarList.Add(modQarEntry); // the qar is managed in 00.dat but not native to MGSV. this file will need to be merged with the existing 00.dat file. we can save these entries for later, so we know which existing packs need to be editted (they will not need repair files)
                        else
                            newModQarEntries.Add(modQarEntry.FilePath); // the program will need to pull repair files from the base archives(if they exist) and add them to the fpkentries gamedata list
                    }  
                    else existingModQarList.Add(modQarEntry); // the qar is managed in 00.dat and one or more mods are using the same qar file. 
                }

                foreach (ModFileEntry modDirFileEntry in modToInstall.ModFileEntries)
                {
                    ModFileEntry existingFileEntry = gameData.GameFileEntries.FirstOrDefault(e => e.FilePath == modDirFileEntry.FilePath);
                    if (existingFileEntry == null) gameData.GameFileEntries.Add(modDirFileEntry);
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
            repairFpkEntries = new List<ModFpkEntry>();
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
                            string workingPath = Path.Combine("_gameFpk", windowsFilePath);
                            GzsLib.ExtractFileByHash<QarFile>(sourceArchive, existingPack.FileHash, workingPath); // extracts the specific .fpk from the game data

                            string unpackDirName = string.Format("{0}\\{1}_{2}", Path.GetDirectoryName(workingPath), Path.GetFileNameWithoutExtension(workingPath), Path.GetExtension(workingPath).Remove(0, 1));
                            foreach (string extractedFile in GzsLib.ListArchiveContents<FpkFile>(workingPath))
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
