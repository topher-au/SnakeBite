using GzsTool;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite;
using System.Collections.Generic;
using System.IO;
using GzsTool.Core.Utility;
using GzsTool.Core.Fpk;
using SnakeBite.GzsTool;

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
                if (FolderName.Contains("_"))
                {
                    string FolderType = FolderName.Substring(FolderName.LastIndexOf("_") + 1).ToLower();
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
            ListQarFolders.Add(PathName);
            // Check all folders for files
            foreach (string Folder in ListQarFolders)
            {
                foreach (string FileName in Directory.GetFiles(Folder))
                {
                    string FilePath = FileName.Substring(Folder.Length);
                    if (!FilePath.Contains("metadata.xml") && !FilePath.Contains("readme.txt") && // ignore xml metadata and readme
                        Tools.IsValidFile(FilePath)) // only add valid files
                        ListQarFiles.Add(FileName);
                }
            }

            return ListQarFiles;
        }

        public static List<ModFpkEntry> BuildFpk(string FpkFolder, string rootDir)
        {
            string FpkName = FpkFolder.Substring(FpkFolder.LastIndexOf("\\") + 1).Replace("_fpk", ".fpk");
            string FpkBuildFolder = FpkFolder.Substring(0, FpkFolder.TrimEnd('\\').LastIndexOf("\\"));
            //string FpkXmlFile = FpkBuildFolder + "\\" + FpkName + ".xml";
            string FpkFile = FpkBuildFolder + "\\" + FpkName;
            string FpkType = FpkFolder.Substring(FpkFolder.LastIndexOf("_") + 1);

            List<string> fpkFiles = new List<string>();

            List<ModFpkEntry> fpkList = new List<ModFpkEntry>();

            foreach (string FileName in Directory.GetFiles(FpkFolder, "*.*", SearchOption.AllDirectories))
            {
                string xmlFileName = FileName.Substring(FpkFolder.Length).Replace("\\", "/");
                fpkList.Add(new ModFpkEntry() { FilePath = xmlFileName, FpkFile = Tools.ToQarPath(FpkFile.Substring(rootDir.Length)), ContentHash = Tools.GetMd5Hash(FileName) });
                fpkFiles.Add(xmlFileName);
            }

            GzsLib.WriteFpkArchive(FpkFile, FpkFolder, fpkFiles);

            return fpkList;
        }

        public static void BuildArchive(string SourceDir, ModEntry metaData, string OutputFile)
        {
            string buildDir = Directory.GetCurrentDirectory() + "\\build";
            if (Directory.Exists(buildDir)) Directory.Delete(buildDir, true);

            Directory.CreateDirectory("_build");

            // check for FPKs that must be built and build
            metaData.ModFpkEntries = new List<ModFpkEntry>();
            List<string> builtFpks = new List<string>();
            foreach (string FpkDir in ListFpkFolders(SourceDir))
            {
                foreach (ModFpkEntry fpkEntry in BuildFpk(FpkDir, SourceDir))
                {
                    metaData.ModFpkEntries.Add(fpkEntry);
                    if (!builtFpks.Contains(fpkEntry.FpkFile)) builtFpks.Add(fpkEntry.FpkFile);
                }
            }

            // check for other FPKs and build fpkentry data
            foreach(string SourceFile in Directory.GetFiles(SourceDir, "*.fpk*", SearchOption.AllDirectories))
            {
                string FileName = Tools.ToQarPath(SourceFile.Substring(SourceDir.Length));
                if(!builtFpks.Contains(FileName))
                {
                    // unpack FPK and build FPK list

                    foreach(string file in GzsLib.ListArchiveContents<FpkFile>(SourceFile))
                    {
                        string fpkDir = Tools.ToWinPath(FileName.Replace(".fpk", "_fpk"));
                        metaData.ModFpkEntries.Add(new ModFpkEntry() { FilePath = file, FpkFile = FileName, ContentHash = Tools.GetMd5Hash(Path.Combine(SourceDir, fpkDir, Tools.ToWinPath(file))) });
                    }
                }
            }

            // build QAR entries
            metaData.ModQarEntries = new List<ModQarEntry>();
            foreach (string qarFile in ListQarFiles(SourceDir))
            {
                string subDir = qarFile.Substring(0, qarFile.LastIndexOf("\\")).Substring(SourceDir.Length).TrimStart('\\'); // the subdirectory for XML output
                string qarFilePath = Tools.ToQarPath(qarFile.Substring(SourceDir.Length));
                if (!Directory.Exists(Path.Combine("_build",subDir))) Directory.CreateDirectory(Path.Combine("_build",subDir)); // create file structure
                File.Copy(qarFile, Path.Combine("_build",Tools.ToWinPath(qarFilePath)), true);

                ulong hash = Tools.NameToHash(qarFilePath);
                metaData.ModQarEntries.Add(new ModQarEntry() { FilePath = qarFilePath, Compressed = qarFile.Substring(qarFile.LastIndexOf(".") + 1).Contains("fpk") ? true : false, ContentHash = Tools.GetMd5Hash(qarFile), Hash = hash });
            }

            metaData.SBVersion = "500"; // 0.4.2.0

            metaData.SaveToFile("_build\\metadata.xml");

            // build archive
            FastZip zipper = new FastZip();
            zipper.CreateZip(OutputFile, "_build", true, "(.*?)");

            Directory.Delete("_build", true);
        }
    }
}