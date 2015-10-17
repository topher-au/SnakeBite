using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SnakeBite.Classes
{
    public class BackupManager
    {
        public BackupArchive backupData = new BackupArchive();

        public BackupManager()
        {
            if (File.Exists("backup\backup.xml"))
            {
                Load();
            }
            else
            {
                backupData.BackupFiles = new List<BackupFile>();
            }
        }

        public void Load()
        {
            XmlSerializer x = new XmlSerializer(typeof(BackupArchive));
            StreamReader s = new StreamReader("backup\\backup.xml");
            backupData = (BackupArchive)x.Deserialize(s);
            s.Close();
        }

        public void Save()
        {
            foreach (BackupFile backupFile in backupData.BackupFiles)
            {
                // clean up missing backup files
                if (!File.Exists(backupFile.BackupPath)) backupData.BackupFiles.Remove(backupFile);
            }
            XmlSerializer x = new XmlSerializer(typeof(BackupArchive));
            StreamWriter s = new StreamWriter("backup\\backup.xml");
            x.Serialize(s, backupData);
            s.Close();
        }

        public void AddFile(string PathOnDisk, string FilePath, string FpkFile = "")
        {
            string DestPath = "backup" + PathOnDisk.Substring(ModManager.ExtractedDatDir.Length);
            string DestDir = DestPath.Substring(0, DestPath.LastIndexOf("\\"));
            // if no backup exists, create one
            if (!File.Exists(DestPath))
            {
                if (!Directory.Exists(DestDir)) Directory.CreateDirectory(DestDir);
                File.Copy(PathOnDisk, DestPath);
                backupData.BackupFiles.Add(new BackupFile() { FilePath = FilePath, BackupPath = DestPath, FpkFile = FpkFile });
            }
        }

        public void RestoreFile(BackupFile BackupFile)
        {
            File.Copy(BackupFile.BackupPath, ModManager.ExtractedDatDir + BackupFile.BackupPath.Substring(BackupFile.BackupPath.IndexOf("\\")));
        }
    }

    [XmlType("BackupArchive")]
    public class BackupArchive
    {
        [XmlArray("BackupFiles")]
        public List<BackupFile> BackupFiles { get; set; }
    }

    [XmlType("BackupFile")]
    public class BackupFile
    {
        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("FpkFile")]
        public string FpkFile { get; set; }

        [XmlAttribute("BackupPath")]
        public string BackupPath { get; set; }
    }
}