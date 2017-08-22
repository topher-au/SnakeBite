using GzsTool.Core.Fpk;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace makebite
{
    public static class Build
    {
        public static string SnakeBiteVersionStr =  "0.9.0.2";
        public static string MGSVVersionStr =       "1.0.12.0";

        static string ExternalDirName = "GameDir";
        internal static List<string> ignoreFileList = new List<string>(new string[] {
            "mgsvtpp.exe",
            "mgsvmgo.exe",
            "steam_api64.dll",
            "steam_appid.txt",
            "version_info.txt",
            "chunk0.dat",
            "chunk1.dat",
            "chunk2.dat",
            "chunk3.dat",
            "chunk0.dat",
            "texture0.dat",
            "texture1.dat",
            "texture2.dat",
            "texture3.dat",
            "texture4.dat",
            "00.dat",
            "01.dat",
            "snakebite.xml"
        });

        internal static List<string> ignoreExtList = new List<string>(new string[] {
            ".exe",
            ".dat",
        });

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
                    if (!Directory.Substring(PathName.Length).Contains(ExternalDirName)) {// tex KLUDGE ignore MGS_TPP 
                    ListQarFolders.Add(Directory);
                }
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

        public static List<string> ListExternalFiles(string PathName) {
            List<string> ListFolders = new List<string>();
            List<string> ListFiles = new List<string>();

            // Get a list of all folders to check for files (no _fpk/_fpkd)
            foreach (string Directory in Directory.GetDirectories(PathName, "*.*", SearchOption.AllDirectories)) {
                if (!Directory.Substring(PathName.Length).Contains("_fpk")) // ignore _fpk and _fpkd directories
                {
                    if (Directory.Substring(PathName.Length).Contains(ExternalDirName)) {// tex KLUDGE ignore MGS_TPP 
                        ListFolders.Add(Directory);
                    }
                }
            }

            // Check all folders for files
            foreach (string Folder in ListFolders) {
                foreach (string FileName in Directory.GetFiles(Folder)) {
                    bool skipFile = false;
                    foreach (string ignoreFile in ignoreFileList) {
                        if (FileName.Contains(ignoreFile)) {
                            skipFile = true;
                        }
                    }
                    foreach (string ignoreExt in ignoreExtList) {
                        if (FileName.Contains(ignoreExt)) {
                            skipFile = true;
                        }
                    }
                    if (skipFile) continue;
                    string FilePath = FileName.Substring(Folder.Length);
                    if (!FilePath.Contains("metadata.xml") && // ignore xml metadata
                        Tools.IsValidFile(FilePath)) // only add valid files
                        ListFiles.Add(FileName);
                }
            }

            return ListFiles;
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

            List<string> fpkFilesSorted = fpkFiles.OrderBy(fileName => Path.GetExtension(fileName)).ThenBy(fileName => fileName).ToList();

            //tex all positions reletively solid except for "bnd", analysis from my ExtensionOrder.lua puts it somewhere between veh and tgt.
            //have put it between des and tgt in line with init.lua RegisterPackageExtensionInfo call
            // RegisterPackageExtensionInfo call seems to mostly match my derived order in reverse - however clo doesnt fit the order and lng isn't in its table.
			//snakebite seems to honor packed order.
			List<string> fpkdExtensionsOrder = new List<string> { "fox2", "evf", "parts", "vfxlb", "vfx", "vfxlf", "veh", "frld", "des", "bnd", "tgt", "phsd", "ph", "sim", "clo", "fsd", "sdf", "lua", "lng" };

            Dictionary<string, List<string>> filesByExtension = new Dictionary<string, List<string>>();

            if (fpkFilesSorted.Count() > 1) {
                if (FpkType == "fpkd") {
                    foreach (var fileName in fpkFilesSorted) {
                        var extension = Path.GetExtension(fileName).Substring(1);
                        List<string> extensionFiles = null;
                        filesByExtension.TryGetValue(extension, out extensionFiles);
                        if (extensionFiles == null) {
                            extensionFiles = new List<string>();
                            filesByExtension.Add(extension, extensionFiles);
                        }
                        extensionFiles.Add(fileName);
                    }

                    //tex sorting by alphabetical just 'cause. I don't know if there's supposed to be some order within extension groupings.
                    foreach (KeyValuePair<string, List<string>> entry in filesByExtension) {
                        entry.Value.Sort();
                    }

                    fpkFilesSorted = new List<string>();
                    foreach (var extension in fpkdExtensionsOrder) {
                        List<string> extensionFiles = null;
                        filesByExtension.TryGetValue(extension, out extensionFiles);
                        if (extensionFiles != null) {
                            foreach (var fileName in extensionFiles) {
                                fpkFilesSorted.Add(fileName);
                            }
                        }
                    }
                }
            }


            GzsLib.WriteFpkArchive(FpkFile, FpkFolder, fpkFilesSorted);

            return fpkList;
        }

        public static void BuildArchive(string SourceDir, ModEntry metaData, string outputFilePath)
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
            foreach (string SourceFile in Directory.GetFiles(SourceDir, "*.fpk*", SearchOption.AllDirectories))
            {
                //tex chunk0\Assets\tpp\pack\collectible\common\col_common_tpp_fpk\Assets\tpp\pack\resident\resident00.fpkl is the only fpkl, don't know what a fpkl is, but gzcore crashes on it.
                if (SourceFile.Contains(".fpkl")) {
                    continue;
                }

                string FileName = Tools.ToQarPath(SourceFile.Substring(SourceDir.Length));
                if (!builtFpks.Contains(FileName))
                {
                    // unpack FPK and build FPK list

                    foreach (string file in GzsLib.ListArchiveContents<FpkFile>(SourceFile))
                    {
                        string fpkDir = Tools.ToWinPath(FileName.Replace(".fpk", "_fpk"));
                        string fpkFullDir = Path.Combine(SourceDir, fpkDir);
                        if (!Directory.Exists(fpkFullDir))
                        {
                            GzsLib.ExtractArchive<FpkFile>(SourceFile, fpkFullDir);
                        }
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
                if (!Directory.Exists(Path.Combine("_build", subDir))) Directory.CreateDirectory(Path.Combine("_build", subDir)); // create file structure
                File.Copy(qarFile, Path.Combine("_build", Tools.ToWinPath(qarFilePath)), true);

                ulong hash = Tools.NameToHash(qarFilePath);
                metaData.ModQarEntries.Add(new ModQarEntry() { FilePath = qarFilePath, Compressed = qarFile.Substring(qarFile.LastIndexOf(".") + 1).Contains("fpk") ? true : false, ContentHash = Tools.GetMd5Hash(qarFile), Hash = hash });
            }

            //tex build external entries
            metaData.ModFileEntries = new List<ModFileEntry>();
            foreach (string externalFile in ListExternalFiles(SourceDir)) {
                string subDir = externalFile.Substring(0, externalFile.LastIndexOf("\\")).Substring(SourceDir.Length).TrimStart('\\'); // the subdirectory for XML output
                string externalFilePath = Tools.ToQarPath(externalFile.Substring(SourceDir.Length));
                
                if (!Directory.Exists(Path.Combine("_build", subDir))) Directory.CreateDirectory(Path.Combine("_build", subDir)); // create file structure
                File.Copy(externalFile, Path.Combine("_build", Tools.ToWinPath(externalFilePath)), true);
                string strip = "/"+ExternalDirName;
                if (externalFilePath.StartsWith(strip)) {
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