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

        public static void SavePreset(string presetFilePath)
        {
            Directory.CreateDirectory("_build\\master\\0");
            SettingsManager manager = new SettingsManager(SnakeBiteSettings);
            Debug.LogLine("[SavePreset] Saving preset file", Debug.LogLevel.Basic);
            try
            {
                foreach (string gameFile in manager.GetModExternalFiles())
                {
                    string sourcePath = Path.Combine(GameDir, Tools.ToWinPath(gameFile));

                    string DestDir = "_build\\" + Path.GetDirectoryName(gameFile);
                    string fileName = Path.GetFileName(gameFile);

                    Directory.CreateDirectory(DestDir);
                    if(File.Exists(sourcePath))File.Copy(sourcePath, Path.Combine(DestDir, fileName), true);
                    else Debug.LogLine(string.Format("[SavePreset] File not found: {0}", sourcePath), Debug.LogLevel.Basic);
                }
                File.Copy(ZeroPath, "_build\\master\\0\\00.dat", true);
                File.Copy(OnePath, "_build\\master\\0\\01.dat", true);
                File.Copy(SnakeBiteSettings, "_build\\snakebite.xml", true);

                FastZip zipper = new FastZip();
                zipper.CreateZip(presetFilePath, "_build", true, "(.*?)");
                Debug.LogLine("[SavePreset] Save Complete", Debug.LogLevel.Basic);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occurred and the preset was not saved.\nException: " + e);
            }
            finally
            {
                ModManager.CleanupFolders();
            }

        }

        public static void LoadPreset(string presetFilePath)
        {
            ModManager.CleanupFolders();
            SettingsManager manager = new SettingsManager(SnakeBiteSettings);
            List<string> existingExternalFiles = manager.GetModExternalFiles();
            List<string> fileEntryDirs = new List<string>();
            Debug.LogLine("[LoadPreset] Storing existing files", Debug.LogLevel.Basic);
            foreach (string gameFile in existingExternalFiles)
            {
                string gameFilePath = Path.Combine(GameDir, Tools.ToWinPath(gameFile));
                if (File.Exists(gameFilePath))
                {
                    fileEntryDirs.Add(Path.GetDirectoryName(gameFilePath));
                    File.Move(gameFilePath, gameFilePath + build_ext);
                }
            }
            
            File.Move(ZeroPath, ZeroPath + build_ext);
            File.Move(OnePath, OnePath + build_ext);
            File.Move(SnakeBiteSettings, SnakeBiteSettings + build_ext);

            Debug.LogLine("[LoadPreset] Importing preset files", Debug.LogLevel.Basic);
            try
            {
                FastZip unzipper = new FastZip();
                unzipper.ExtractZip(presetFilePath, GameDir, "(.*?)");

                Debug.LogLine("[LoadPreset] Deleting old files", Debug.LogLevel.Basic);
                foreach (string gameFile in existingExternalFiles)
                {
                    string gameFilePath = Path.Combine(GameDir, Tools.ToWinPath(gameFile));
                    File.Delete(gameFilePath + build_ext);
                }

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

                File.Delete(ZeroPath + build_ext);
                File.Delete(OnePath + build_ext);
                File.Delete(SnakeBiteSettings + build_ext);
                Debug.LogLine("[LoadPreset] Load Complete", Debug.LogLevel.Basic);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occurred and the preset was not imported.\nException: " + e);
                Debug.LogLine("[LoadPreset] Restoring old files", Debug.LogLevel.Basic);
                foreach (string gameFile in existingExternalFiles)
                {
                    string gameFilePath = Path.Combine(GameDir, Tools.ToWinPath(gameFile));
                    File.Move(gameFilePath + build_ext, gameFilePath);
                }
                File.Move(ZeroPath + build_ext, ZeroPath);
                File.Move(OnePath + build_ext, OnePath);
                File.Move(SnakeBiteSettings + build_ext, SnakeBiteSettings);
            }
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
