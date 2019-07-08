using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using static SnakeBite.GamePaths;

namespace SnakeBite
{
    static class PresetManager
    {

        /// <summary>
        /// Creates a .MGSVPreset file for the mods that are currently installed
        /// </summary>
        public static bool SavePreset(string presetFilePath)
        {
            bool success = false;
            Directory.CreateDirectory("_build\\master\\0");
            SettingsManager manager = new SettingsManager(SnakeBiteSettings);
            Debug.LogLine("[SavePreset] Saving preset file...", Debug.LogLevel.Basic);
            try
            {
                foreach (string gameFile in manager.GetModExternalFiles())
                {
                    string sourcePath = Path.Combine(GameDir, Tools.ToWinPath(gameFile));

                    string DestDir = "_build\\" + Path.GetDirectoryName(gameFile);
                    string fileName = Path.GetFileName(gameFile);

                    Directory.CreateDirectory(DestDir);
                    if (File.Exists(sourcePath)) { Debug.LogLine(string.Format("[SavePreset] Copying to build directory: {0}", gameFile), Debug.LogLevel.Basic);  File.Copy(sourcePath, Path.Combine(DestDir, fileName), true); }
                    else Debug.LogLine(string.Format("[SavePreset] File not found: {0}", sourcePath), Debug.LogLevel.Basic);
                }
                Debug.LogLine("[SavePreset] Copying to build directory: 00.dat", Debug.LogLevel.Basic);
                File.Copy(ZeroPath, "_build\\master\\0\\00.dat", true);

                Debug.LogLine("[SavePreset] Copying to build directory: 01.dat", Debug.LogLevel.Basic);
                File.Copy(OnePath, "_build\\master\\0\\01.dat", true);

                Debug.LogLine("[SavePreset] Copying to build directory: snakebite.xml", Debug.LogLevel.Basic);
                File.Copy(SnakeBiteSettings, "_build\\snakebite.xml", true);

                FastZip zipper = new FastZip();
                Debug.LogLine(string.Format("[SavePreset] Writing {0}", Path.GetFileName(presetFilePath)), Debug.LogLevel.Basic);
                zipper.CreateZip(presetFilePath, "_build", true, "(.*?)");
                Debug.LogLine("[SavePreset] Write Complete", Debug.LogLevel.Basic);
                success = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occurred and the preset was not saved.\nException: " + e);
            }
            finally
            {
                ModManager.CleanupFolders();
            }

            return success;
        }

