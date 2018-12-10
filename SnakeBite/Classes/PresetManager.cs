using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SnakeBite
{
    static class PresetManager
    {

        public static void SavePreset(string presetFilePath)
        {
            Directory.CreateDirectory("_build\\master\\0");
            SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
            Debug.LogLine("[SavePreset] Saving preset files", Debug.LogLevel.Basic);
            try
            {
                foreach (string gameFile in manager.GetModExternalFiles())
                {
                    string sourcePath = Path.Combine(GamePaths.GameDir, Tools.ToWinPath(gameFile));

                    string DestDir = "_build\\" + Path.GetDirectoryName(gameFile);
                    string fileName = Path.GetFileName(gameFile);

                    Directory.CreateDirectory(DestDir);
                    File.Copy(sourcePath, Path.Combine(DestDir, fileName), true);
                }
                File.Copy(GamePaths.ZeroPath, "_build\\master\\0\\00.dat", true);
                File.Copy(GamePaths.OnePath, "_build\\master\\0\\01.dat", true);
                File.Copy(GamePaths.SnakeBiteSettings, "_build\\snakebite.xml", true);

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
            string build_ext = GamePaths.build_ext;
            SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
            List<string> existingExternalFiles = manager.GetModExternalFiles();

            Debug.LogLine("[LoadPreset] Storing existing files", Debug.LogLevel.Basic);
            foreach (string gameFile in existingExternalFiles)
            {
                string gameFilePath = Path.Combine(GamePaths.GameDir, Tools.ToWinPath(gameFile));
                File.Move(gameFilePath, gameFilePath + build_ext);
            }
            
            File.Move(GamePaths.ZeroPath, GamePaths.ZeroPath + build_ext);
            File.Move(GamePaths.OnePath, GamePaths.OnePath + build_ext);
            File.Move(GamePaths.SnakeBiteSettings, GamePaths.SnakeBiteSettings + build_ext);

            Debug.LogLine("[LoadPreset] Importing preset files", Debug.LogLevel.Basic);
            try
            {
                FastZip unzipper = new FastZip();
                unzipper.ExtractZip(presetFilePath, GamePaths.GameDir, "(.*?)");

                Debug.LogLine("[LoadPreset] Deleting old files", Debug.LogLevel.Basic);
                foreach (string gameFile in existingExternalFiles)
                {
                    string gameFilePath = Path.Combine(GamePaths.GameDir, Tools.ToWinPath(gameFile));
                    File.Delete(gameFilePath + build_ext);
                }

                File.Delete(GamePaths.ZeroPath + build_ext);
                File.Delete(GamePaths.OnePath + build_ext);
                File.Delete(GamePaths.SnakeBiteSettings + build_ext);
                Debug.LogLine("[LoadPreset] Load Complete", Debug.LogLevel.Basic);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occurred and the preset was not imported.\nException: " + e);
                Debug.LogLine("[LoadPreset] Restoring old files", Debug.LogLevel.Basic);
                foreach (string gameFile in existingExternalFiles)
                {
                    string gameFilePath = Path.Combine(GamePaths.GameDir, Tools.ToWinPath(gameFile));
                    File.Move(gameFilePath + build_ext, gameFilePath);
                }
                File.Move(GamePaths.ZeroPath + build_ext, GamePaths.ZeroPath);
                File.Move(GamePaths.OnePath + build_ext, GamePaths.OnePath);
                File.Move(GamePaths.SnakeBiteSettings + build_ext, GamePaths.SnakeBiteSettings);
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
