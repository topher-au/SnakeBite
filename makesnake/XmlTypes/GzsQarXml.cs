using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SnakeBite
{
    [XmlType("ArchiveFile")] // ArchiveFile class used to import XML data
    public abstract class ArchiveFile
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Flags")]
        public uint Flags { get; set; }
    }

    [XmlType("QarFile")]
    public class GzsQarXml : ArchiveFile
    {
        [XmlArray("Entries")]
        public List<GzsQarEntry> GzsQarEntries { get; set; }

        public void LoadFromFile(string Filename)
        {
            // Deserialize object from GzsTool XML data
            XmlSerializer xSerializer = new XmlSerializer(typeof(ArchiveFile), new[] { typeof(GzsQarXml) });
            FileStream xStream = new FileStream(Filename, FileMode.Open);
            XmlReader xReader = XmlReader.Create(xStream);
            GzsQarXml qarXml = (GzsQarXml)xSerializer.Deserialize(xReader);
            xStream.Close();

            // Clear existing entries and reload
            GzsQarEntries = new List<GzsQarEntry>();
            foreach (GzsQarEntry qarEntry in qarXml.GzsQarEntries)
            {
                GzsQarEntries.Add(qarEntry);
            }
        }
    }

    [XmlType("Entry")]
    public class GzsQarEntry
    {
        [XmlAttribute("Hash")]
        public ulong Hash { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("Compressed")]
        public bool Compressed { get; set; }
    }
}