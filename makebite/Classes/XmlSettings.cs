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
    }

    [XmlType("GameData")]
    public class GameData
    {
        public GameData()
        {
            GameQarEntries = new List<ModQarEntry>();
            GameFpkEntries = new List<ModFpkEntry>();
        }

        [XmlAttribute("DatHash")]
        public string DatHash { get; set; }

        [XmlArray("QarEntries")]
        public List<ModQarEntry> GameQarEntries { get; set; } = new List<ModQarEntry>();

        [XmlArray("FpkEntries")]
        public List<ModFpkEntry> GameFpkEntries { get; set; } = new List<ModFpkEntry>();
    }

    [XmlType("ModEntry")]
    public class ModEntry
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlElement("MGSVersion")]
        public SerialVersion MGSVersion { get; set; } = new SerialVersion();

        [XmlElement("SBVersion")]
        public SerialVersion SBVersion { get; set; } = new SerialVersion();

        [XmlAttribute("Author")]
        public string Author { get; set; }

        [XmlAttribute("Website")]
        public string Website { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlArray("QarEntries")]
        public List<ModQarEntry> ModQarEntries { get; set; } = new List<ModQarEntry>();

        [XmlArray("FpkEntries")]
        public List<ModFpkEntry> ModFpkEntries { get; set; } = new List<ModFpkEntry>();

        [XmlArray("FileEntries")]
        public List<ModFileEntry> ModFileEntries { get; set; } = new List<ModFileEntry>();

        public void ReadFromFile(string Filename)
        {
            // Read mod metadata from xml

            if (!File.Exists(Filename)) return;

            XmlSerializer x = new XmlSerializer(typeof(ModEntry));
            StreamReader s = new StreamReader(Filename);
            System.Xml.XmlReader xr = System.Xml.XmlReader.Create(s);

            ModEntry loaded = (ModEntry)x.Deserialize(xr);

            Name = loaded.Name;
            Version = loaded.Version;
            try
            {
                MGSVersion.Version = loaded.MGSVersion.AsString();
                SBVersion.Version = loaded.SBVersion.AsString();
            }
            catch
            {
                if (MGSVersion == null)
                {
                    MGSVersion = new SerialVersion();
                    MGSVersion.Version = "0.0.0.0";
                }
                if (SBVersion == null)
                {
                    SBVersion = new SerialVersion();
                    SBVersion.Version = "0.0.0.0";
                }
            }
            
            Author = loaded.Author;
            Website = loaded.Website;
            Description = loaded.Description;

            ModQarEntries = loaded.ModQarEntries;
            ModFpkEntries = loaded.ModFpkEntries;

            s.Close();
        }

        public void SaveToFile(string Filename)
        {
            // Write mod metadata to XML

            if (File.Exists(Filename)) File.Delete(Filename);

            XmlSerializer x = new XmlSerializer(typeof(ModEntry), new[] { typeof(ModEntry) });
            StreamWriter s = new StreamWriter(Filename);
            x.Serialize(s, this);
            s.Close();
        }
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

        [XmlAttribute("ContentHash")]
        public string ContentHash { get; set; }
    }

    [XmlType("FpkEntry")]
    public class ModFpkEntry
    {
        [XmlAttribute("FpkFile")]
        public string FpkFile { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("ContentHash")]
        public string ContentHash { get; set; }
    }

    [XmlType("FileEntry")]
    public class ModFileEntry {
        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("ContentHash")]
        public string ContentHash { get; set; }
    }
}