using GzsTool.Core.Utility;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SnakeBite
{
    public static class Tools
    {
        private static readonly List<string> FileExtensions = new List<string>
        {
            "1.ftexs",
            "1.nav2",
            "2.ftexs",
            "3.ftexs",
            "4.ftexs",
            "5.ftexs",
            "6.ftexs",
            "ag.evf",
            "aia",
            "aib",
            "aibc",
            "aig",
            "aigc",
            "aim",
            "aip",
            "ait",
            "atsh",
            "bmp",
            "bnd",
            "bnk",
            "cc.evf",
            "cfg",
            "clo",
            "csnav",
            "dat",
            "db",
            "dds",
            "des",
            "dll",
            "dnav",
            "dnav2",
            "eng.lng",
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
            "fre.lng",
            "frig",
            "frt",
            "fsd",
            "fsm",
            "fsml",
            "fsop",
            "fstb",
            "ftex",
            "fv2",
            "fx.evf",
            "fx",
            "fxp",
            "gani",
            "geom",
            "ger.lng",
            "gpfp",
            "grxla",
            "grxoc",
            "gskl",
            "h",
            "hlsl",
            "htre",
            "info",
            "ini",
            "ita.lng",
            "jpg",
            "jpn.lng",
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
            "png",
            "por.lng",
            "qar",
            "rbs",
            "rdb",
            "rdf",
            "rnav",
            "rus.lng",
            "sad",
            "sand",
            "sani",
            "sbp",
            "sd.evf",
            "sdf",
            "sim",
            "simep",
            "snav",
            "spa.lng",
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
            "undef",
            "utxl",
            "veh",
            "vfx",
            "vfxbin",
            "vfxdb",
            "vnav",
            "vo.evf",
            "vpc",
            "wem",
            "xml"
        };

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
            // regenerate hash for file
            string filePath = Tools.ToQarPath(FileName);
            ulong hash = Hashing.HashFileNameWithExtension(filePath);
            if (!filePath.Substring(1).Contains("/")) {
                // try to parse hash from filename
                string fileName = filePath.TrimStart('/');
                string fileNoExt = fileName.Substring(0, fileName.IndexOf("."));
                string fileExt = fileName.Substring(fileName.IndexOf(".") + 1);

                bool tryParseHash = ulong.TryParse(fileNoExt, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out hash);
                if (tryParseHash) // successfully parsed filename
                {
                    ulong ExtHash = Hashing.HashFileName(fileExt, false) & 0x1FFF;
                    hash = (ExtHash << 51) | hash;
                } else {//tex attempted fix, see snakebite/tools NameToHash for more info
                    hash = Hashing.HashFileNameWithExtension(filePath);
                }
            }
            return hash;
        }

        internal static bool IsValidFile(string FilePath)
        {
            string ext = FilePath.Substring(FilePath.IndexOf(".") + 1);
            if (FileExtensions.Contains(ext)) return true;
            return false;
        }
    }
}