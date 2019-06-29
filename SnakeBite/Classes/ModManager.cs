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
using static SnakeBite.GamePaths;

namespace SnakeBite
{
    internal static class ModManager
    {

        //Cull any invalid entries that might have slipped in via older versions of snakebite
        public static void ValidateGameData(ref GameData gameData, ref List<string> zeroFiles)
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

        public static bool hasQarZeroFiles(List<string> ModFiles) // returns true if any mods in the list contain a loose texture file which was installed to 01
        {
            ModEntry metaData;
            foreach (string modfile in ModFiles)
            {
                metaData = Tools.ReadMetaData(modfile);
                foreach (ModQarEntry qarFile in metaData.ModQarEntries)
                {
                    if (!qarFile.FilePath.Contains(".ftex"))
                        return true;
                }
            }
            return false;
        }

        public static bool hasQarZeroFiles(List<ModEntry> checkMods) // any non-.ftex(s) file in modQarEntries will return true
        {
            foreach (ModEntry mod in checkMods)
            {
                foreach (ModQarEntry qarFile in mod.ModQarEntries)
                {
                    if (!qarFile.FilePath.Contains(".ftex")) 
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
            try
            {
                GzsLib.LoadDictionaries();
                ClearBuildFiles(c7Path, t7Path, ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath);
                ClearSBGameDir();
                CleanupFolders();
                mergeProcessor.ReportProgress(0, "Moving files into new archives");
                if (!MoveDatFiles()) //moves vanilla 00 files into 01, excluding foxpatch. 
                {
                    Debug.LogLine("[DatMerge] Failed to complete archive migration. Cancelling...");
                    e.Cancel = true;
                    ClearBuildFiles(c7Path, t7Path, ZeroPath, OnePath);
                    return;
                }

                mergeProcessor.ReportProgress(0, "Modfying foxfs in chunk0");
                if (!ModifyFoxfs()) // adds lines to foxfs in chunk0.
                {
                    Debug.LogLine("[ModifyFoxfs] Failed to complete Foxfs modification. Cancelling...");
                    e.Cancel = true;
                    ClearBuildFiles(c7Path, t7Path, ZeroPath, OnePath, chunk0Path);
                    return;
                }

                mergeProcessor.ReportProgress(0, "Promoting new archives");
                PromoteBuildFiles(c7Path, t7Path, ZeroPath, OnePath, chunk0Path); // overwrites existing archives with modified archives

                mergeProcessor.ReportProgress(0, "Cleaning up database");
                CleanupDatabase();
            }
            catch (Exception f)
            {
                MessageBox.Show(string.Format("An error has occurred while attempting to merge or clean up SnakeBite data: {0}", f), "Exception Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.LogLine(string.Format("[MergeAndCleanup] Exception Occurred: {0}", f), Debug.LogLevel.Basic);
                Debug.LogLine("[MergeAndCleanup] SnakeBite has failed to merge and clean up SnakeBite data", Debug.LogLevel.Basic);
                e.Cancel = true;
                try
                {
                    ClearBuildFiles(c7Path, t7Path, ZeroPath, OnePath, chunk0Path);
                }
                catch (Exception g)
                {
                    Debug.LogLine(string.Format("[MergeAndCleanup] Exception Occurred: {0}", g), Debug.LogLevel.Basic);
                    Debug.LogLine("[MergeAndCleanup] SnakeBite has failed to remove the build files.", Debug.LogLevel.Basic);
                }
                return;
            }
        }

        public static bool MoveDatFiles() // moves all vanilla 00.dat files, excluding foxpatch.dat, to 01.dat
        {
            SettingsManager manager = new SettingsManager(SnakeBiteSettings);
            Debug.LogLine("[DatMerge] Beginning to move files to new archives");
            try
            {
                if (manager.IsVanilla0001DatHash() || manager.IsVanilla0001Size() )
                {   // first time setup or files have been revalidated
                    MoveDatFilesClean(manager);
                }
                else
                {   // the "uncertainty" case.
                    MoveDatFilesDirty(manager);
                }

                Debug.LogLine(String.Format("[DatMerge] Archive merging finished"), Debug.LogLevel.Debug);
                CleanupFolders();

                return Checkc7t7Setup();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("An error has occured while moving files into new archives: {0}", e), "Exception Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.LogLine(string.Format("[DatMerge] Exception Occurred: {0}", e), Debug.LogLevel.Basic);
                Debug.LogLine("[DatMerge] SnakeBite could not move the 00.dat or 01.dat contents to new archives.", Debug.LogLevel.Basic);

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
            
            if (manager.SettingsExist()) manager.ClearAllMods();

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
            File.Copy(OnePath, t7Path + build_ext, true);

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

        private static bool Checkc7t7Setup()
        {
            //check for good c7, t7
            int archiveState = 0;

            string chunkCheckPath = c7Path + build_ext;
            if (!File.Exists(chunkCheckPath)) chunkCheckPath = c7Path;

            string texCheckPath = t7Path + build_ext;
            if (!File.Exists(texCheckPath)) texCheckPath = t7Path;

            // check if they exist and their size
            long chunkSize = 0;
            long texSize = 0;
            if (File.Exists(chunkCheckPath) && File.Exists(texCheckPath))
            {
                chunkSize = new FileInfo(chunkCheckPath).Length;
                texSize = new FileInfo(texCheckPath).Length;
                if (chunkSize >= 345000000 && texSize >= 250000000)
                {
                    archiveState = 1; // Good State
                }
                else archiveState = 2; // Bad Size
            }
            else archiveState = 3; // Not Found
            
            switch (archiveState)
            {
                case 1:
                    Debug.LogLine("[DatMerge] chunk7 and texture7 are sufficiently large and likely valid.", Debug.LogLevel.Basic);
                    return true;

                case 2:
                    MessageBox.Show("SnakeBite has detected that the reformatted game data is smaller than expected and likely invalid." +
                        "\n\nThis will result in the game crashing on startup." +
                        "\n\n If this occurs, please use 'Restore Backup Game Files' in the SnakeBite settings or verify the integrity of your game through Steam.", "Filesize check failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Debug.LogLine(string.Format("[DatMerge] {0} filesize: {1}", Path.GetFileName(chunkCheckPath), chunkSize), Debug.LogLevel.Basic);
                    Debug.LogLine(string.Format("[DatMerge] {0} filesize: {1}", Path.GetFileName(texCheckPath), texSize), Debug.LogLevel.Basic);
                    break;

                case 3:
                    MessageBox.Show("SnakeBite could not reformat the necessary game data during setup. This issue could be caused by missing game files."+
                        "\n\nThis error will result in the game crashing on startup." +
                        "\n\n If this occurs, please use 'Restore Backup Game Files' in the SnakeBite settings or verify the integrity of your game through Steam.", "Missing archive file(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Debug.LogLine("[DatMerge] chunk7 and/or texture7 were not created during the setup process.", Debug.LogLevel.Basic);
                    break;
            }

            return (MessageBox.Show("Would you still like to continue the setup process?", "Continue Setup?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
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

                Debug.LogLine("[ModifyFoxfs] Archive modification complete.", Debug.LogLevel.Debug);
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

        public static void PrepGameDirFiles()
        {
            CopyGameDirManagedFiles(GameDirSB_Build);
            CopyGameDirManagedFiles(GameDirBackup_Build);
        }

        public static void CopyGameDirManagedFiles(string destinationDir)
        {
            Debug.LogLine("[SB_Build] Writing SB_Build Game Directory", Debug.LogLevel.Basic);
            foreach (string externalFile in new SettingsManager(SnakeBiteSettings).GetModExternalFiles()) 
            {
                string fileModPath = Tools.ToWinPath(externalFile);
                string destFullPath = Path.Combine(destinationDir, fileModPath);
                string sourceFullPath = Path.Combine(GameDir, fileModPath);

                Directory.CreateDirectory(Path.GetDirectoryName(destFullPath));
                if (File.Exists(sourceFullPath)) { File.Copy(sourceFullPath, destFullPath, true); }
            }
        }

        public static void PromoteGameDirFiles() // call this method BEFORE snakebite.xml.SB_Build is promoted, so it will reference the old snakebite.xml
        {
            Debug.LogLine("[SB_Build] Promoting SB_Build Game Directory", Debug.LogLevel.Basic);
            if (!Directory.Exists(GameDirSB_Build))
            {
                Debug.LogLine($"[SB_Build] Directory not found: {GameDirSB_Build}", Debug.LogLevel.Basic);
                return;
            }

            List<string> fileEntryDirs = new List<string>();
            foreach (string externalFile in new SettingsManager(SnakeBiteSettings).GetModExternalFiles())
            {
                string fileModPath = Tools.ToWinPath(externalFile);
                string sourceFullPath = Path.Combine(GameDir, fileModPath);

                string sourceDir = Path.GetDirectoryName(sourceFullPath);
                if (!fileEntryDirs.Contains(sourceDir)) fileEntryDirs.Add(sourceDir);

                if (File.Exists(sourceFullPath)) File.Delete(sourceFullPath); // deletes all of the old snakebite.xml's managed files (unmanaged files will be overwritten later or left alone)
                else Debug.LogLine(string.Format("[SB_Build] File not found: {0}", sourceFullPath), Debug.LogLevel.Basic);
            }

            Tools.DirectoryCopy(GameDirSB_Build, GameDir, true); // moves all gamedir_sb_build files over

            foreach (string fileEntryDir in fileEntryDirs)
            {
                if (Directory.Exists(fileEntryDir) && Directory.GetFiles(fileEntryDir).Length == 0)
                {
                    Debug.LogLine(String.Format("[SB_Build] deleting empty folder: {0}", fileEntryDir), Debug.LogLevel.All);
                    try
                    {
                        Directory.Delete(fileEntryDir);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                    }
                }
            }
        }

        public static void RestoreBackupGameDir(SettingsManager SBBuildSettings)
        {
            Debug.LogLine("[SB_Build] Promoting SB_Build Game Directory", Debug.LogLevel.Basic);
            if (!Directory.Exists(GameDirBackup_Build))
            {
                Debug.LogLine($"[SB_Build] Directory not found: {GameDirBackup_Build}", Debug.LogLevel.Basic);
                return;
            }

            List<string> fileEntryDirs = new List<string>();
            foreach (string externalBuildFiles in SBBuildSettings.GetModExternalFiles())
            {
                string fileModPath = Tools.ToWinPath(externalBuildFiles);
                string sourceFullPath = Path.Combine(GameDir, fileModPath);

                string sourceDir = Path.GetDirectoryName(sourceFullPath);
                if (!fileEntryDirs.Contains(sourceDir)) fileEntryDirs.Add(sourceDir);

                if (File.Exists(sourceFullPath)) File.Delete(sourceFullPath); // deletes all of the new snakebite.xml's managed files
                else Debug.LogLine(string.Format("[SB_Build] File not found: {0}", sourceFullPath), Debug.LogLevel.Basic);
            }

            Tools.DirectoryCopy(GameDirBackup_Build, GameDir, true); // moves all gamedir_backup_build files over

            foreach (string fileEntryDir in fileEntryDirs) //all the directories that have had files deleted within them
            {
                if (Directory.Exists(fileEntryDir) && Directory.GetFiles(fileEntryDir).Length == 0) // if the directory has not yet been deleted and there are no more files inside the directory
                {
                    Debug.LogLine(String.Format("[SB_Build] deleting empty folder: {0}", fileEntryDir), Debug.LogLevel.All);
                    try
                    {
                        Directory.Delete(fileEntryDir);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("[Uninstall] Could not delete: " + e.Message);
                    }
                }
            }
        }

        public static void PromoteBuildFiles(params string[] paths)
        {
            // Promote SB builds
            Debug.LogLine("[SB_Build] Promoting SB_Build files", Debug.LogLevel.Basic);
            foreach (string path in paths)
            {
                GzsLib.PromoteQarArchive(path + build_ext, path);
            }
            
            new SettingsManager(SnakeBiteSettings).UpdateDatHash();
        }

        public static void ClearBuildFiles(params string[] paths)
        {
            Debug.LogLine("[SB_Build] Deleting SB_Build files", Debug.LogLevel.Basic);
            foreach (string path in paths)
            {
                if (File.Exists(path + build_ext))
                    File.Delete(path + build_ext);
            }
        }

        public static void ClearSBGameDir()
        {
            Debug.LogLine("[SB_Build] Deleting old SB_Build Game Directory", Debug.LogLevel.Basic);
            try
            {
                if(Directory.Exists(GameDirSB_Build))
                    Tools.DeleteDirectory(GameDirSB_Build);
                if (Directory.Exists(GameDirBackup_Build))
                    Tools.DeleteDirectory(GameDirBackup_Build);
            }
            catch (IOException e)
            {
                Console.WriteLine("[Cleanup] Could not delete Game Directory Content: " + e.Message);
            }
        }

        /// <summary>
        /// Checks 00.dat files, indcluding fpk contents and adds the different mod entry types (if missing) to database (snakebite.xml)
        /// Slows down as number of fpks increase
        /// </summary>
        public static void CleanupDatabase()
        {
            Debug.LogLine("[Cleanup] Database cleanup started", Debug.LogLevel.Basic);

            // Retrieve installation data
            SettingsManager manager = new SettingsManager(SnakeBiteSettings);
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
            var fpkFiles = cleanFiles.FindAll(entry => entry.EndsWith(".fpk") || entry.EndsWith(".fpkd"));
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
            if (versionInfo != null)
            {
                if (versionInfo.ProductVersion != null)
                {
                    return new Version(versionInfo.ProductVersion);
                }
            }
            return new Version(0,0,0,0);
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
            "_build",
            "_gameFpk",
            "_modfpk",
        };

        public static void CleanupFolders() // deletes the work folders which contain extracted files from 00/01
        {
            Debug.LogLine("[Mod] Cleaning up snakebite work folders.");
            try
            {
                foreach (var folder in cleanupFolders)
                {
                    if (Directory.Exists(folder)) Tools.DeleteDirectory(folder);
                }
                /*
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
                */
            }
            catch { }
        }
    }//class ModManager
}//namespace SnakeBite