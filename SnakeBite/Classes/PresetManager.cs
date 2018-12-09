using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SnakeBite
{
    static class PresetManager
    {

        public static void SavePreset(string presetFilePath)
        {
            Directory.CreateDirectory("_build");

            File.Copy(GamePaths.ZeroPath, "_build\\00.dat", true);
            File.Copy(GamePaths.OnePath, "_build\\01.dat", true);
            File.Copy(GamePaths.SnakeBiteSettings, "_build\\snakebite.xml", true);

            FastZip zipper = new FastZip();
            zipper.CreateZip(presetFilePath, "_build", true, "(.*?)");

            Directory.Delete("_build", true);
        }

        public static bool isPresetUpToDate(Settings presetSettings)
        {
            var presetVersion = presetSettings.MGSVersion.AsVersion();
            var MGSVersion = ModManager.GetMGSVersion();

            return (presetVersion == MGSVersion);
        }

        public static void InstallPreset(string presetFilePath)
        {
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(presetFilePath, "_extr", "(.*?)");

            //todo create revert presets first!!!
            File.Delete(GamePaths.ZeroPath);
            File.Delete(GamePaths.OnePath);
            File.Delete(GamePaths.SnakeBiteSettings);

            File.Move("_extr\\00.dat", GamePaths.ZeroPath);
            File.Move("_extr\\01.dat", GamePaths.OnePath);
            File.Move("_extr\\snakebite.xml", GamePaths.SnakeBiteSettings);
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
