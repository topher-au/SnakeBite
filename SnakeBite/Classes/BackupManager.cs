using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeBite.Classes
{
    public class BackupManager
    {
        public BackupArchive backupData = new BackupArchive();

        public BackupManager()
        {
            if(File.Exists("backup\backup.xml"))
            {
                Load();
            } else
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
            foreach(BackupFile bFile in backupData.BackupFiles)
            {
                // clean up missing backup files
                if (!File.Exists(bFile.BackupPath)) backupData.BackupFiles.Remove(bFile);
            }
            XmlSerializer x = new XmlSerializer(typeof(BackupArchive));
            StreamWriter s = new StreamWriter("backup\\backup.xml");
            x.Serialize(s, backupData);
            s.Close();
        }

        public void AddFile(string PathOnDisk, string FilePath, string FpkFile = "")
        {
            string DestPath = "backup" + PathOnDisk.Substring(ModManager.GameArchiveDir.Length);
            string DestDir = DestPath.Substring(0, DestPath.LastIndexOf("\\"));
            // if no backup exists, create one
            if(!File.Exists(DestPath))
            {
                if (!Directory.Exists(DestDir)) Directory.CreateDirectory(DestDir);
                File.Copy(PathOnDisk, DestPath);
                backupData.BackupFiles.Add(new BackupFile() { FilePath = FilePath, BackupPath = DestPath, FpkFile = FpkFile });
            }
        }

        public void RestoreFile(BackupFile BackupFile)
        {
            File.Copy(BackupFile.BackupPath, ModManager.GameArchiveDir + BackupFile.BackupPath.Substring(BackupFile.BackupPath.IndexOf("\\")));
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
