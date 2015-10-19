using System.Text;
using GzsTool.Core.Utility;
using System.Security.Cryptography;
using System.IO;

namespace SnakeBite
{
        public static class Tools
        {
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

            internal static ulong ConvertFileNameToHash(string FileName)
            {
                // regenerate hash for file
                string filePath = Tools.ToQarPath(FileName);
                ulong hash = Hashing.HashFileNameWithExtension(filePath);
                if (!filePath.Substring(1).Contains("/"))
                {
                    // try to parse hash from filename
                    string fileName = filePath.TrimStart('/');
                    string fileNoExt = fileName.Substring(0, fileName.IndexOf("."));
                    string fileExt = fileName.Substring(fileName.IndexOf(".") + 1);

                    bool tryParseHash = ulong.TryParse(fileNoExt, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out hash);
                    if (tryParseHash) // successfully parsed filename
                    {
                        ulong ExtHash = Hashing.HashFileName(fileExt, false) & 0x1FFF;
                        hash = (ExtHash << 51) | hash;
                    }
                }
                return hash;
            }
        }
    }