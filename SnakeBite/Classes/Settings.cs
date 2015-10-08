using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SnakeBite
{
    [XmlType("Settings")]
    public class Settings
    {
        [XmlElement("GameData")]
        public GameData GameData { get; set; } = new GameData();

        [XmlArray("Mods")]
        public List<ModEntry> ModEntries { get; set; } = new List<ModEntry>();

        public void SaveSettings()
        {
            // Write settings to XML

            if (File.Exists("settings.xml"))
            {
                File.Delete("settings.xml");
            }

            XmlSerializer x = new XmlSerializer(typeof(Settings), new[] { typeof(Settings) });
            StreamWriter s = new StreamWriter("settings.xml");
            x.Serialize(s, this);
            s.Close();
        }

        public bool LoadSettings()
        {
            // Load settings from XML

            if (!File.Exists("settings.xml"))
            {
                return false;
            }

            XmlSerializer x = new XmlSerializer(typeof(Settings));
            StreamReader s = new StreamReader("settings.xml");
            Settings loaded = (Settings)x.Deserialize(s);
            GameData = loaded.GameData;
            ModEntries = loaded.ModEntries;
            s.Close();
            return true;
        }
    }

    [XmlType("GameData")]
    public class GameData
    {
        public GameData()
        {
            GameQarEntries = new List<ModQarEntry>();
            GameFpkEntries = new List<ModFpkEntry>();
        }

        [XmlArray("QarEntries")]
        public List<ModQarEntry> GameQarEntries { get; set; }

        [XmlArray("FpkEntries")]
        public List<ModFpkEntry> GameFpkEntries { get; set; }
    }

    [XmlType("ModEntry")]
    public class ModEntry
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlAttribute("Author")]
        public string Author { get; set; }

        [XmlAttribute("Website")]
        public string Website { get; set; }

        [XmlArray("QarEntries")]
        public List<ModQarEntry> ModQarEntries { get; set; }

        [XmlArray("FpkEntries")]
        public List<ModFpkEntry> ModFpkEntries { get; set; }
    }

    [XmlType("QarEntry")]
    public class ModQarEntry
    {
        [XmlAttribute("Hash")]
        public ulong Hash { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("Compressed")]
        public bool Compressed { get; set; }
    }

    [XmlType("FpkEntry")]
    public class ModFpkEntry
    {
        [XmlAttribute("FpkFile")]
        public string FpkFile { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }
    }
}