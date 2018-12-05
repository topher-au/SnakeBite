using GzsTool.Core.Fpk;
using GzsTool.Core.Qar;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.Forms;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SnakeBite
{
    internal static class ModManager
    {
        internal static string vanillaDatHash = "41317C4D473D9A3DB6C1169E5ACDD358B6905D4A97F8A9750B6CC2B97B6BE218"; //expected original hash for 1.0.15.0
        internal static Version IntendedGameVersion = new Version(1, 0, 15, 0); // GAMEVERSION
        internal static string SBWMSearchURL = "https://www.nexusmods.com/metalgearsolidvtpp/search/?search_description=SBWM";
        internal static string SBWMBugURL = "https://www.nexusmods.com/metalgearsolidvtpp/mods/106?tab=bugs";
        internal static string WikiURL = "https://metalgearmodding.wikia.com/wiki/Metal_Gear_Modding_Wiki";
        internal static string chunk0Path { get { return Properties.Settings.Default.InstallPath + "\\master\\chunk0.dat"; } }
        internal static string OnePath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat"; } }
        internal static string ZeroPath { get { return Properties.Settings.Default.InstallPath + "\\master\\0\\00.dat"; } }
        internal static string t7Path { get { return Properties.Settings.Default.InstallPath + "\\master\\a_texture7.dat"; } }
        internal static string c7Path { get { return Properties.Settings.Default.InstallPath + "\\master\\a_chunk7.dat"; } }
        internal static string GameDir { get { return Properties.Settings.Default.InstallPath; } }
        internal static string build_ext = ".SB_Build";


        // SYNC makebite
        static string ExternalDirName = "GameDir";

        public static bool InstallMod(List<string> ModFiles, bool skipCleanup = false) // Installs a list of mod filenames
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.LogLine("[Install] Start", Debug.LogLevel.Basic);
            CleanupFolders();
            GzsLib.LoadDictionaries();
            var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working0");       
            List<string> oneFiles = null;
            bool hasFtexs = foundLooseFtexs(ModFiles);
            if (hasFtexs)
            {
                oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_working1");
            }       
            //snakebites settings manager also includes the installed mod lists
            SettingsManager manager = new SettingsManager(GameDir);
            var gameData = manager.GetGameData();
            ValidateGameData(ref gameData, ref zeroFiles);
            var zeroFilesHashSet = new HashSet<string>(zeroFiles);
            //one list to rule them all, list per game .dat in priority order
            //does not include texture dats since we only care about archives to merge while non archive files in 00 will just override
            Debug.LogLine("[Install] Building gameFiles lists", Debug.LogLevel.Basic);
            var baseGameFiles = GzsLib.ReadBaseData();
            var zeroGameFiles = GzsLib.GetQarGameFiles(ZeroPath);
            var qarGameFiles = new List<Dictionary<ulong, GameFile>>();
            qarGameFiles.Add(zeroGameFiles);
            qarGameFiles.AddRange(baseGameFiles);
            foreach (string modfilePath in ModFiles)
            {
                InstallMod(modfilePath, manager, ref qarGameFiles, ref zeroFilesHashSet, ref oneFiles);
            }
            Debug.LogLine("[Install] Rebuilding 00.dat", Debug.LogLevel.Basic);
            zeroFiles = zeroFilesHashSet.ToList();
            zeroFiles.Sort();
            GzsLib.WriteQarArchive(ZeroPath, "_working0", zeroFiles, GzsLib.zeroFlags);
            if (hasFtexs)
            {
                Debug.LogLine("[Install] Rebuilding 01.dat", Debug.LogLevel.Basic);
                oneFiles.Sort();
                GzsLib.WriteQarArchive(OnePath, "_working1", oneFiles, GzsLib.oneFlags);
            }
            if (!skipCleanup)
            {
                //DEBUGNOW this takes a big chunk of time, see what of it is absolutenly nessesary 
                //DEBUGNOW    CleanupDatabase();
            }
            if (!skipCleanup)
            {
                CleanupFolders();
            }
            manager.UpdateDatHash();
            stopwatch.Stop();
            Debug.LogLine($"[Install] Installation finished in {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
            return true;
        }

        //Cull any invalid entries that might have slipped in via older versions of snakebite
        private static void ValidateGameData(ref GameData gameData, ref List<string> zeroFiles)
        {
            Debug.LogLine("[ValidateGameData] Validating gameData files", Debug.LogLevel.Basic);
            Debug.LogLine("[ValidateGameData] Validating qar entries", Debug.LogLevel.Basic);
            for (int i = gameData.GameQarEntries.Count-1; i >= 0; i--)
            {
                ModQarEntry qarEntry = gameData.GameQarEntries[i];
                if (!GzsLib.IsExtensionValidForArchive(qarEntry.FilePath, ".dat"))
                {
                    Debug.LogLine($"[ValidateGameData] Found invalid file entry {qarEntry.FilePath} for archive {qarEntry.SourceName}", Debug.LogLevel.Basic);
                    gameData.GameQarEntries.RemoveAt(i);
                    zeroFiles.Remove(qarEntry.FilePath);//DEBUGNOW VERIFY
                }
            }
            Debug.LogLine("[ValidateGameData] Validating fpk entries", Debug.LogLevel.Basic);
            for (int i = gameData.GameFpkEntries.Count-1; i >= 0; i--)
            {
                ModFpkEntry fpkEntry = gameData.GameFpkEntries[i];
                if (!GzsLib.IsExtensionValidForArchive(fpkEntry.FilePath, fpkEntry.FpkFile))
                {
                    Debug.LogLine($"[ValidateGameData] Found invalid file entry {fpkEntry.FilePath} for archive {fpkEntry.FpkFile}", Debug.LogLevel.Basic);
                    gameData.GameFpkEntries.RemoveAt(i);
                }
            }
        }

        private static void InstallMod(string modfilePath, SettingsManager manager, ref List<Dictionary<ulong, GameFile>> qarGameFiles, ref HashSet<string> zeroFiles, ref List<string> oneFilesList)
        {
            Debug.LogLine($"[Install] Installation started: {modfilePath}", Debug.LogLevel.Basic);
            Debug.LogLine("[Install] Unzip mod .mgsv", Debug.LogLevel.Basic);
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(modfilePath, "_extr", "(.*?)");
            Debug.LogLine("[Install] Load mod metadata", Debug.LogLevel.Basic);
            ModEntry modEntry = new ModEntry("_extr\\metadata.xml");
            GzsLib.LoadModDictionary(modEntry);
            ValidateModEntries(ref modEntry);
            //DEBUGNOW
            //If file was packed by makebite as a hashed filename and the current dictionary has the full name then it will have issues when merging fpks
            foreach (var entry in modEntry.ModQarEntries)
            {
                //need to do some hashedrename stuff, but gzstool.hashing doesnt expose its dictionary check function which is a drag
                //will also have to think through whether I'll have to update the mod metadata too
                //could pare things down to: only mergefpks and files that are in 00 
                //(or have some other mechanism to make sure you dont have both an unhashed and hashed version packed)
            }
            var gameData = manager.GetGameData();
            Debug.LogLine("[Install] Check mod FPKs against game .dat fpks", Debug.LogLevel.Basic);
            HashSet<ulong> mergeFpkHashes;//used as a fast isInMergeFpks
            List<ModQarEntry> mergeFpks = GetMergeFpks(modEntry, ref qarGameFiles, ref zeroFiles, out mergeFpkHashes);
            Debug.LogLine($"[Install] Merging {mergeFpks.Count} FPK files", Debug.LogLevel.Basic);
            foreach (ModQarEntry fpkQarEntry in mergeFpks)
            {
                MergeFpk(fpkQarEntry, ref gameData, ref zeroFiles);
            }
            //move external files to game directory
            Debug.LogLine("[Install] Copying game dir files", Debug.LogLevel.Basic);
            InstallGameDirFiles(modEntry, ref gameData);
            manager.SetGameData(gameData);
            // copy loose texture files to 01.dat
            Debug.LogLine("[Install] Copying loose textures to 01.", Debug.LogLevel.Basic);
            InstallLooseFtexs(modEntry, ref oneFilesList);
            // Copy (non-texture) files for 00.dat, ignoring merged FPKs
            Debug.LogLine("[Install] Copying remaining mod files", Debug.LogLevel.Basic);
            InstallModFiles(modEntry, mergeFpkHashes, ref zeroFiles);
            modEntry = new ModEntry("_extr\\metadata.xml");
            manager.AddMod(modEntry);
        }

        private static void ValidateModEntries(ref ModEntry modEntry)
        {
            Debug.LogLine("[ValidateModEntries] Validating qar entries", Debug.LogLevel.Basic);
            for (int i = modEntry.ModQarEntries.Count-1; i >= 0; i--)
            {
                ModQarEntry qarEntry = modEntry.ModQarEntries[i];
                if (!GzsLib.IsExtensionValidForArchive(qarEntry.FilePath, ".dat"))
                {
                    Debug.LogLine($"[ValidateModEntries] Found invalid file entry {qarEntry.FilePath} for archive {qarEntry.SourceName}", Debug.LogLevel.Basic);
                    modEntry.ModQarEntries.RemoveAt(i);
                }
            }
            Debug.LogLine("[ValidateModEntries] Validating fpk entries", Debug.LogLevel.Basic);
            for (int i = modEntry.ModFpkEntries.Count-1; i >= 0; i--)
            {
                ModFpkEntry fpkEntry = modEntry.ModFpkEntries[i];
                if (!GzsLib.IsExtensionValidForArchive(fpkEntry.FilePath, fpkEntry.FpkFile))
                {
                    Debug.LogLine($"[ValidateModEntries] Found invalid file entry {fpkEntry.FilePath} for archive {fpkEntry.FpkFile}", Debug.LogLevel.Basic);
                    modEntry.ModFpkEntries.RemoveAt(i);
                }
            }
        }

        private static List<ModQarEntry> GetMergeFpks(ModEntry modEntry, ref List<Dictionary<ulong, GameFile>> qarGameFiles, ref HashSet<string> zeroFiles, out HashSet<ulong> mergeFpkHashes)
        {
            List<ModQarEntry> mergeFpks = new List<ModQarEntry>();
            mergeFpkHashes = new HashSet<ulong>();
            foreach (ModFpkEntry fpkEntry in modEntry.ModFpkEntries)
            {
                ulong fpkHash = Tools.NameToHash(fpkEntry.FpkFile);
                if (mergeFpkHashes.Contains(fpkHash)) continue;
                foreach (var gameFiles in qarGameFiles)
                {
                    if (gameFiles.Count > 0)
                    {
                        GameFile existingFpk = null;
                        gameFiles.TryGetValue(Tools.NameToHash(fpkEntry.FpkFile), out existingFpk);
                        if (existingFpk != null)
                        {
                            // Create destination directory
                            string destDirectory = Path.Combine("_working0", Path.GetDirectoryName(Tools.ToWinPath(existingFpk.FilePath)));
                            if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);

                            // Extract file into dat directory
                            string sourceArchive = Path.Combine(GameDir, "master\\" + existingFpk.QarFile);
                            if (existingFpk.QarFile=="00.dat")
                            {
                                sourceArchive = Path.Combine(GameDir, "master\\0\\" + existingFpk.QarFile);
                            }
                            string outputPath = Path.Combine("_working0", Tools.ToWinPath(existingFpk.FilePath));
                            var ex = GzsLib.ExtractFileByHash<QarFile>(sourceArchive, existingFpk.FileHash, outputPath);

                            mergeFpkHashes.Add(fpkHash);
                            mergeFpks.Add(new ModQarEntry() {
                                FilePath = existingFpk.FilePath,
                                SourceType = FileSource.Merged,
                                SourceName = existingFpk.QarFile,
                                Hash = existingFpk.FileHash
                            });

                            //Update 00 gamefiles for multiple mod install
                            if (existingFpk.QarFile != "00.dat")
                            {
                                GameFile gameFile = null;
                                if (!qarGameFiles[0].TryGetValue(existingFpk.FileHash, out gameFile))
                                {
                                    qarGameFiles[0][existingFpk.FileHash] = existingFpk;
                                    gameFile = existingFpk;
                                }
                                gameFile.QarFile = "00.dat";
                            }

                            zeroFiles.Add(Tools.ToWinPath(existingFpk.FilePath));
                            break;
                        }
                    }//if gameFiles > 0
                }//foreach dat gamefiles
            }//foreach modfpkentries
            return mergeFpks;
        }//GetMergeFpks

        /// <summary>
        /// Merge mod fpk with existing fpk
        /// </summary>
        private static void MergeFpk(ModQarEntry fpkQarEntry, ref GameData gameData, ref HashSet<string> zeroFiles)
        {
            Debug.LogLine($"[MergeFpk] Starting merge: {fpkQarEntry.FilePath} ({fpkQarEntry.SourceName})", Debug.LogLevel.Debug);
            // Extract game FPK //ASSUMPTION: fpk exists in _working0 from a prior step.
            string gameFpkPath = Path.Combine("_working0", Tools.ToWinPath(fpkQarEntry.FilePath));
            var gameFpk = GzsLib.ExtractArchive<FpkFile>(gameFpkPath, "_gamefpk");
            // Extract mod FPK //ASSUMPTION: fpk exists in _extr from a prior step.
            string modFpkPath = Path.Combine("_extr", Tools.ToWinPath(fpkQarEntry.FilePath));
            var modFpk = GzsLib.ExtractArchive<FpkFile>(modFpkPath, "_modfpk");
            // Add FPK to gamedata info
            var existingModQarEntry = gameData.GameQarEntries.FirstOrDefault(entry => entry.FilePath == fpkQarEntry.FilePath);
            if (existingModQarEntry == null)
            {
                gameData.GameQarEntries.Add(new ModQarEntry() {
                    FilePath = Tools.ToQarPath(fpkQarEntry.FilePath),
                    SourceType = fpkQarEntry.SourceType,
                    SourceName = fpkQarEntry.SourceName,
                    Hash = fpkQarEntry.Hash
                });
            }
            //must for loop in case we have to remove invalid files
            for (int i=gameFpk.Count-1; i >= 0; i--)
            {
                string fileName = gameFpk[i];
                var c = modFpk.FirstOrDefault(entry => Tools.CompareHashes(entry, fileName));
                if (c == null)
                {
                    if (gameData.GameFpkEntries.FirstOrDefault(entry => Tools.CompareNames(entry.FilePath, fileName) && Tools.NameToHash(entry.FpkFile) == fpkQarEntry.Hash) == null)
                    {
                        //invalid files may have slipped through from old makebite/before it checked them (not an issue for base files, but fpkQarEntry is not just base files/may be a 00.dat modded fpk).
                        //GOTCHA: this is only catching additions to merged fpks, new fpks are just outright copied (in a later step) (which is ok, and good for perf) so will not be checked.
                        //and it wont catch existing invalid files in a prior modded fpks (from a prior mod install)
                        if (!GzsLib.IsExtensionValidForArchive(fileName, fpkQarEntry.FilePath))
                        {
                            Debug.LogLine($"[MergeFpk] Found invalid file {fileName} in {fpkQarEntry.FilePath}", Debug.LogLevel.Debug);
                            gameFpk.RemoveAt(i);
                            continue;
                        }
                        gameData.GameFpkEntries.Add(new ModFpkEntry() {
                            FpkFile = Tools.ToQarPath(fpkQarEntry.FilePath),
                            FilePath = fileName,
                            SourceType = fpkQarEntry.SourceType,
                            SourceName = fpkQarEntry.SourceName,
                        });
                    }
                }
            }//foreach file in gameFpk
            // Merge contents
            foreach (string fileName in modFpk)
            {
                string fileDir = (Path.Combine("_gamefpk", Path.GetDirectoryName(fileName)));
                string sourceFile = Path.Combine("_modfpk", fileName);
                string destFile = Path.Combine("_gamefpk", fileName);

                if (!GzsLib.IsExtensionValidForArchive(fileName, modFpkPath))
                {
                    Debug.LogLine($"[MergeFpk] Found invalid file {fileName} in {modFpkPath}", Debug.LogLevel.Debug);
                    continue;
                }
                Debug.LogLine($"[MergeFpk] Copying file: {fileName}", Debug.LogLevel.All);
                if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
                File.Copy(sourceFile, destFile, true);
                if (!gameFpk.Contains(fileName)) gameFpk.Add(Tools.ToQarPath(fileName));
            }//foreach file in modFpk
            var fpkType = Path.GetExtension(gameFpkPath).TrimStart('.');
            gameFpk = GzsLib.SortFpksFiles(fpkType, gameFpk);
            // Rebuild game FPK
            GzsLib.WriteFpkArchive(gameFpkPath, "_gamefpk", gameFpk);
            zeroFiles.Add(Tools.ToWinPath(fpkQarEntry.FilePath));
            try
            {
                Directory.Delete("_modfpk", true);
                Directory.Delete("_gamefpk", true);
            } catch (IOException e)
            {
                Console.WriteLine("[MergeFpk] Could not delete: " + e.Message);
            }
            Debug.LogLine(String.Format("[MergeFpk] Merge complete"), Debug.LogLevel.Debug);
        }//MergeFpk

        /// <summary>
        /// Copies installing mod files from _extr (from a prior step) to _working0 (which 00.dat will be built from, using zeroFiles list)
        /// skips loose ftex and fpk/ds
        /// adds to zeroFiles list.
        /// i/o: _extr to _working0
        /// </summary>
        /// <param name="modEntry">Of mod being installed</param>
        /// <param name="mergeFpkHashes">List of fpk/ds (as hash)</param>
        /// <param name="zeroFiles">current zeroFiles list, mod files are added</param>
        private static void InstallModFiles(ModEntry modEntry, HashSet<ulong> mergeFpkHashes, ref HashSet<string> zeroFiles)
        {
            foreach (ModQarEntry modQarEntry in modEntry.ModQarEntries)
            {
                //loose ftex don't go in 00.
                if (modQarEntry.FilePath.Contains(".ftex")) continue;
                //already done by mergefpk
                if (mergeFpkHashes.Contains(modQarEntry.Hash)) continue;
                zeroFiles.Add(Tools.ToWinPath(modQarEntry.FilePath));
                string sourceFile = Path.Combine("_extr", Tools.ToWinPath(modQarEntry.FilePath));
                string destFile = Path.Combine("_working0", Tools.ToWinPath(modQarEntry.FilePath));
                string destDir = Path.GetDirectoryName(destFile);
                Debug.LogLine(String.Format("[Install] Copying file: {0}", modQarEntry.FilePath), Debug.LogLevel.All);
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                File.Copy(sourceFile, destFile, true);
            }//foreach modQarEntry
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
                    string sourceFile = Path.Combine("_extr", ExternalDirName, Tools.ToWinPath(fileEntry.FilePath));
                    string destFile = Path.Combine(GameDir, Tools.ToWinPath(fileEntry.FilePath));
                    Debug.LogLine(String.Format("[Install] Copying file: {0}", destFile), Debug.LogLevel.All);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(sourceFile, destFile, true);
                    gameData.GameFileEntries.Add(fileEntry);
                }
            }
        }//InstallGameDirFiles

        public static bool UninstallMod(CheckedListBox.CheckedIndexCollection modIndices) // Uninstalls mods based on their indices in the list
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.LogLine("[Uninstall] Start", Debug.LogLevel.Basic);
            CleanupFolders();
            GzsLib.LoadDictionaries();
            SettingsManager manager = new SettingsManager(GameDir);
            List<ModEntry> mods = manager.GetInstalledMods();
            List<ModEntry> uninstallMods = new List<ModEntry>();
            foreach (int index in modIndices)
            {
                ModEntry mod = mods[index];
                uninstallMods.Add(mod);
            }
            Debug.LogLine("[Uninstall] Extracting 00.dat to _working0", Debug.LogLevel.Basic);
            var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working0"); // extracts 00.dat and creates a list of filenames, which is pruned throughout the uninstall process and repacked at the end.
            List<string> oneFiles = null;
            bool hasFtexs = foundLooseFtexs(uninstallMods);
            if (hasFtexs)
            {
                oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_working1"); // if necessary, extracts 01.dat and creates a list of filenames similar to zeroFiles. only textures are pruned from the list.
            }
            //end of qar extraction
            GameData gameData = manager.GetGameData();
            ValidateGameData(ref gameData, ref zeroFiles);
            Debug.LogLine("[Install] Building gameFiles lists", Debug.LogLevel.Basic);
            var baseGameFiles = GzsLib.ReadBaseData();
            foreach (ModEntry mod in uninstallMods)
            {
                UninstallMod(mod, manager, baseGameFiles, ref zeroFiles, ref oneFiles);
            }
            Debug.LogLine("[Uninstall] Rebuilding 00.dat", Debug.LogLevel.Basic);
            zeroFiles.Sort();
            GzsLib.WriteQarArchive(ZeroPath, "_working0", zeroFiles, GzsLib.zeroFlags);
            if (hasFtexs)
            {
                Debug.LogLine("[Install] Rebuilding 01.dat", Debug.LogLevel.Basic);
                oneFiles.Sort();
                GzsLib.WriteQarArchive(OnePath, "_working1", oneFiles, GzsLib.oneFlags);
            }
            // end of qar repacking
            Debug.LogLine("[Uninstall] Updating 00.dat hash", Debug.LogLevel.Basic);
            manager.UpdateDatHash();
            foreach (ModEntry mod in uninstallMods)
            {
                Debug.LogLine("[Uninstall] Removing mod entry from snakebite.xml", Debug.LogLevel.Basic);
                manager.RemoveMod(mod);
            }
            //DEBUGNOW CleanupDatabase();
            //Debug.LogLine("CleanupFolders", Debug.LogLevel.Basic); //allready logs
            CleanupFolders();
            Debug.LogLine("[Uninstall] Uninstall complete", Debug.LogLevel.Basic);
            stopwatch.Stop();
            Debug.LogLine($"[Uninstall] Uninstall took {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
            return true;
        }//UninstallMod batch

        private static void UninstallMod(ModEntry modEntry, SettingsManager manager, List<Dictionary<ulong, GameFile>> baseGameFiles, ref List<string> zeroFiles, ref List<string> oneFilesList)
        {
            Debug.LogLine(String.Format("[Uninstall] Uninstall started: {0}", modEntry.Name), Debug.LogLevel.Basic);
            Debug.LogLine("[Uninstall] Reading snakebite.xml", Debug.LogLevel.Basic);
            GameData gameData = manager.GetGameData(); //retrieves snakebite.xml information for lists of current installed
            Debug.LogLine("[Uninstall] Removing game dir file entries", Debug.LogLevel.Basic);
            UninstallGameDirEntries(modEntry, gameData);
            Debug.LogLine(String.Format("[Uninstall] Removing any loose textures in {0}", modEntry.Name), Debug.LogLevel.Basic); // begin loose texture check for current mod.
            UninstallLooseFtexs(modEntry, oneFilesList, gameData);
            Debug.LogLine("[Uninstall] Building list of fpks in mod", Debug.LogLevel.Basic);
            HashSet<string> modFpks = GetModFpks(modEntry);
            Debug.LogLine("[Uninstall] Processing fpk entries", Debug.LogLevel.Basic);
            foreach (string fpk in modFpks)
            {
                UninstallFpk(modEntry, fpk, baseGameFiles, ref gameData, ref zeroFiles);
            }
            //write out snakebite.xml, at this point the mods qar and fpk entries should have been removed, but the mods modentry is still there
            Debug.LogLine("[Uninstall] Saving snakebite.xml", Debug.LogLevel.Debug);
            manager.SetGameData(gameData);
            Debug.LogLine("[Uninstall] Remove all mod files from 00.dat files list", Debug.LogLevel.Debug);
            foreach (ModQarEntry qarEntry in modEntry.ModQarEntries)
            {
                string fExt = Path.GetExtension(qarEntry.FilePath);
                if (!fExt.Contains(".fpk"))
                {
                    zeroFiles.RemoveAll(file => Tools.CompareHashes(file, qarEntry.FilePath));
                }
            }
        }//UninstallMod

        private static HashSet<string> GetModFpks(ModEntry mod)
        {
            HashSet<string> modFpks = new HashSet<string>();
            foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
            {
                modFpks.Add(fpkEntry.FpkFile);
            }
            //tex: the above wont catch empty fpks (fpkds require a fpk, which can be empty)
            foreach (ModQarEntry fpkEntry in mod.ModQarEntries)
            {
                if (fpkEntry.FilePath.Contains(".fpk"))
                {
                    modFpks.Add(fpkEntry.FilePath);//modfkps now has every fpk file and filepath for the current mod
                }
            }
            return modFpks;
        }

        private static void UninstallFpk(ModEntry modEntry, string fpk, List<Dictionary<ulong, GameFile>> baseGameFiles, ref GameData gameData, ref List<string> zeroFiles)
        {
            Debug.LogLine($"[Uninstall] Processing {fpk}", Debug.LogLevel.Basic);
            //Extract fpk/d
            string fpkName = Path.GetFileName(fpk);
            string fpkDatPath = zeroFiles.FirstOrDefault(file => Tools.CompareHashes(file, fpk));//NMC internal path
            if (fpkDatPath == null) return;
            // TODO: (on install) mark fpks that soley belong to a mod/arent merged from others so it can be just removed without extracting and checking each file. 
            List<string> fpkFiles = GzsLib.ExtractArchive<FpkFile>(Path.Combine("_working0", Tools.ToWinPath(fpkDatPath)), "_modfpk");
            // Remove all mod fpk files from fpkFiles
            foreach (ModFpkEntry fpkEntry in modEntry.ModFpkEntries)
            {
                //tex OFF logspam Debug.LogLine(String.Format("[Uninstall] Removing {1}\\{0}", Tools.ToWinPath(fpkEntry.FilePath), fpkName), Debug.LogLevel.Debug);
                fpkFiles.RemoveAll(file => Tools.ToQarPath(file) == Tools.ToQarPath(fpkEntry.FilePath));
            }
            var gameFpks = gameData.GameFpkEntries.ToList();
            // Remove all merged files from fpkFiles and gameData list
            foreach (ModFpkEntry gameFpkFile in gameFpks)
            {
                if (Tools.ToQarPath(gameFpkFile.FpkFile) == Tools.ToQarPath(fpk) && gameFpkFile.SourceType == FileSource.Merged)
                {
                    // OFF logspam Debug.LogLine(String.Format("[Uninstall] Removing merged file {0}", gameFpkFile.FilePath));
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
            } else
            {
                Debug.LogLine(String.Format("[Uninstall] Rebuilding {0}", fpk), Debug.LogLevel.Debug);
                // rebuild fpk from base file
                foreach (var gameFiles in baseGameFiles)
                {
                    if (gameFiles.Count > 0)
                    {
                        GameFile baseFpk = null;
                        gameFiles.TryGetValue(Tools.NameToHash(fpk), out baseFpk);
                        if (baseFpk != null)
                        {
                            // Extract base FPK files
                            GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\" + baseFpk.QarFile), baseFpk.FileHash, "_working\\temp.fpk");
                            var rebuildFpkFiles = GzsLib.ExtractArchive<FpkFile>("_working\\temp.fpk", "_gamefpk");

                            // Add merged base files to game file database
                            var mCount = 0;
                            foreach (var fileFpkPath in rebuildFpkFiles)
                            {
                                if (!fpkFiles.Contains(fileFpkPath))
                                {
                                    gameData.GameFpkEntries.Add(new ModFpkEntry() {
                                        FpkFile = fpk,
                                        FilePath = fileFpkPath,
                                        SourceType = FileSource.Merged,
                                        SourceName = baseFpk.QarFile
                                    });
                                    mCount++;
                                }
                            }
                            Debug.LogLine(String.Format("[Uninstall] {0} files to restore from {1}", mCount, baseFpk.QarFile), Debug.LogLevel.Debug);
                            // Copy remaining files over base FPK
                            foreach (string mFile in fpkFiles)
                            {
                                string fDir = Path.GetDirectoryName(mFile);
                                if (!Directory.Exists(Path.Combine("_gamefpk", fDir))) Directory.CreateDirectory(Path.Combine("_gamefpk", fDir));
                                Debug.LogLine(String.Format("[Uninstall] Merging existing file: {0}", mFile));
                                File.Copy(Path.Combine("_modfpk", mFile), Path.Combine(Path.Combine("_gamefpk", mFile)), true);
                                if (!rebuildFpkFiles.Contains(mFile)) rebuildFpkFiles.Add(mFile);
                            }
                            var fpkType = Path.GetExtension(fpk).TrimStart('.');
                            rebuildFpkFiles = GzsLib.SortFpksFiles(fpkType, rebuildFpkFiles);
                            // Rebuild FPK
                            GzsLib.WriteFpkArchive(Path.Combine("_working0", Tools.ToWinPath(fpk)), "_gamefpk", rebuildFpkFiles);
                            try
                            {
                                Directory.Delete("_modfpk", true);
                                Directory.Delete("_gamefpk", true);
                            } catch (IOException e)
                            {
                                Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                            }
                        }// if file in basefiles !null
                    }//if baseGameFiles.Count > 0
                }//foreach baseGameFiles
            }//if fpkFiles.Count
        }

        private static void UninstallLooseFtexs(ModEntry mod, List<string> oneFilesList, GameData gameData)
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
                        } catch (IOException e)
                        {
                            Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                        }
                    }
                    gameData.GameQarEntries.RemoveAll(file => Tools.CompareHashes(file.FilePath, qarEntry.FilePath)); //remove all mentions of the deleted texture from snakebite.xml
                    oneFilesList.RemoveAll(file => Tools.CompareHashes(file, qarEntry.FilePath)); // removes all mentions of deleted texture from 01.dat's repack list
                }
            }
        }//UninstallLooseFtexs

        private static void UninstallGameDirEntries(ModEntry mod, GameData gameData)
        {
            HashSet<string> fileEntryDirs = new HashSet<string>();
            foreach (ModFileEntry fileEntry in mod.ModFileEntries) //checks all of current mod's files
            {
                string destFile = Path.Combine(GameDir, Tools.ToWinPath(fileEntry.FilePath)); //create the filepath to the file in question
                string dir = Path.GetDirectoryName(destFile); //filepath of the directory containing the file
                fileEntryDirs.Add(dir); //the directory is added to the list of fileentrydirectories
                if (File.Exists(destFile)) // attempt to delete the file in question
                {
                    Debug.LogLine(String.Format("[Uninstall] deleting file: {0}", destFile), Debug.LogLevel.All);
                    try
                    {
                        File.Delete(destFile); // deletes the specified file
                    } catch (IOException e)
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
                    Debug.LogLine(String.Format("[Uninstall] deleting folder: {0}", fileEntryDir), Debug.LogLevel.All);
                    try
                    {
                        Directory.Delete(fileEntryDir, true); //attempt to delete the empty directory
                    } catch (IOException e)
                    {
                        Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                    }
                }
            }//foreach fileEntryDirs
        }//UninstallGameDirEntries

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

        public static bool foundLooseFtexs(List<ModEntry> checkMods) // returns true if any mods in the list contain a loose texture file which was installed to 01
        {
            foreach (ModEntry mod in checkMods)
            {
                foreach (ModQarEntry qarFile in mod.ModQarEntries)
                {
                    if (qarFile.FilePath.Contains(".ftex"))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// move vanilla 00 files to 01, moves vanilla 01 textures to texture7, cleans snakebite.xml 
        /// as DoWorkEventHandler
        /// </summary>
        public static void backgroundWorker_MergeAndCleanup(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker mergeProcessor = (BackgroundWorker)sender;
            GzsLib.LoadDictionaries();

            mergeProcessor.ReportProgress(0, "Moving files into new archives");
            if (!MoveDatFiles()) //moves vanilla 00 files into 01, excluding foxpatch. 
            {
                    e.Cancel = true;
                    ClearBuildArchives();
                    return;
            }

            mergeProcessor.ReportProgress(0, "Modfying foxfs in chunk0");
            if (!ModifyFoxfs()) // adds lines to foxfs in chunk0.
            {
                    e.Cancel = true;
                    ClearBuildArchives();
                    return;
            }

            mergeProcessor.ReportProgress(0, "Promoting new archives");
            PromoteBuildArchives(); // overwrites existing archives with modified archives

            mergeProcessor.ReportProgress(0, "Cleaning up database");
            CleanupDatabase();
        }

        public static bool MoveDatFiles() // moves all vanilla 00.dat files, excluding foxpatch.dat, to 01.dat
        {
            SettingsManager manager = new SettingsManager(GameDir);
            bool goodChunkSetup = false; // proper chunk7 (filesize appears sufficient)
            bool goodTexSetup = false; // proper texture7 (filesize appears sufficient)
            bool promptContinue = false; // prompt shown upon apparently improper filesizes, but user may wish to continue anyway

            CleanupFolders();
            Debug.LogLine("[DatMerge] Beginning to move files to new archives");
            try
            {
                if (manager.IsVanilla0001DatHash())
                {   // first time setup or files have been revalidated
                    MoveDatFilesClean(manager);
                }
                else
                {   // the "uncertainty" case.
                    MoveDatFilesDirty(manager);
                }

                if (File.Exists(c7Path + build_ext)) // check if chunk7/texture7 is at least the proper filesize to run the game, warn the player if otherwise.
                    if (new System.IO.FileInfo(c7Path + build_ext).Length < 345000000)
                    {
                        MessageBox.Show("SnakeBite has detected that a_chunk7.dat.SB_Build is smaller than expected, and likely invalid.\n\nThis will result in the game crashing on startup.\n\n If this occurs, please use 'Restore Original Game Files' in the SnakeBite settings, or verify the integrity of your game through Steam.", "Filesize check failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Debug.LogLine("[DatMerge] a_chunk7 filesize is likely too small to be valid.", Debug.LogLevel.Basic);
                        promptContinue = true;
                    } // filesize should be around 350,000,000 bytes as of 1.0.11.0
                    else
                    {
                        Debug.LogLine("[DatMerge] a_chunk7 filesize is sufficiently large, and likely valid.", Debug.LogLevel.Basic);
                        goodChunkSetup = true;
                    }
                else // no chunk files were moved to a_chunk7.
                {
                    MessageBox.Show("a_chunk7.dat.SB_Build could not be created during setup, likely because the original files were missing from 00.dat or 01.dat.\n\nThis will result in the game crashing on startup.\n\n If this occurs, please use 'Restore Original Game Files' in the SnakeBite settings, or verify the integrity of your game through Steam.", "Missing archive file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.LogLine("[DatMerge] a_chunk7 was not created during the setup process.", Debug.LogLevel.Basic);
                }

                if (File.Exists(t7Path + build_ext))
                    if (new System.IO.FileInfo(t7Path + build_ext).Length < 250000000)
                    {
                        MessageBox.Show("SnakeBite has detected that a_texture7.dat.SB_Build is smaller than expected, and likely invalid.\n\nThis will result in the game crashing on startup.\n\n If this occurs, please use 'Restore Original Game Files' in the SnakeBite settings, or verify the integrity of your game through Steam.", "Setup required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Debug.LogLine("[DatMerge] a_texture7 filesize is likely too small to be valid.", Debug.LogLevel.Basic);
                        promptContinue = true;
                    } // filesize should be around 255,000,000 bytes as of 1.0.11.0
                    else
                    {
                        Debug.LogLine("[DatMerge] a_texture7 filesize is sufficiently large, and likely valid.", Debug.LogLevel.Basic);
                        goodTexSetup = true;
                    }
                else // no textures were moved from 01.
                {
                    MessageBox.Show("a_texture7.dat.SB_Build could not be created during setup, likely because the original files were missing from 00.dat or 01.dat.\n\nThis will result in the game crashing on startup.\n\n If this occurs, please use 'Restore Original Game Files' in the SnakeBite settings, or verify the integrity of your game through Steam.", "Missing archive file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.LogLine("[DatMerge] a_texture7 was not created during the setup process.", Debug.LogLevel.Basic);
                }

                Debug.LogLine(String.Format("[DatMerge] Archive merging complete."), Debug.LogLevel.Debug);
                CleanupFolders();

                if (goodChunkSetup && goodTexSetup)
                    return true;
                else if (promptContinue)
                {
                    return (MessageBox.Show("Would you still like to continue the setup process?", "Continue Setup?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("An error has occured while moving files into new archives: {0}", e), "Exception Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.LogLine(string.Format("[DatMerge] Exception Occurred: {0}", e), Debug.LogLevel.Basic);
                Debug.LogLine("[DatMerge] SnakeBite has failed to move the 00.dat or 01.dat contents to new archives.", Debug.LogLevel.Basic);

                return false;
            }
        }

        private static void MoveDatFilesClean(SettingsManager manager)
        {
            string sourceName = null;
            string destName = null;
            string destFolder = null;

            // lua files 00 -> 01,    texture files 01 -> texture7,   foxpatch 00 -> 00,   chunkfiles 00 -> chunk7
            Debug.LogLine("[DatMerge] First Time Setup Started", Debug.LogLevel.Debug);

            bool c7t7Exists = true;
            if (manager.SettingsExist()) manager.ClearAllMods();
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

            List<string> zeroOut = zeroFiles.ToList();

            foreach (string zeroFile in zeroFiles)
            {
                if (zeroFile == "foxpatch.dat") continue;

                sourceName = Path.Combine("_extr", Tools.ToWinPath(zeroFile));

                if (zeroFile.Contains(".lua"))
                {
                    destName = Path.Combine("_working1", Tools.ToWinPath(zeroFile)); // 00 -> 01
                    oneFiles.Add(zeroFile);
                } else
                {
                    destName = Path.Combine("_working2", Tools.ToWinPath(zeroFile)); // 00 -> chunk7
                    chunk7Files.Add(zeroFile);
                }
                zeroOut.Remove(zeroFile);

                destFolder = Path.GetDirectoryName(destName);
                if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                if (!File.Exists(destName)) File.Move(sourceName, destName);
            }
            
            // Build a_chunk7.dat.SB_Build
            GzsLib.WriteQarArchive(c7Path + build_ext, "_working2", chunk7Files, GzsLib.chunk7Flags);

            // Build a_texture7.dat.SB_Build
            File.Copy(OnePath, t7Path + build_ext);

            // Build 00.dat.SB_Build
            GzsLib.WriteQarArchive(ZeroPath + build_ext, "_extr", zeroOut, GzsLib.zeroFlags);

            // Build 01.dat.SB_Build
            GzsLib.WriteQarArchive(OnePath + build_ext, "_working1", oneFiles, GzsLib.oneFlags);
        }

        // 00 non-snakebite Files to 01,  01 lua files unchanged,   01 textures -> t7,   01 chunkfiles -> c7, 
        private static void MoveDatFilesDirty(SettingsManager manager)
        {
            var modQarFiles = manager.GetModQarFiles();

            string sourceName = null;
            string destName = null;
            string destFolder = null;

            Debug.LogLine("[DatMerge] Dispersing files from 00 to 01, and then from 01 to a_chunk7 and a_texture7 if necessary.", Debug.LogLevel.Debug);
            List<string> oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_extr");
            List<string> zeroList = new List<string>();
            int moveCount = 0;

            try
            {
                zeroList = GzsLib.ListArchiveContents<QarFile>(ZeroPath);
            } catch (Exception e)
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

                    oneFiles.Add(zeroFile); // 00 -> 01
                    zeroOut.Remove(zeroFile);

                    destFolder = Path.GetDirectoryName(destName);
                    if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                    if (!File.Exists(destName)) File.Move(sourceName, destName);
                }

                GzsLib.WriteQarArchive(ZeroPath + build_ext, "_working1", zeroOut, GzsLib.zeroFlags); // rebuild 00 archive


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
                if (File.Exists(t7Path)) tex7List = GzsLib.ListArchiveContents<QarFile>(t7Path);
                if (File.Exists(c7Path)) chunk7List = GzsLib.ListArchiveContents<QarFile>(c7Path);
            } catch (Exception e)
            {
                Debug.LogLine(String.Format("[Error] GzsLib.ListArchiveContents exception: {0}", e.Message), Debug.LogLevel.Debug);
                throw e;
            }

            foreach (string oneFile in oneFiles)
            {
                if (modQarFiles.Contains(Tools.ToQarPath(oneFile)) || tex7List.Contains(oneFile) || chunk7List.Contains(oneFile)) continue;
                if (oneFile.Contains(".lua")) continue; // vanilla lua files must stay in 01
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
                    GzsLib.WriteQarArchive(t7Path + build_ext, "_working1", texture7Files, GzsLib.texture7Flags);
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
                    GzsLib.WriteQarArchive(c7Path + build_ext, "_working2", chunk7Files, GzsLib.chunk7Flags); // rebuild chunk7 archive
                }
                GzsLib.WriteQarArchive(OnePath + build_ext, "_extr", oneOut, GzsLib.oneFlags); // rebuild 01 archive
            }
        }

        public static bool ModifyFoxfs() // edits the chunk/texture lines in foxfs.dat to accommodate a_chunk7 a_texture7, MGO and GZs data.
        {
            CleanupFolders();

            Debug.LogLine("[ModifyFoxfs] Beginning foxfs.dat check.", Debug.LogLevel.Debug);
            try
            {
                string foxfsInPath = "foxfs.dat";
                string foxfsOutPath = "_extr\\foxfs.dat";

                if (GzsLib.ExtractFile<QarFile>(chunk0Path, foxfsInPath, foxfsOutPath)) //extract foxfs alone, to save time if the changes are already made
                {
                    if (!File.ReadAllText(foxfsOutPath).Contains("a_chunk7.dat")) // checks if there's an indication that it's modified
                    {
                        Debug.LogLine("[ModifyFoxfs] foxfs.dat is unmodified, extracting chunk0.dat.", Debug.LogLevel.Debug);
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
                        Debug.LogLine("[ModifyFoxfs] Updating foxfs.dat", Debug.LogLevel.Debug);
                        var foxfsLine = File.ReadAllLines(foxfsOutPath).ToList();   // read the file
                        int startIndex = foxfsLine.IndexOf("		<chunk id=\"0\" label=\"cypr\" qar=\"chunk0.dat\" textures=\"texture0.dat\"/>");

                        foxfsLine.RemoveRange(startIndex, 6);
                        foxfsLine.InsertRange(startIndex, linesToAdd);

                        File.WriteAllLines(foxfsOutPath, foxfsLine); // write to file

                        Debug.LogLine("[ModifyFoxfs] repacking chunk0.dat", Debug.LogLevel.Debug);

                        //Build chunk0.dat.SB_Build with modified foxfs
                        GzsLib.WriteQarArchive(chunk0Path + build_ext, "_extr", chunk0Files, GzsLib.chunk0Flags);

                    }
                    else
                    {
                        Debug.LogLine("[ModifyFoxfs] foxfs.dat is already modified", Debug.LogLevel.Debug);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Setup cancelled: SnakeBite failed to extract foxfs from chunk0."), "foxfs check failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.LogLine("[ModifyFoxfs] Process failed: could not check foxfs.dat", Debug.LogLevel.Debug);
                    CleanupFolders();

                    return false;
                }

                Debug.LogLine("[ModifyFoxfs] foxfs.dat modification complete.", Debug.LogLevel.Debug);
                CleanupFolders();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("An error has occured while modifying foxfs in chunk0: {0}", e), "Exception Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.LogLine(string.Format("[ModifyFoxfs] Exception Occurred: {0}", e), Debug.LogLevel.Basic);
                Debug.LogLine("[ModifyFoxfs] SnakeBite has failed to modify foxfs in chunk0", Debug.LogLevel.Basic);

                return false;
            }
        }

        private static void PromoteBuildArchives()
        {
            // Promote SB builds
            GzsLib.PromoteQarArchive(c7Path + build_ext, c7Path);
            GzsLib.PromoteQarArchive(t7Path + build_ext, t7Path);
            GzsLib.PromoteQarArchive(ZeroPath + build_ext, ZeroPath);
            GzsLib.PromoteQarArchive(OnePath + build_ext, OnePath);
            GzsLib.PromoteQarArchive(chunk0Path + build_ext, chunk0Path);

            new SettingsManager(GameDir).UpdateDatHash();
        }

        private static void ClearBuildArchives()
        {
            File.Delete(c7Path + build_ext);
            File.Delete(t7Path + build_ext);
            File.Delete(ZeroPath + build_ext);
            File.Delete(OnePath + build_ext);
            File.Delete(chunk0Path + build_ext);
        }

        /// <summary>
        /// Checks 00.dat files, indcluding fpk contents and adds the different mod entry types (if missing) to database (snakebite.xml)
        /// Slows down as number of fpks increase
        /// </summary>
        public static void CleanupDatabase()
        {
            Debug.LogLine("[Cleanup] Database cleanup started", Debug.LogLevel.Basic);

            // Retrieve installation data
            SettingsManager manager = new SettingsManager(GameDir);
            var mods = manager.GetInstalledMods();
            var game = manager.GetGameData();
            var zeroFiles = GzsLib.ListArchiveContents<QarFile>(ZeroPath);

            //Should only happen if user manually mods 00.dat
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
            var GameFpks = game.GameFpkEntries.ToList();
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
                } catch (IOException e)
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
                    Debug.LogLine($"[Cleanup] Adding missing {s}", Debug.LogLevel.Debug);
                    game.GameQarEntries.Add(new ModQarEntry() {
                        FilePath = Tools.ToQarPath(s),
                        SourceType = FileSource.System,
                        Hash = Tools.NameToHash(s)
                    });
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

        private static List<string> cleanupFolders = new List<string> {
            "_working0",
            "_working1",
            "_working2",//LEGACY
            "_extr",
            "_gameFpk",
            "_modfpk",
        };

        private static void CleanupFolders() // deletes the work folders which contain extracted files from 00/01
        {
            Debug.LogLine("[Mod] Cleaning up snakebite work folders.");
            foreach (var folder in cleanupFolders)
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
            bool directoryExists = true;
            while (directoryExists)
            {
                Thread.Sleep(100);
                directoryExists = false;
                foreach (var folder in cleanupFolders)
                {
                    if (Directory.Exists(folder)) directoryExists = true;
                }
            }
        }
    }//class ModManager
}//namespace SnakeBite