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

            // Extract game archive
            var datFiles = GzsLib.ExtractArchive<QarFile>(DatPath, "_working");
            datFiles = FixModFilenames(datFiles, "_working");

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

            // Check for FPKs in 01.dat
            foreach (string fpk in modFpks)
            {
                string datFile = datFiles.FirstOrDefault(file => Tools.NameToHash(file) == Tools.NameToHash(fpk));
                if (datFile != null && !mergeFpks.Contains(fpk))
                {
                    mergeFpks.Add(fpk);
                }
            }

            var gameData = GzsLib.ReadBaseData();

            var zeroFiles = gameData.FileList.FindAll(entry => entry.QarFile == "0\\00.dat");
            var baseFiles = gameData.FileList.FindAll(entry => entry.QarFile != "0\\00.dat");

            // Check for FPKs in 00.dat
            foreach (string fpk in modFpks)
            {
                GameFile file = zeroFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                if (file != null && !mergeFpks.Contains(fpk))
                {
                    // Create destination directory
                    string destDirectory = Path.Combine("_working", Path.GetDirectoryName(Tools.ToWinPath(file.FilePath)));
                    if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);
                    // Extract file into dat directory
                    GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\0\\00.dat"), file.FileHash, Path.Combine("_working", Tools.ToWinPath(fpk)));
                    mergeFpks.Add(fpk);
                }
            }

            // Check for FPKs in base data
            foreach (string fpk in modFpks)
            {
                GameFile file = baseFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                if (file != null && !mergeFpks.Contains(fpk))
                {
                    // Create destination directory
                    string destDirectory = Path.Combine("_working", Path.GetDirectoryName(Tools.ToWinPath(file.FilePath)));
                    if (!Directory.Exists(destDirectory)) Directory.CreateDirectory(destDirectory);
                    // Extract file into dat directory
                    GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\" + file.QarFile), file.FileHash, Path.Combine("_working", Tools.ToWinPath(fpk)));
                    mergeFpks.Add(fpk);
                }
            }

            // Merge FPK files
            foreach (string fpk in mergeFpks)
            {
                // Extract game FPK
                string fpkPath = Path.Combine("_working", Tools.ToWinPath(fpk));
                var gameFpk = GzsLib.ExtractArchive<FpkFile>(fpkPath, "_gamefpk");

                // Extract mod FPK
                GzsLib.ExtractArchive<FpkFile>(Path.Combine("_extr", Tools.ToWinPath(fpk)), "_modfpk");

                // Merge contents
                foreach (string fileName in Directory.GetFiles("_modfpk", "*.*", SearchOption.AllDirectories))
                {
                    string fileFpkName = fileName.Substring(8);
                    string fileDir = (Path.Combine("_gamefpk", Path.GetDirectoryName(fileName).Substring(8)));

                    if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
                    File.Copy(fileName, Path.Combine(fileDir, Path.GetFileName(fileName)), true);
                    if (!gameFpk.Contains(fileFpkName)) gameFpk.Add(fileFpkName);
                }

                // Rebuild game FPK
                GzsLib.WriteFpkArchive(fpkPath, "_gamefpk", gameFpk);
                if (!datFiles.Contains(Tools.ToWinPath(fpk))) datFiles.Add(Tools.ToWinPath(fpk));
                Directory.Delete("_modfpk", true);
                Directory.Delete("_gamefpk", true);
            }

            // Copy files for 01.dat, ignoring merged FPKs
            List<ModQarEntry> qarEntries = metaData.ModQarEntries.FindAll(entry => !mergeFpks.Contains(entry.FilePath));
            foreach (ModQarEntry modEntry in qarEntries)
            {
                string sourceFile = Path.Combine("_extr", Tools.ToWinPath(modEntry.FilePath));
                string destFile = Path.Combine("_working", Tools.ToWinPath(modEntry.FilePath));
                string destDir = Path.GetDirectoryName(destFile);

                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                File.Copy(sourceFile, destFile, true);

                if (!datFiles.Contains(modEntry.FilePath)) datFiles.Add(Tools.ToWinPath(modEntry.FilePath));
            }

            // Rebuild 01.dat
            GzsLib.WriteQarArchive(DatPath, "_working", datFiles, 3150048);

            CleanupFolders();

            return true;
        }

        public static bool UninstallMod(ModEntry mod)
        {
            CleanupFolders();

            // Extract game archive
            var datFiles = GzsLib.ExtractArchive<QarFile>(DatPath, "_working");
            datFiles = FixModFilenames(datFiles, "_working");

            // List all FPKs in mod
            List<string> modFpks = new List<string>();
            foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
            {
                if (!modFpks.Contains(fpkEntry.FpkFile)) modFpks.Add(fpkEntry.FpkFile);
            }

            // Extract FPK
            foreach (string fpk in modFpks)
            {
                var fpkFile = GzsLib.ExtractArchive<FpkFile>(Path.Combine("_working", Tools.ToWinPath(fpk)), "_modfpk");
                foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
                {
                    // Remove Mod Files
                    fpkFile.RemoveAll(file => Tools.ToQarPath(file) == Tools.ToQarPath(fpkEntry.FilePath));
                }
                if (fpkFile.Count == 0)
                {
                    datFiles.RemoveAll(file => Tools.NameToHash(file) == Tools.NameToHash(fpk));
                }
                else
                {
                    var gameData = GzsLib.ReadBaseData();

                    var zeroFiles = gameData.FileList.FindAll(entry => entry.QarFile == "0\\00.dat");
                    var baseFiles = gameData.FileList.FindAll(entry => entry.QarFile != "0\\00.dat");

                    // Check for FPKs in 00.dat first
                    GameFile file = zeroFiles.FirstOrDefault(entry => entry.FileHash == Tools.NameToHash(fpk));
                    if (file != null)
                    {
                        // Extract base FPK files
                        GzsLib.ExtractFileByHash<QarFile>(Path.Combine(GameDir, "master\\0\\00.dat"), file.FileHash, "_working\\temp.fpk");
                        var gameFpk = GzsLib.ExtractArchive<FpkFile>("_working\\temp.fpk", "_gamefpk");

                        // Copy remaining files over base FPK
                        foreach (string mFile in fpkFile)
                        {
                            string fDir = Path.GetDirectoryName(mFile);
                            if (!Directory.Exists(Path.Combine("_gamefpk", fDir))) Directory.CreateDirectory(Path.Combine("_gamefpk", fDir));
                            File.Copy(Path.Combine("_modfpk", mFile), Path.Combine(Path.Combine("_gamefpk", mFile)), true);
                            if (!gameFpk.Contains(mFile)) gameFpk.Add(mFile);
                        }

                        // Rebuild FPK
                        GzsLib.WriteFpkArchive(Path.Combine("_working", Tools.ToWinPath(fpk)), "_gamefpk", gameFpk);
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

                        // Copy remaining files over base FPK
                        foreach (string mFile in fpkFile)
                        {
                            string fDir = Path.GetDirectoryName(mFile);
                            if (!Directory.Exists(Path.Combine("_gamefpk", fDir))) Directory.CreateDirectory(Path.Combine("_gamefpk", fDir));
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

            // Remove all mod files from 01.dat
            foreach (ModQarEntry qarEntry in mod.ModQarEntries)
            {
                string fExt = Path.GetExtension(qarEntry.FilePath);
                if (!fExt.Contains(".fpk"))
                {
                    datFiles.RemoveAll(file => Tools.NameToHash(file) == Tools.NameToHash(qarEntry.FilePath));
                }
            }

            // Rebuild 01.dat
            GzsLib.WriteQarArchive(DatPath, "_working", datFiles, 3150048);

            CleanupFolders();
            return true;
        }

        internal static void CleanupDatabase()
        {
            // Remove existing working data
            CleanupFolders();

            // Get installed mods
            List<ModEntry> mods = SettingsManager.GetInstalledMods();

            // Extract game dat to working dir
            List<string> datFiles = GzsLib.ExtractArchive<QarFile>(DatPath, "_working");
            List<string> gameFiles = datFiles.ToList();
            List<string> gameFpks = gameFiles.FindAll(entry => entry.Contains(".fpk"));
            List<ModFpkEntry> gameFpkEntries = new List<ModFpkEntry>();
            foreach (string fpk in gameFpks)
            {
                // List contents of FPK
                var fpkContents = GzsLib.ListArchiveContents<FpkFile>(Path.Combine("_working", Tools.ToWinPath(fpk)));
                foreach (string file in fpkContents)
                {
                    gameFpkEntries.Add(new ModFpkEntry() { FilePath = file, FpkFile = fpk });
                }
            }

            // Iterate through installed mods
            foreach (ModEntry mod in mods)
            {
                // Files to be removed from mod
                List<ulong> removeFiles = new List<ulong>();

                // Iterate all QAR files for the mod
                foreach (ModQarEntry modQarEntry in mod.ModQarEntries)
                {
                    if (!gameFiles.Contains(modQarEntry.FilePath))
                        removeFiles.Add(Tools.NameToHash(modQarEntry.FilePath));
                }

                // Check if mod files are removed
                foreach (ulong hash in removeFiles)
                {
                    mod.ModFpkEntries.RemoveAll(entry => Tools.NameToHash(entry.FpkFile) == hash);
                    mod.ModQarEntries.RemoveAll(entry => Tools.NameToHash(entry.FilePath) == hash);
                }
            }

            Settings settings = new Settings();
            settings.Load();
            settings.ModEntries.RemoveAll(entry => entry.ModFpkEntries.Count == 0 && entry.ModQarEntries.Count == 0);
            foreach (ModEntry mod in mods)
            {
                foreach (ModFpkEntry fpkEntry in mod.ModFpkEntries)
                {
                    gameFpkEntries.RemoveAll(entry => entry.FilePath == fpkEntry.FilePath && entry.FpkFile == fpkEntry.FpkFile);
                }
                foreach (ModQarEntry qarEntry in mod.ModQarEntries)
                {
                    gameFiles.RemoveAll(entry => Tools.NameToHash(entry) == Tools.NameToHash(qarEntry.FilePath));
                }
            }
            settings.GameData.GameQarEntries = new List<ModQarEntry>();
            foreach (string file in gameFiles)
            {
                settings.GameData.GameQarEntries.Add(new ModQarEntry() { FilePath = Tools.ToQarPath(file), Hash = Tools.NameToHash(file), Compressed = file.Contains(".fpk") ? true : false });
            }
            settings.GameData.GameFpkEntries = gameFpkEntries;
            settings.Save();
        }

        internal static List<string> FixModFilenames(List<string> Files, string SourceDir)
        {
            List<ModEntry> mods = SettingsManager.GetInstalledMods();
            List<string> outFiles = Files.ToList();

            foreach (string file in Files)
            {
                foreach (ModEntry mod in mods)
                {
                    ModQarEntry qarEntry = mod.ModQarEntries.FirstOrDefault(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(file));
                    if (qarEntry != null)
                    {
                        string FileDir = Path.Combine(SourceDir, Path.GetDirectoryName(Tools.ToWinPath(qarEntry.FilePath)));
                        if (FileDir == SourceDir) continue; // don't want to move named files to un-named files
                        if (!Directory.Exists(FileDir)) Directory.CreateDirectory(FileDir);
                        if (File.Exists(Path.Combine(SourceDir, Tools.ToWinPath(file))))
                            File.Move(Path.Combine(SourceDir, Tools.ToWinPath(file)),
                                      Path.Combine(SourceDir, Tools.ToWinPath(qarEntry.FilePath)));
                        outFiles[Files.IndexOf(file)] = qarEntry.FilePath;
                    }
                }
            }

            return outFiles;
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