using GzsTool.Core.Fpk;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace makebite
{
    public static class Build
    {
        public static string MGSVVersionStr = "1.0.15.3";// GAMEVERSION

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFpkFolder">path to folder that contains the files that will go into the fpk (with fpk-internal layout)</param>
        /// <param name="destFpkName"></param>
        /// <param name="rootDir">The root the fpk will go into, used by smakebite to derive the qar-internal path</param>
        /// <param name="fpkReferences"></param>
        /// <returns></returns>
        public static List<ModFpkEntry> BuildFpk(string sourceFpkFolder, string destFpkName, string rootDir, List<string> fpkReferences) {
            SnakeBite.Debug.LogLine($"[BuildFpk] {sourceFpkFolder}.");

            List<string> fpkFiles = new List<string>();
            List<ModFpkEntry> fpkList = new List<ModFpkEntry>();
            foreach (string fileName in Directory.GetFiles(sourceFpkFolder, "*.*", SearchOption.AllDirectories)) {
                if (!GzsLib.IsExtensionValidForArchive(fileName, destFpkName)) {
                    SnakeBite.Debug.LogLine($"[BuildFpk] {fileName} is not a valid file for a {Path.GetExtension(destFpkName)} archive.");
                    continue;
                }
                string inQarName = fileName.Substring(sourceFpkFolder.Length).Replace("\\", "/");//tex actually fpk-internal name, but as fpks are subsets of qars then?
                fpkFiles.Add(inQarName);

                fpkList.Add(new ModFpkEntry() {
                    FilePath = inQarName,
                    FpkFile = Tools.ToQarPath(destFpkName.Substring(rootDir.Length)),//qar-internal name of fpk
                    ContentHash = Tools.GetMd5Hash(fileName)
                });
            }//foreach filename
            GzsLib.WriteFpkArchive(destFpkName, sourceFpkFolder, fpkFiles, fpkReferences);

            return fpkList;
        }//BuildFpk

        public static void BuildArchive(string SourceDir, ModEntry metaData, string outputFilePath)
        {
            Debug.LogLine($"[BuildArchive] {SourceDir}.");
            HashingExtended.ReadDictionary();
            string buildDir = Directory.GetCurrentDirectory() + "\\_build";
            try
            {
                if (Directory.Exists(buildDir)) Directory.Delete(buildDir, true);
            }
            catch
            {
                Debug.LogLine(string.Format("[BuildArchive] preexisting _build directory could not be deleted: {0}", buildDir));
            }

            Directory.CreateDirectory("_build");

            //try and update hashed names of fpks
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
            }//for fpkFiles
      
            List<string> fpkFolders = ListFpkFolders(SourceDir);
            //try and update hashed names of fpk folders
            for (int i = fpkFolders.Count - 1; i >= 0; i--)
            {
                string fpkFolder = fpkFolders[i].Substring(SourceDir.Length + 1);
                if (!fpkFolder.StartsWith("Assets"))
                {
                    string updatedFileName = HashingExtended.UpdateName(fpkFolder.Replace("_fpk", ".fpk"));
                    if (updatedFileName != null)
                    {
                        updatedFileName = SourceDir + updatedFileName.Replace('/', '\\');
                        if (fpkFolders.Contains(updatedFileName.Replace(".fpk", "_fpk")) || fpkFiles.Contains(updatedFileName))
                        {

                            MessageBox.Show(string.Format("{0} was not packed or added to the build, because {1} (the unhashed filename of {0}) already exists in the mod directory.", Path.GetFileName(fpkFolders[i]), Path.GetFileName(updatedFileName)));
                            fpkFolders.Remove(fpkFolders[i]);
                        }
                    }
                }
            }//for fpkFolders

            // check for FPKs that must be built and build
            metaData.ModFpkEntries = new List<ModFpkEntry>();
            List<string> builtFpks = new List<string>();
            var fpkReferences = new List<string>();//TODO: allow user to specify references
            foreach (string FpkFullDir in fpkFolders)
            {
                string destFpkName = FpkFullDir.Replace("_fpk", ".fpk");
                foreach (ModFpkEntry fpkEntry in BuildFpk(FpkFullDir, destFpkName, SourceDir, fpkReferences))
                {
                    metaData.ModFpkEntries.Add(fpkEntry);
                    if (!builtFpks.Contains(fpkEntry.FpkFile)) builtFpks.Add(fpkEntry.FpkFile);
                }
            }

            // check for other FPKs and build fpkentry data
            foreach (string SourceFile in Directory.GetFiles(SourceDir, "*.fpk*", SearchOption.AllDirectories))
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
            List<string> qarFiles = ListQarFiles(SourceDir);
            for (int i = qarFiles.Count - 1; i >= 0; i--)
            {
                string qarFile = qarFiles[i].Substring(SourceDir.Length + 1);
                if (!qarFile.StartsWith("Assets"))
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

            metaData.ModQarEntries = new List<ModQarEntry>();
            metaData.ModFileEntries = new List<ModFileEntry>();
            metaData.ModWmvEntries = new List<ModWmvEntry>(); //ZIP: Custom WMV Support
            foreach (string qarFile in qarFiles)
            {
                //ZIP: Custom WMV Support
                if (qarFile.Contains("Assets\\tpp\\movie\\Win"))
                {
                    BuildWMVDat(qarFile, SourceDir, ref metaData);
                    continue;
                }

                string qarFilePath = Tools.ToQarPath(qarFile.Substring(SourceDir.Length));
                string subDir = qarFile.Substring(0, qarFile.LastIndexOf("\\")).Substring(SourceDir.Length).TrimStart('\\'); // the subdirectory for XML output
                if (!Directory.Exists(Path.Combine("_build", subDir))) Directory.CreateDirectory(Path.Combine("_build", subDir)); // create file structure
                File.Copy(qarFile, Path.Combine("_build", Tools.ToWinPath(qarFilePath)), true);

                ulong hash = Tools.NameToHash(qarFilePath);
                metaData.ModQarEntries.Add(new ModQarEntry()
                {
                    FilePath = qarFilePath,
                    Compressed = qarFile.EndsWith(".fpk") || qarFile.EndsWith(".fpkd") ? true : false,
                    ContentHash = Tools.GetMd5Hash(qarFile),
                    Hash = hash
                });
            }

            //tex build external entries
            //metaData.ModFileEntries = new List<ModFileEntry>();
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

            metaData.SBVersion.Version = Application.ProductVersion;

            metaData.SaveToFile("_build\\metadata.xml");

            // build archive
            FastZip zipper = new FastZip();
            zipper.CreateZip(outputFilePath, "_build", true, "(.*?)");

            try
            {
                Directory.Delete("_build", true);
            }
            catch (Exception e)
            {
                Debug.LogLine(string.Format("[BuildArchive] _build directory could not be deleted: {0}", e.ToString()));
            }
        }

        public static void BuildWMVDat(string qarFile, string SourceDir, ref ModEntry metaData)
        {
            //ZIP: If filename doesn't include en/jp, fix that.
            if (!qarFile.Contains("_jp") && !qarFile.Contains("_en") )
            {
                string newName = qarFile.Replace(".dat", "_en.dat");
                File.Move(qarFile, newName);
                qarFile = newName;
            }

            //ZIP: Add decimal hash to WmvEntries.
            ulong datToWMV = 3924887075253387264; //ZIP: .wmv extension doesn't hash properly. Hack.
            string qarFilePath = Tools.ToQarPath(qarFile.Substring(SourceDir.Length));
            ulong wmvHash = Tools.NameToHash(qarFilePath) + datToWMV; //TODO: Get HashFileNameWithExtension to work with strings containing ".wmv"         

            //ZIP: Change export directory for WMV .dat
            string newSourceDir = SourceDir.Replace("Assets\\tpp\\movie\\Win", "GameDir\\master");
            string newQarFile = qarFile.Replace("Assets\\tpp\\movie\\Win", "GameDir\\master");
            qarFilePath = Tools.ToQarPath(newQarFile.Substring(newSourceDir.Length));

            //ZIP: Convert decimal to hex hash and rename dat.
            string hexHash = wmvHash.ToString("X").ToLower() + ".dat";
            qarFilePath = qarFilePath.Substring(0, qarFilePath.LastIndexOf("/") + 1) + hexHash;

            //ZIP: Copy the new WMV .dat
            string subDir = newQarFile.Substring(0, newQarFile.LastIndexOf("\\")).Substring(newSourceDir.Length).TrimStart('\\'); // the subdirectory for XML output
            if (!Directory.Exists(Path.Combine("_build", subDir))) Directory.CreateDirectory(Path.Combine("_build", subDir)); // create file structure
            File.Copy(qarFile, Path.Combine("_build", Tools.ToWinPath(qarFilePath)), true);

            //ZIP: Add WMV .dat to file/wmv entries.
            metaData.ModWmvEntries.Add(new ModWmvEntry() { Hash = wmvHash });
            metaData.ModFileEntries.Add(new ModFileEntry()
            {
                FilePath = "/master/" + hexHash,
                ContentHash = Tools.GetMd5Hash(qarFile)
            });
        }
    }
}