using SnakeBite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using GzsTool.Core.Utility;

namespace GzsTool
{
    public static class GzsApp
    {
        // Runs GzsTool with specified paramaters
        public static void Run(string args, bool wait = true)
        {
            Process gzsProcess = new Process();
            gzsProcess.StartInfo.UseShellExecute = false;
            gzsProcess.StartInfo.CreateNoWindow = true;
            gzsProcess.StartInfo.FileName = "GzsTool.exe";
            gzsProcess.StartInfo.Arguments = "\"" + args + "\"";
            gzsProcess.StartInfo.RedirectStandardError = true;
            gzsProcess.Start();

            if (wait)
            {
                while (!gzsProcess.HasExited) { Application.DoEvents(); }

                if (gzsProcess.ExitCode != 0)
                {
                    string gzsError = gzsProcess.StandardError.ReadToEnd();
                    MessageBox.Show(String.Format("Error running GzsTool!\n\n{0}", gzsError));
                }
            }
        }

        // Runs GzsTool with specified paramaters
        public static void ExtractSingle(string QarFile, ulong FileHash, bool wait = true)
        {
            Process gzsProcess = new Process();
            gzsProcess.StartInfo.UseShellExecute = false;
            gzsProcess.StartInfo.CreateNoWindow = true;
            gzsProcess.StartInfo.FileName = "GzsTool.exe";
            gzsProcess.StartInfo.Arguments = "\"" + QarFile + "\" \"" + FileHash.ToString() + "\"";
            gzsProcess.StartInfo.RedirectStandardError = true;
            gzsProcess.Start();

            if (wait)
            {
                while (!gzsProcess.HasExited) { Application.DoEvents(); }

                if (gzsProcess.ExitCode != 0)
                {
                    string gzsError = gzsProcess.StandardError.ReadToEnd();
                    MessageBox.Show(String.Format("Error running GzsTool!\n\n{0}", gzsError));
                }
            }
        }
    }

    [XmlType("ArchiveFile")] // ArchiveFile class used to import XML data
    public abstract class ArchiveFile
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }

    [XmlType("Entry")]
    public class FpkEntry
    {
        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }
    }

    [XmlType("FpkFile")]
    public class FpkFile : ArchiveFile
    {
        [XmlArray("Entries")]
        public List<FpkEntry> FpkEntries { get; set; }

        [XmlAttribute("FpkType")]
        public string FpkType { get; set; }

        public void ReadXml(string Filename)
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

        public void WriteXml(string Filename)
        {
            XmlSerializer x = new XmlSerializer(typeof(ArchiveFile), new[] { typeof(FpkFile) });
            StreamWriter s = new StreamWriter(Filename);
            x.Serialize(s, this);
            s.Close();
        }
    }

    [XmlType("Entry")]
    public class QarEntry
    {
        [XmlAttribute("Compressed")]
        public bool Compressed { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("Hash")]
        public ulong Hash { get; set; }
    }

    [XmlType("QarFile")]
    public class QarFile : ArchiveFile
    {
        [XmlAttribute("Flags")]
        public uint Flags { get; set; }

        [XmlArray("Entries")]
        public List<QarEntry> QarEntries { get; set; }

        public void ReadXml(string Filename)
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

        public void WriteXml(string Filename)
        {
            foreach (QarEntry Entry in QarEntries)
            {
                // regenerate hash for file
                string filePath = Tools.ToQarPath(Entry.FilePath);
                if (filePath.Substring(1).Contains("/"))
                {
                    // generate normal hash
                    ulong hash = Hashing.HashFileNameWithExtension(filePath);
                    Entry.Hash = hash;
                }
                else
                {
                    // get hash from filename
                    string fileName = filePath.TrimStart('/');
                    string fileNoExt = fileName.Substring(0, fileName.IndexOf("."));
                    string fileExt = fileName.Substring(fileName.IndexOf(".") + 1);

                    ulong Hash;
                    bool tryParseHash = ulong.TryParse(fileNoExt, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out Hash);
                    if(tryParseHash)
                    {
                        ulong ExtHash = Hashing.HashFileName(fileExt, false) & 0x1FFF;
                        ulong XH = (ExtHash << 51);
                        Hash = XH | Hash;
                    } else
                    {
                        Hash = Hashing.HashFileNameWithExtension(filePath);
                    }

                    Entry.Hash = Hash;
                }
                string ext = Path.GetExtension(Entry.FilePath);
                if (ext == ".fpk" || ext == ".fpkd")
                {
                    Entry.Compressed = true;
                }
            }
            XmlSerializer x = new XmlSerializer(typeof(ArchiveFile), new[] { typeof(QarFile) });
            StreamWriter s = new StreamWriter(Filename);
            x.Serialize(s, this);
            s.Close();
        }
    }
}