using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Text;
using makebite;

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

        [XmlAttribute("MGSVersion")]
        public string MGSVersion { get; set; }

        [XmlAttribute("SBVersion")]
        public string SBVersion { get; set; }

        [XmlAttribute("Author")]
        public string Author { get; set; }

        [XmlAttribute("Website")]
        public string Website { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlArray("QarEntries")]
        public List<ModQarEntry> ModQarEntries { get; set; }

        [XmlArray("FpkEntries")]
        public List<ModFpkEntry> ModFpkEntries { get; set; }

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
            MGSVersion = loaded.MGSVersion;
            SBVersion = loaded.SBVersion;
            Author = loaded.Author;
            Website = loaded.Website;
            Description = loaded.Description;//.Replace("\n", "\r\n");

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

    public static class Tools
    {
        public static string ToWinPath(string Path)
        {
            return "\\" + Path.Replace("/", "\\").TrimStart('\\');
        }

        public static string ToQarPath(string Path)
        {
            return "/" + Path.Replace("\\", "/").TrimStart('/');
        }

        internal static string HashFile(string Filename)
        {
            byte[] hashBytes;
            using (var hashMD5 = MD5.Create())
            {
                using (var stream = File.OpenRead(Filename))
                {
                    hashBytes = hashMD5.ComputeHash(stream);
                }
            }

            StringBuilder hashBuilder = new StringBuilder(hashBytes.Length * 2);

            for (int i = 0; i < hashBytes.Length; i++)
                hashBuilder.Append(hashBytes[i].ToString("X2"));

            return hashBuilder.ToString();
        }
    }
}