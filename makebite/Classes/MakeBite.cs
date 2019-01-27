using GzsTool.Core.Fpk;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace makebite
{
    public static class Build
    {
        public static string SnakeBiteVersionStr =  "0.9.1.0";
        public static string MGSVVersionStr =       "1.0.15.0";

        static string ExternalDirName = "GameDir";
        static List<string> archiveFolders = new List<string> {
            "_fpk",
            "_fpkd",
            "_pftx",
        };

        private static bool IsArchiveFolder(string PathName, string Directory)
        {
            foreach (var archiveFolderExt in archiveFolders)
            {
                if (Directory.Substring(PathName.Length).Contains(archiveFolderExt))
                {
                    return true;
                }
            }
            return false;
        }

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
                if (IsArchiveFolder(PathName, Directory))
                {
                    continue;
                }

                if (Directory.Substring(PathName.Length).Contains(ExternalDirName))// tex KLUDGE ignore MGS_TPP 
                {
                    continue;
                }

                ListQarFolders.Add(Directory);
            }
            ListQarFolders.Add(PathName);
            // Check all folders for files
            foreach (string Folder in ListQarFolders)
            {
                foreach (string FileName in Directory.GetFiles(Folder))
                {
                    string FilePath = FileName.Substring(Folder.Length);
                    if (!GzsLib.IsExtensionValidForArchive(FileName, ".dat"))
                    {
                        Debug.LogLine($"[BuildArchive] {FileName} is not a valid file for a .dat archive.");
                        continue;
                    }

                    if (!FilePath.Contains("metadata.xml") && !FilePath.Contains("readme.txt"))// ignore xml metadata and readme
                    {
                        ListQarFiles.Add(FileName);
                    }

                }
            }

            return ListQarFiles;
        }

        public static List<string> ListExternalFiles(string PathName)
        {
            List<string> ListFolders = new List<string>();
            List<string> ListFiles = new List<string>();

            // Get a list of all folders to check for files (no _fpk/_fpkd)
            foreach (string Directory in Directory.GetDirectories(PathName, "*.*", SearchOption.AllDirectories))
            {
                if (IsArchiveFolder(PathName, Directory))
                {
                    continue;
                }

                if (!Directory.Substring(PathName.Length).Contains(ExternalDirName)) 
                {
                    continue;
                }

                ListFolders.Add(Directory);
            }

            // Check all folders for files
            foreach (string Folder in ListFolders)
            {
                foreach (string FileName in Directory.GetFiles(Folder))
                {
                    bool skipFile = false;
                    foreach (string ignoreFile in Tools.ignoreFileList)
                    {
                        if (FileName.Contains(ignoreFile))
                        {
                            skipFile = true;
                        }
                    }
                    /*
                    foreach (string ignoreExt in ignoreExtList) {
                        if (FileName.Contains(ignoreExt)) {
                            skipFile = true;
                        }
                    }
                    */
                    if (skipFile) continue;
                    string FilePath = FileName.Substring(Folder.Length);
                    if (!FilePath.Contains("metadata.xml")) // ignore xml metadata
                        ListFiles.Add(FileName);
                }
            }

            return ListFiles;
        }

        public static List<ModFpkEntry> BuildFpk(string FpkFolder, string rootDir)
        {
            Debug.LogLine($"[BuildFpk] {FpkFolder}.");
            string FpkName = FpkFolder.Substring(FpkFolder.LastIndexOf("\\") + 1).Replace("_fpk", ".fpk");
            string FpkBuildFolder = FpkFolder.Substring(0, FpkFolder.TrimEnd('\\').LastIndexOf("\\"));
            //string FpkXmlFile = FpkBuildFolder + "\\" + FpkName + ".xml";
            string FpkFile = FpkBuildFolder + "\\" + FpkName;
            string FpkType = FpkFolder.Substring(FpkFolder.LastIndexOf("_") + 1);

            List<string> fpkFiles = new List<string>();

            List<ModFpkEntry> fpkList = new List<ModFpkEntry>();

            foreach (string FileName in Directory.GetFiles(FpkFolder, "*.*", SearchOption.AllDirectories))
            {
                if (!GzsLib.IsExtensionValidForArchive(FileName, FpkName))
                {
                    Debug.LogLine($"[BuildFpk] {FileName} is not a valid file for a {Path.GetExtension(FpkName)} archive.");
                    continue;
                }

                string inQarName = FileName.Substring(FpkFolder.Length).Replace("\\", "/");
                fpkList.Add(new ModFpkEntry() {
                    FilePath = inQarName,
                    FpkFile = Tools.ToQarPath(FpkFile.Substring(rootDir.Length)),
                    ContentHash = Tools.GetMd5Hash(FileName)
                });
                fpkFiles.Add(inQarName);
            }

            List<string> fpkFilesSorted = GzsLib.SortFpksFiles(FpkType, fpkFiles);

            GzsLib.WriteFpkArchive(FpkFile, FpkFolder, fpkFilesSorted);

            return fpkList;
        }

        public static void BuildArchive(string SourceDir, ModEntry metaData, string outputFilePath)
        {
            Debug.LogLine($"[BuildArchive] {SourceDir}.");
            HashingExtended.ReadDictionary();
            string buildDir = Directory.GetCurrentDirectory() + "\\build";
            if (Directory.Exists(buildDir)) Directory.Delete(buildDir, true);

            Directory.CreateDirectory("_build");

            List<string> fpkFolders = ListFpkFolders(SourceDir);
            for(int i = fpkFolders.Count - 1; i >= 0; i--)
            {
                string fpkFolder = fpkFolders[i].Substring(SourceDir.Length + 1);
                if (!fpkFolder.StartsWith("Assets"))
                {
                    string updatedFolderName = HashingExtended.UpdateName(fpkFolder.Replace("_fpk", ".fpk"));
                    if(updatedFolderName != null)
                    {
                        updatedFolderName = SourceDir + updatedFolderName.Replace(".fpk", "_fpk").Replace('/','\\');
                        if (fpkFolders.Contains(updatedFolderName))
                        {
                            fpkFolders.Remove(fpkFolders[i]);
                        }
                    }
                }
            }

            List<string> fpkFiles = Directory.GetFiles(SourceDir, "*.fpk*", SearchOption.AllDirectories).ToList();
            for (int i = fpkFiles.Count - 1; i >= 0; i--)
            {
                string fpkFile = fpkFiles[i].Substring(SourceDir.Length + 1);
                if (!fpkFile.StartsWith("Assets"))
                {
                    string updatedFileName = HashingExtended.UpdateName(fpkFile);
                    if(updatedFileName != null)
                    {
                        updatedFileName = SourceDir + updatedFileName.Replace('/', '\\');
                        if (fpkFiles.Contains(updatedFileName))
                        {
                            fpkFiles.Remove(fpkFiles[i]);
                        }
                    }
                }
            }

            List<string> qarFiles = ListQarFiles(SourceDir);
            for (int i = qarFiles.Count - 1; i >= 0; i--)
            {
                string qarFile = qarFiles[i].Substring(SourceDir.Length + 1);
                if(!qarFile.StartsWith("Assets"))
                {
                    string updatedQarName = HashingExtended.UpdateName(qarFile);
                    if (updatedQarName != null)
                    {
                        updatedQarName = SourceDir + updatedQarName.Replace('/', '\\');
                        if (qarFiles.Contains(updatedQarName))
                        {
                            MessageBox.Show(string.Format("{0} was not added to the build, because {1} (the unhashed filename of {0}) already exists in the mod directory.", Path.GetFileName(qarFiles[i]), Path.GetFileName(updatedQarName)));
                            qarFiles.Remove(qarFiles[i]);
                        }
                    }
                }
            }

            // check for FPKs that must be built and build
            metaData.ModFpkEntries = new List<ModFpkEntry>();
            List<string> builtFpks = new List<string>();
            foreach (string FpkFullDir in fpkFolders)
            {
                foreach (ModFpkEntry fpkEntry in BuildFpk(FpkFullDir, SourceDir))
                {
                    metaData.ModFpkEntries.Add(fpkEntry);
                    if (!builtFpks.Contains(fpkEntry.FpkFile)) builtFpks.Add(fpkEntry.FpkFile);
                }
            }

            // check for other FPKs and build fpkentry data
            foreach (string SourceFile in fpkFiles)
            {
                //tex chunk0\Assets\tpp\pack\collectible\common\col_common_tpp_fpk\Assets\tpp\pack\resident\resident00.fpkl is the only fpkl, don't know what a fpkl is, but gzcore crashes on it.
                if (SourceFile.EndsWith(".fpkl") || SourceFile.EndsWith(".xml"))
                {
                    continue;
                }
                string FileName = Tools.ToQarPath(SourceFile.Substring(SourceDir.Length));
                if (!builtFpks.Contains(FileName))
                {
                    // unpack FPK and build FPK list
                    string fpkDir = Tools.ToWinPath(FileName.Replace(".fpk", "_fpk"));
                    string fpkFullDir = Path.Combine(SourceDir, fpkDir);
                    if (!Directory.Exists(fpkFullDir))
                    {
                        GzsLib.ExtractArchive<FpkFile>(SourceFile, fpkFullDir);
                    }

                    var fpkContents = GzsLib.ListArchiveContents<FpkFile>(SourceFile);
                    foreach (string file in fpkContents)
                    {
                        if (!GzsLib.IsExtensionValidForArchive(file, fpkDir))
                        {
                            Debug.LogLine($"[BuildArchive] {file} is not a valid file for a {Path.GetExtension(fpkDir)} archive.");
                            continue;
                        }

                        metaData.ModFpkEntries.Add(new ModFpkEntry() {
                            FilePath = file,
                            FpkFile = FileName,
                            ContentHash = Tools.GetMd5Hash(Path.Combine(SourceDir, fpkDir, Tools.ToWinPath(file)))
                        });
                    }
                }
            }

            // build QAR entries
            metaData.ModQarEntries = new List<ModQarEntry>();
            foreach (string qarFile in qarFiles)
            {
                string subDir = qarFile.Substring(0, qarFile.LastIndexOf("\\")).Substring(SourceDir.Length).TrimStart('\\'); // the subdirectory for XML output
                string qarFilePath = Tools.ToQarPath(qarFile.Substring(SourceDir.Length));
                
                if (!Directory.Exists(Path.Combine("_build", subDir))) Directory.CreateDirectory(Path.Combine("_build", subDir)); // create file structure
                File.Copy(qarFile, Path.Combine("_build", Tools.ToWinPath(qarFilePath)), true);

                ulong hash = Tools.NameToHash(qarFilePath);
                metaData.ModQarEntries.Add(new ModQarEntry() {
                    FilePath = qarFilePath,
                    Compressed = qarFile.EndsWith(".fpk") || qarFile.EndsWith(".fpkd") ? true : false,
                    ContentHash = Tools.GetMd5Hash(qarFile), Hash = hash
                });
            }

            //tex build external entries
            metaData.ModFileEntries = new List<ModFileEntry>();
            var externalFiles = ListExternalFiles(SourceDir);
            foreach (string externalFile in externalFiles)
            {
                string subDir = externalFile.Substring(0, externalFile.LastIndexOf("\\")).Substring(SourceDir.Length).TrimStart('\\'); // the subdirectory for XML output
                string externalFilePath = Tools.ToQarPath(externalFile.Substring(SourceDir.Length));

                if (!Directory.Exists(Path.Combine("_build", subDir))) Directory.CreateDirectory(Path.Combine("_build", subDir)); // create file structure
                File.Copy(externalFile, Path.Combine("_build", Tools.ToWinPath(externalFilePath)), true);
                string strip = "/" + ExternalDirName;
                if (externalFilePath.StartsWith(strip))
                {
                    externalFilePath = externalFilePath.Substring(strip.Length);
                }
                //ulong hash = Tools.NameToHash(qarFilePath);
                metaData.ModFileEntries.Add(new ModFileEntry() { FilePath = externalFilePath, ContentHash = Tools.GetMd5Hash(externalFile) });
            }

            metaData.SBVersion.Version = SnakeBiteVersionStr;

            metaData.SaveToFile("_build\\metadata.xml");

            // build archive
            FastZip zipper = new FastZip();
            zipper.CreateZip(outputFilePath, "_build", true, "(.*?)");

            Directory.Delete("_build", true);
        }
    }
}