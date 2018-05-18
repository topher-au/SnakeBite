using GzsTool.Core.Utility;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Serialization;
using System.Collections;

namespace SnakeBite
{
    public static class Tools
    {
        private static readonly List<string> DatFileExtensions = new List<string>
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
            "bnd",
            "bnk",
            "cc.evf",
            "clo",
            "csnav",
            "dat",
            "des",
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
            "fxp",
            "gani",
            "geom",
            "ger.lng",
            "gpfp",
            "grxla",
            "grxoc",
            "gskl",
            "htre",
            "info",
            "ita.lng",
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
            // regenerate hash for file
            string filePath = Tools.ToQarPath(FileName);
            ulong hash = Hashing.HashFileNameWithExtension(filePath);
            if (!filePath.Substring(1).Contains("/")) {
                // try to parse hash from filename
                string fileName = filePath.TrimStart('/');
                string fileNoExt = fileName.Substring(0, fileName.IndexOf("."));
                string fileExt = fileName.Substring(fileName.IndexOf(".") + 1);

                //tex NMC aparently cant use HashFileNameWithExtension with undictionaried/files with hash names
                // however BUG? because hexnumber is only 0-9, a-f thus tryParseHash will fail and return 0 for init.lua and foxpatch.dat
                // see also duplicate function in makebite/classes/tools NameToHash
                bool tryParseHash = ulong.TryParse(fileNoExt, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out hash);
                if (tryParseHash) // successfully parsed filename
                {
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
            string ext = FilePath.Substring(FilePath.IndexOf("."));
            if (DatFileExtensions.Contains(ext)) return true;
            return false;
        }
    }
}