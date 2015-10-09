using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using GzsTool.Utility;
using System;

namespace SnakeBite.GzsTool
{
    public static class GzsTool
    {
        /// <summary>
        /// Runs an instance of GzsTool with the specified parameters
        /// </summary>
        /// <param name="args">arguments to be passed to GzsTool</param>
        /// <param name="wait">whether to wait for GzsTool to close before returning</param>
        public static void Run(string args, bool wait = true)
        {
            Process gzsProcess = new Process();
            gzsProcess.StartInfo.UseShellExecute = false;
            gzsProcess.StartInfo.CreateNoWindow = true;
            gzsProcess.StartInfo.FileName = "GzsTool.exe";
            gzsProcess.StartInfo.Arguments = args;
            gzsProcess.Start();

            if (wait)
            {
                while (!gzsProcess.HasExited) { Application.DoEvents(); }
            }
        }
    }

    [XmlType("ArchiveFile")] // ArchiveFile class used to import XML data
    public abstract class ArchiveFile
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }

    [XmlType("QarFile")]
    public class QarFile : ArchiveFile
    {
        [XmlAttribute("Flags")]
        public uint Flags { get; set; }

        [XmlArray("Entries")]
        public List<QarEntry> QarEntries { get; set; }

        public void LoadFromFile(string Filename)
        {
            // Deserialize object from GzsTool XML data
            XmlSerializer xSerializer = new XmlSerializer(typeof(ArchiveFile), new[] { typeof(QarFile), typeof(ArchiveFile) });
            FileStream xStream = new FileStream(Filename, FileMode.Open);
            XmlReader xReader = XmlReader.Create(xStream);
            QarFile fpkXml = (QarFile)xSerializer.Deserialize(xReader);

            xStream.Close();
            Name = fpkXml.Name;
            Flags = fpkXml.Flags;

            // Clear existing entries and reload
            QarEntries = new List<QarEntry>();
            foreach (QarEntry qarEntry in fpkXml.QarEntries)
            {
                QarEntries.Add(qarEntry);
            }
        }

        public void WriteToFile(string Filename)
        {
            foreach(QarEntry qarFile in QarEntries)
            {
                // regenerate hash for file
                if(qarFile.FilePath.Substring(1).Contains("/"))
                {
                    // generate normal hash
                    string fileName = "/" + qarFile.FilePath.Replace("\\","/");
                    ulong hash = Hashing.HashFileNameWithExtension(fileName);
                    qarFile.Hash = hash;
                } else {
                    // generate extension only hash
                    string fileName = "/" + qarFile.FilePath.Replace("\\", "/");
                    ulong hash = Hashing.HashFileNameExtensionOnly(fileName);
                    qarFile.Hash = hash;
                }
                string ext = qarFile.FilePath.Substring(qarFile.FilePath.LastIndexOf(".")+1);
                if (ext == "fpk" || ext == "fpkd")
                {
                    qarFile.Compressed = true;
                }
            }
            XmlSerializer x = new XmlSerializer(typeof(ArchiveFile), new[] { typeof(QarFile) });
            StreamWriter s = new StreamWriter(Filename);
            x.Serialize(s, this);
            s.Close();
        }
    }

    [XmlType("Entry")]
    public class QarEntry
    {
        [XmlAttribute("Hash")]
        public ulong Hash { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("Compressed")]
        public bool Compressed { get; set; }
    }

    [XmlType("FpkFile")]
    public class FpkFile : ArchiveFile
    {
        [XmlAttribute("FpkType")]
        public string FpkType { get; set; }

        [XmlArray("Entries")]
        public List<FpkEntry> FpkEntries { get; set; }

        public void LoadFromFile(string Filename)
        {
            // Deserialize object from GzsTool XML data
            XmlSerializer xSerializer = new XmlSerializer(typeof(ArchiveFile), new[] { typeof(FpkFile), typeof(ArchiveFile) });
            FileStream xStream = new FileStream(Filename, FileMode.Open);
            XmlReader xReader = XmlReader.Create(xStream);
            FpkFile fpkXml = (FpkFile)xSerializer.Deserialize(xReader);
            xStream.Close();

            Name = fpkXml.Name;
            FpkType = fpkXml.FpkType;

            // Clear existing entries and reload
            FpkEntries = new List<FpkEntry>();
            foreach (FpkEntry qarEntry in fpkXml.FpkEntries)
            {
                FpkEntries.Add(qarEntry);
            }
        }

        public void WriteToFile(string Filename)
        {
            XmlSerializer x = new XmlSerializer(typeof(FpkFile), new[] { typeof(ArchiveFile) });
            StreamWriter s = new StreamWriter(Filename);
            x.Serialize(s, this);
            s.Close();
        }
    }

    [XmlType("Entry")]
    public class FpkEntry
    {
        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }
    }
}