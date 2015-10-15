using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnakeBite.GzsTool;
using SnakeBite;

namespace makebite
{
    public static class Build
    {
        public static List<string> ListFpkFolders(string PathName)
        {
            List<string> ListFpkFolders = new List<string>();
            foreach (string Directory in Directory.GetDirectories(PathName, "*.*", SearchOption.AllDirectories))
            {
                string FolderName = Directory.Substring(Directory.LastIndexOf("\\") + 1);
                if(FolderName.Contains("_"))
                {
                    string FolderType = FolderName.Substring(FolderName.LastIndexOf("_")+1).ToLower();
                    if (FolderType == "fpk" || FolderType == "fpkd") ListFpkFolders.Add(Directory);
                }
            }
            return ListFpkFolders;
        }

        public static List<string> ListQarFiles(string PathName)
        {
            List<string> ListQarFolders = new List<string>();
            List<string> ListQarFiles = new List<string>();

            // Get a list of all folders to check for files (no _fpk/_fpkd)
            foreach (string Directory in Directory.GetDirectories(PathName, "*.*", SearchOption.AllDirectories))
            {
                if (!Directory.Substring(PathName.Length).Contains("_fpk")) // ignore _fpk and _fpkd directories
                {
                    ListQarFolders.Add(Directory);
                }
            }

            // Check all folders for files
            foreach(string Folder in ListQarFolders)
            {
                foreach(string FileName in Directory.GetFiles(Folder))
                {
                    ListQarFiles.Add(FileName);
                }
            }

            return ListQarFiles;
        }

        public static void BuildFpk(string FpkFolder)
        {
            string FpkName = FpkFolder.Substring(FpkFolder.LastIndexOf("\\") + 1).Replace("_fpk", ".fpk");
            string FpkBuildFolder = FpkFolder.Substring(0, FpkFolder.TrimEnd('\\').LastIndexOf("\\"));
            string FpkXmlFile = FpkBuildFolder + "\\" + FpkName + ".xml";
            string FpkFile = FpkBuildFolder + "\\" + FpkName;
            string FpkType = FpkFolder.Substring(FpkFolder.LastIndexOf("_") + 1);
            FpkFile gzsFpk = new SnakeBite.GzsTool.FpkFile();
            
            if(FpkType == "fpk")
            {
                gzsFpk.FpkType = "Fpk";
            } else if(FpkType == "fpkd")
            {
                gzsFpk.FpkType = "Fpkd";
            }
            gzsFpk.Name = FpkName;
            gzsFpk.FpkEntries = new List<FpkEntry>();

            foreach(string FileName in Directory.GetFiles(FpkFolder, "*.*", SearchOption.AllDirectories))
            {
                string xmlFileName = FileName.Substring(FpkFolder.Length).Replace("\\", "/");
                gzsFpk.FpkEntries.Add(new FpkEntry() { FilePath = xmlFileName });

            }

            gzsFpk.WriteToFile(FpkXmlFile);
            SnakeBite.GzsTool.GzsTool.Run(FpkXmlFile);
        }
    }
}
