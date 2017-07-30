using GzsTool.Core.Common;
using GzsTool.Core.Common.Interfaces;
using GzsTool.Core.Fpk;
using GzsTool.Core.Qar;
using GzsTool.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SnakeBite.GzsTool
{
    public static class GzsLib
    {
        // Extract full archive
        public static List<string> ExtractArchive<T>(string FileName, string OutputPath) where T : ArchiveFile, new()
        {
            Debug.LogLine(String.Format("[GzsLib] Extracting archive {0} to {1}", FileName, OutputPath));

            if (!File.Exists(FileName))
            {
                Debug.LogLine("[GzsLib] File not found");
                throw new FileNotFoundException();
            }

            LoadDictionaries();
            using (FileStream archiveFile = new FileStream(FileName, FileMode.Open))
            {
                List<string> outFiles = new List<string>();
                T archive = new T();
                archive.Name = Path.GetFileName(FileName);
                archive.Read(archiveFile);

                // Extract all files
                var exportedFiles = archive.ExportFiles(archiveFile);
                foreach (var v in exportedFiles)
                {
                    string outDirectory = Path.Combine(OutputPath, Path.GetDirectoryName(v.FileName));
                    string outFileName = Path.Combine(OutputPath, v.FileName);
                    if (!Directory.Exists(outDirectory)) Directory.CreateDirectory(outDirectory);
                    using (FileStream outStream = new FileStream(outFileName, FileMode.Create))
                    {
                        // copy to output stream
                        v.DataStream().CopyTo(outStream);
                        outFiles.Add(v.FileName);
                    }
                }
                Debug.LogLine(String.Format("[GzsLib] Extracted {0} files", outFiles.Count));
                return outFiles;
            }
        }

        // Extract single file from archive
        public static bool ExtractFile<T>(string SourceArchive, string FilePath, string OutputFile) where T : ArchiveFile, new()
        {
            LoadDictionaries();
            Debug.LogLine(String.Format("[GzsLib] Extracting file {1}: {0} -> {2}", FilePath, SourceArchive, OutputFile));
            if (!File.Exists(SourceArchive))
            {
                Debug.LogLine("[GzsLib] File not found");
                throw new FileNotFoundException();
            }
            // Get file hash from path
            ulong fileHash = Tools.NameToHash(FilePath);

            using (FileStream archiveFile = new FileStream(SourceArchive, FileMode.Open))
            {
                T archive = new T();
                archive.Name = Path.GetFileName(SourceArchive);
                archive.Read(archiveFile);

                // Select single file for output
                var outFile = archive.ExportFiles(archiveFile).FirstOrDefault(entry => Tools.NameToHash(entry.FileName) == fileHash);

                if (outFile != null)
                {
                    string path = Path.GetDirectoryName(Path.GetFullPath(OutputFile));
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    using (FileStream outStream = new FileStream(OutputFile, FileMode.Create))
                    {
                        // copy to output stream
                        outFile.DataStream().CopyTo(outStream);
                    }
                    return true;
                }
                else
                {
                    // file not found
                    return false;
                }
            }
        }

        // Extract single file from archive
        public static bool ExtractFileByHash<T>(string SourceArchive, ulong FileHash, string OutputFile) where T : ArchiveFile, new()
        {
            Debug.LogLine(String.Format("[GzsLib] Extracting file from {1}: hash {0} -> {2}", FileHash, SourceArchive, OutputFile));
            if (!File.Exists(SourceArchive))
            {
                Debug.LogLine("[GzsLib] File not found");
                throw new FileNotFoundException();
            }
            LoadDictionaries();
            // Get file hash from path
            ulong fileHash = FileHash;

            using (FileStream archiveFile = new FileStream(SourceArchive, FileMode.Open))
            {
                T archive = new T();
                archive.Name = Path.GetFileName(SourceArchive);
                archive.Read(archiveFile);

                // Select single file for output
                var outFile = archive.ExportFiles(archiveFile).FirstOrDefault(entry => Tools.NameToHash(entry.FileName) == fileHash);

                if (outFile != null)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(OutputFile))) Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));
                    using (FileStream outStream = new FileStream(OutputFile, FileMode.Create))
                    {
                        // copy to output stream
                        outFile.DataStream().CopyTo(outStream);
                    }
                    return true;
                }
                else
                {
                    // file not found
                    return false;
                }
            }
        }

        // Read file hashes contained within QAR archive
        public static List<GameFile> ListArchiveHashes<T>(string ArchiveName) where T : ArchiveFile, new()
        {
            Debug.LogLine(String.Format("[GzsLib] Reading archive contents: {0}", ArchiveName));
            if (!File.Exists(ArchiveName))
            {
                Debug.LogLine("[GzsLib] File not found");
                throw new FileNotFoundException();
            }
            LoadDictionaries();
            using (FileStream archiveFile = new FileStream(ArchiveName, FileMode.Open))
            {
                List<GameFile> archiveContents = new List<GameFile>();
                T archive = new T();
                archive.Name = Path.GetFileName(ArchiveName);
                archive.Read(archiveFile);
                foreach (var x in archive.ExportFiles(archiveFile))
                {
                    archiveContents.Add(new GameFile() { FilePath = x.FileName, FileHash = Tools.NameToHash(x.FileName), QarFile = archive.Name });
                }
                return archiveContents;
            }
        }

        // Read file hashes contained within QAR archive
        public static List<string> ListArchiveContents<T>(string ArchiveName) where T : ArchiveFile, new()
        {
            Debug.LogLine(String.Format("[GzsLib] Reading archive contents: {0}", ArchiveName));
            if (!File.Exists(ArchiveName))
            {
                Debug.LogLine("[GzsLib] File not found");
                throw new FileNotFoundException();
            }
            LoadDictionaries();
            using (FileStream archiveFile = new FileStream(ArchiveName, FileMode.Open))
            {
                List<string> archiveContents = new List<string>();
                T archive = new T();
                archive.Name = Path.GetFileName(ArchiveName);
                archive.Read(archiveFile);
                foreach (var x in archive.ExportFiles(archiveFile))
                {
                    archiveContents.Add(x.FileName);
                }
                return archiveContents;
            }
        }

        // Load filename dictionaries
        public static void LoadDictionaries()
        {
            SettingsManager manager = new SettingsManager(ModManager.GameDir);
            var FpkNames = manager.GetModFpkFiles();
            var QarNames = manager.GetModQarFiles(true);

            File.WriteAllLines("mod_fpk_dict.txt", FpkNames);
            File.WriteAllLines("mod_qar_dict.txt", QarNames);

            Hashing.ReadDictionary("qar_dictionary.txt");
            Hashing.ReadDictionary("mod_qar_dict.txt");

            Hashing.ReadMd5Dictionary("fpk_dictionary.txt");
            Hashing.ReadMd5Dictionary("mod_fpk_dict.txt");
        }

        // Read contents of base game files into list
        public static GameFiles ReadBaseData()
        {
            Debug.LogLine(String.Format("[GzsLib] Acquiring base game data"));
            List<GameFile> ReadBaseData = new List<GameFile>();
            string dataDir = Path.Combine(ModManager.GameDir, "master");
            string dataDat = "data{0}.dat";
            string chunkDat = "chunk{0}.dat";
            string a_c7Dat = "a_chunk7.dat";
            string oneDat = "0\\{0:X2}.dat";

            Dictionary<ulong, string> BaseData = new Dictionary<ulong, string>();
            LoadDictionaries();

            // read a_chunk7
            var a_c7Files = ListArchiveHashes<QarFile>(Path.Combine(dataDir, a_c7Dat));
            foreach (GameFile file in a_c7Files)
            {
                try
                {
                    ReadBaseData.Add(file);
                }
                catch { }
            }

            // read data1
            var dataFiles = ListArchiveHashes<QarFile>(Path.Combine(dataDir, String.Format(dataDat, 1)));
            foreach (GameFile file in dataFiles)
            {
                try
                {
                    ReadBaseData.Add(file);
                }
                catch { }
            }

            // read chunks
            for (int i = 0; i <= 4; i++)
            {
                var chunkFiles = ListArchiveHashes<QarFile>(Path.Combine(dataDir, String.Format(chunkDat, i)));
                foreach (GameFile file in chunkFiles)
                {
                    try
                    {
                        ReadBaseData.Add(file);
                    }
                    catch { }
                }
            }

            // read 01.dat
            var oneFiles = ListArchiveHashes<QarFile>(Path.Combine(dataDir, String.Format(oneDat, 1)));
            foreach (GameFile file in oneFiles)
            {
                try
                {
                    ReadBaseData.Add(file);
                }
                catch { }
            }

            return new GameFiles { FileList = ReadBaseData };
        }

        // Export FPK archive with specified parameters
        public static void WriteFpkArchive(string FileName, string SourceDirectory, List<string> Files)
        {
            Debug.LogLine(String.Format("[GzsLib] Writing FPK archive: {0}", FileName));
            FpkFile q = new FpkFile() { Name = FileName };
            foreach (string s in Files)
            {
                q.Entries.Add(new FpkEntry() { FilePath = Tools.ToQarPath(s) });
            }

            using (FileStream outFile = new FileStream(FileName, FileMode.Create))
            {
                IDirectory fileDirectory = new FileSystemDirectory(SourceDirectory);
                q.Write(outFile, fileDirectory);
            }
        }

        // Export QAR archive with specified parameters
        public static void WriteQarArchive(string FileName, string SourceDirectory, List<string> Files, uint Flags)
        {
            Debug.LogLine(String.Format("[GzsLib] Writing QAR archive: {0}", FileName));
            List<QarEntry> qarEntries = new List<QarEntry>();
            foreach (string s in Files)
            {
                qarEntries.Add(new QarEntry() { FilePath = s, Hash = Tools.NameToHash(s), Compressed = Path.GetExtension(s).Contains(".fpk") ? true : false });
            }

            QarFile q = new QarFile() { Entries = qarEntries, Flags = Flags, Name = FileName };

            using (FileStream outFile = new FileStream(FileName, FileMode.Create))
            {
                IDirectory fileDirectory = new FileSystemDirectory(SourceDirectory);
                q.Write(outFile, fileDirectory);
            }
        }
    }
}