        /// <summary>
        /// overwrites existing mods with the set of mods stored in the .MGSVPreset file
        /// </summary>
        public static bool LoadPreset(string presetFilePath)
        {
            bool panicMode = (!File.Exists(ZeroPath) || !File.Exists(OnePath) || !File.Exists(SnakeBiteSettings)); 
            bool success = false;
            ModManager.CleanupFolders();
            SettingsManager manager = new SettingsManager(SnakeBiteSettings);
            List<string> existingExternalFiles = new List<string>();
            List<string> fileEntryDirs = new List<string>();
            try
            {
                existingExternalFiles = manager.GetModExternalFiles();
            }
            catch
            {
                panicMode = true;
            }
            try
            {
                if (!panicMode)
                {
                    Debug.LogLine("[LoadPreset] Storing backups of existing files...", Debug.LogLevel.Basic);
                    foreach (string gameFile in existingExternalFiles)
                    {
                        string gameFilePath = Path.Combine(GameDir, Tools.ToWinPath(gameFile));
                        if (File.Exists(gameFilePath)) // only stores backups of managed files
                        {
                            Debug.LogLine(string.Format("[LoadPreset] Storing backup: {0}", gameFile), Debug.LogLevel.Basic);
                            fileEntryDirs.Add(Path.GetDirectoryName(gameFilePath));
                            if (File.Exists(gameFilePath + build_ext)) File.Delete(gameFilePath + build_ext);
                            File.Move(gameFilePath, gameFilePath + build_ext);
                        }
                    }
                    Debug.LogLine("[LoadPreset] Storing backup: 00.dat", Debug.LogLevel.Basic);
                    File.Copy(ZeroPath, ZeroPath + build_ext, true);

                    Debug.LogLine("[LoadPreset] Storing backup: 01.dat", Debug.LogLevel.Basic);
                    File.Copy(OnePath, OnePath + build_ext, true);

                    Debug.LogLine("[LoadPreset] Storing backup: snakebite.xml", Debug.LogLevel.Basic);
                    File.Copy(SnakeBiteSettings, SnakeBiteSettings + build_ext, true);
                }
                else
                {
                    Debug.LogLine("[LoadPreset] Critical file(s) are disfunctional or not found, skipping backup procedure", Debug.LogLevel.Basic);
                }

                Debug.LogLine("[LoadPreset] Importing preset files", Debug.LogLevel.Basic);
                FastZip unzipper = new FastZip();
                unzipper.ExtractZip(presetFilePath, GameDir, "(.*?)");

                Debug.LogLine("[LoadPreset] Import Complete", Debug.LogLevel.Basic);
                success = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occurred and the preset was not imported.\nException: " + e);
                if (!panicMode)
                {
                    Debug.LogLine("[LoadPreset] Restoring backup files", Debug.LogLevel.Basic);

                    File.Copy(ZeroPath + build_ext, ZeroPath, true);
                    File.Copy(OnePath + build_ext, OnePath, true);
                    File.Copy(SnakeBiteSettings + build_ext, SnakeBiteSettings, true);

                    foreach (string gameFile in existingExternalFiles)
                    {
                        string gameFilePath = Path.Combine(GameDir, Tools.ToWinPath(gameFile));
                        if (File.Exists(gameFilePath + build_ext))
                            File.Copy(gameFilePath + build_ext, gameFilePath, true);
                    }
                }
            }
            finally
            {
                if (!panicMode)
                {
                    Debug.LogLine("[LoadPreset] Removing backup files", Debug.LogLevel.Basic);
                    foreach (string gameFile in existingExternalFiles)
                    {
                        string gameFilePath = Path.Combine(GameDir, Tools.ToWinPath(gameFile));
                        if (File.Exists(gameFilePath)) File.Delete(gameFilePath + build_ext);
                    }

                    foreach (string fileEntryDir in fileEntryDirs)
                    {
                        if (Directory.Exists(fileEntryDir))
                        {
                            try
                            {
                                if (Directory.GetFiles(fileEntryDir).Length == 0)
                                {
                                    Debug.LogLine(String.Format("[SB_Build] deleting empty folder: {0}", fileEntryDir), Debug.LogLevel.All);
                                    Directory.Delete(fileEntryDir);
                                }
                            }

                            catch (Exception e)
                            {
                                Debug.LogLine("[Uninstall] Could not delete: " + e.Message);
                            }
                        }
                    }
                    File.Delete(ZeroPath + build_ext);
                    File.Delete(OnePath + build_ext);
                    File.Delete(SnakeBiteSettings + build_ext);
                }
            }

            return success;
        }

        public static bool isPresetUpToDate(Settings presetSettings)
        {
                var presetVersion = presetSettings.MGSVersion.AsVersion();
                var MGSVersion = ModManager.GetMGSVersion();

                return (presetVersion == MGSVersion);
        }

        public static Settings ReadSnakeBiteSettings(string PresetFilePath)
        {
            if (!File.Exists(PresetFilePath)) return null;

            try
            {
                using (FileStream streamPreset = new FileStream(PresetFilePath, FileMode.Open))
                using (ZipFile zipMod = new ZipFile(streamPreset))
                {
                    var sbIndex = zipMod.FindEntry("snakebite.xml", true);
                    if (sbIndex == -1) return null;
                    using (StreamReader sbReader = new StreamReader(zipMod.GetInputStream(sbIndex)))
                    {
                        XmlSerializer x = new XmlSerializer(typeof(Settings));
                        var settings = (Settings)x.Deserialize(sbReader);
                        return settings;
                    }
                }
            }
            catch { return null; }

        }

    }
}
