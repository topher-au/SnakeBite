﻿using GzsTool.Core.Utility;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Serialization;
using System.Collections;
using System;
using System.Threading;

namespace SnakeBite
{
    public static class Tools
    {
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

        private static readonly List<string> DatFileExtensions = new List<string>
        {
            "ftexs",
            "nav2",
            "evf",
            "aia",
            "aib",
            "aibc",
            "aig",
            "aigc",
            "aim",
            "aip",
            "ait",
            "atsh",
            "bnd",
            "bnk",
            "clo",
            "csnav",
            "dat",
            "des",
            "dnav",
            "dnav2",
            "lng",
            "ese",
            "evb",
            "evf",
            "fag",
            "fage",
            "fago",
            "fagp",
            "fagx",
            "fclo",
            "fcnp",
            "fcnpx",
            "fdes",
            "fdmg",
            "ffnt",
            "fmdl",
            "fmdlb",
            "fmtt",
            "fnt",
            "fova",
            "fox",
            "fox2",
            "fpk",
            "fpkd",
            "fpkl",
            "frdv",
            "frig",
            "frt",
            "fsd",
            "fsm",
            "fsml",
            "fsop",
            "fstb",
            "ftex",
            "fv2",
            "fxp",
            "gani",
            "geom",
            "gpfp",
            "grxla",
            "grxoc",
            "gskl",
            "htre",
            "info",
            "json",
            "lad",
            "ladb",
            "lani",
            "las",
            "lba",
            "lng",
            "lpsh",
            "lua",
            "mas",
            "mbl",
            "mog",
            "mtar",
            "mtl",
            "nav2",
            "nta",
            "obr",
            "obrb",
            "parts",
            "path",
            "pftxs",
            "ph",
            "phep",
            "phsd",
            "qar",
            "rbs",
            "rdb",
            "rdf",
            "rnav",
            "sad",
            "sand",
            "sani",
            "sbp",
            "sdf",
            "sim",
            "simep",
            "snav",
            "spch",
            "sub",
            "subp",
            "tgt",
            "tre2",
            "txt",
            "uia",
            "uif",
            "uig",
            "uigb",
            "uil",
            "uilb",
            "utxl",
            "veh",
            "vfx",
            "vfxbin",
            "vfxdb",
            "vnav",
            "vo.evf",
            "vpc",
            "wem",
            //"xml"
        };

        public static ModEntry ReadMetaData(string ModFile)
        {
            if (!File.Exists(ModFile)) return null;

            try
            {
                using (FileStream streamMod = new FileStream(ModFile, FileMode.Open))
                using (ZipFile zipMod = new ZipFile(streamMod))
                {
                    var metaIndex = zipMod.FindEntry("metadata.xml", true);
                    if (metaIndex == -1) return null;
                    using (StreamReader metaReader = new StreamReader(zipMod.GetInputStream(metaIndex)))
                    {
                        XmlSerializer x = new XmlSerializer(typeof(ModEntry));
                        var metaData = (ModEntry)x.Deserialize(metaReader);
                        return metaData;
                    }
                }
            }
            catch { return null; }

        }
        
        public static string ToWinPath(string Path)
        {
            return Path.Replace("/", "\\").TrimStart('\\');
        }

        public static string ToQarPath(string Path)
        {
            return "/" + Path.Replace("\\", "/").TrimStart('/');
        }

        internal static string GetMd5Hash(string FileName)
        {
            byte[] hashBytes;
            using (var hashMD5 = MD5.Create())
            {
                using (var stream = File.OpenRead(FileName))
                {
                    hashBytes = hashMD5.ComputeHash(stream);
                }
            }

            StringBuilder hashBuilder = new StringBuilder(hashBytes.Length * 2);

            for (int i = 0; i < hashBytes.Length; i++)
                hashBuilder.Append(hashBytes[i].ToString("X2"));

            return hashBuilder.ToString();
        }

        internal static ulong NameToHash(string FileName)
        {
            string filePath = Tools.ToQarPath(FileName);
            ulong hash = Hashing.HashFileNameWithExtension(filePath);
            // find hashed names, which will be in root
            if (!filePath.Substring(1).Contains("/")) {
                // try to parse hash from filename
                string fileName = filePath.TrimStart('/');
                string fileNoExt = fileName.Substring(0, fileName.IndexOf("."));
                string fileExt = fileName.Substring(fileName.IndexOf(".") + 1);
                //tex NMC aparently cant use HashFileNameWithExtension with undictionaried/files with hash names
                // tryParseHash will fail for non hashed files in root (currently only init.lua and foxpatch.dat)
                bool tryParseHash = ulong.TryParse(fileNoExt, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out hash);
                if (tryParseHash) // successfully parsed filename
                {
                    //TODO: create Hashing.HashFileExtension
                    ulong ExtHash = Hashing.HashFileName(fileExt, false) & 0x1FFF;
                    hash = (ExtHash << 51) | hash;
                } else {//tex attempted fix for above
                    hash = Hashing.HashFileNameWithExtension(filePath);
                }
            }
            return hash;
        }

        internal static bool CompareNames(string File1, string File2)
        {
            // TODO: change name comparison to use function
            return Tools.ToQarPath(File1) == Tools.ToQarPath(File2);
        }

        internal static bool CompareHashes(string File1, string File2)
        {
            return Tools.NameToHash(File1) == Tools.NameToHash(File2);
        }

        internal static bool IsValidFile(string FilePath)
        {
            string ext = FilePath.Substring(FilePath.IndexOf(".") + 1);
            if (DatFileExtensions.Contains(ext)) return true;
            return false;
        }

        public static void DeleteDirectory(string target_dir)
        {
            //Debug.LogLine("[Cleanup Debug] Removing " + target_dir);
            foreach (string file in Directory.EnumerateFiles(target_dir))
            {
                //Debug.LogLine("[Cleanup Debug] Setting FileAttributes for " + file);
                File.SetAttributes(file, FileAttributes.Normal);
                //Debug.LogLine("[Cleanup Debug] Deleting " + file);
                File.Delete(file);
            }
            foreach (string dir in Directory.EnumerateDirectories(target_dir))
            {
                //Debug.LogLine("[Cleanup Debug] Deleting " + dir);
                DeleteDirectory(dir);
            }

            //Debug.LogLine("[Cleanup Debug] Deleting " + target_dir);
            DirectoryInfo target = new DirectoryInfo(target_dir);
            if (target.GetFiles().Length == 0)
                Directory.Delete(target_dir, true);
            else
            {
                Thread.Sleep(50);
                DeleteDirectory(target_dir);
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (dir.Exists)
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, true);
                }

                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
        }

        public static string GetFileSizeKB(params string[] filePaths)
        {
            long size = 0;
            foreach(string filePath in filePaths)
            {
                size += new FileInfo(filePath).Length;
            }

            return string.Format("{0:n0}", size / 1024);
        }
    }
}