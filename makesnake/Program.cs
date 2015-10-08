using ICSharpCode.SharpZipLib.Zip;
using SnakeBite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace makesnake
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("makesnake 0.1 mod file generator");
            Console.WriteLine("by Topher-AU");
            Console.WriteLine("------------");
            Console.WriteLine();
            Console.WriteLine("Install your mod into 01.dat by itself then run makesnake <01.dat> to\ngenerate a SnakeBite compatible mod file.");

            if (args.Count() == 0)
            {
                Console.WriteLine("Please specify a DAT/QAR archive to build the mod file from");
                return;
            }

            string datFile = args[0];
            if (!File.Exists(datFile))
            {
                Console.WriteLine(String.Format("Unable to archive: {0}", datFile));
                return;
            }

            // copy to temp folder
            if (File.Exists("_temp.dat"))
            {
                File.Delete("_temp.dat");
            }
            File.Copy(datFile, "_temp.dat");

            // initialize GzsTool process
            Process gzsProcess = new Process();
            gzsProcess.StartInfo.FileName = "GzsTool.exe";
            gzsProcess.StartInfo.UseShellExecute = false;
            gzsProcess.StartInfo.CreateNoWindow = true;

            // unpack archive
            Console.WriteLine(String.Format("Unpacking {0}...", datFile));
            gzsProcess.StartInfo.Arguments = "_temp.dat";
            gzsProcess.Start();
            while (!gzsProcess.HasExited) { }

            // get list of all extracted files
            List<string> tempFiles = Directory.GetFiles("_temp", "*.*", SearchOption.AllDirectories).ToList();

            // build QAR list from XML
            GzsQarXml qarXml = new GzsQarXml();
            qarXml.LoadFromFile("_temp.dat.xml");

            List<ModQarEntry> modQarEntries = new List<ModQarEntry>();

            // check for fpk/fpkd files
            List<string> fpkFiles = new List<string>();
            foreach (GzsQarEntry qarEntry in qarXml.GzsQarEntries)
            {
                string fileExt = qarEntry.FilePath.Substring(qarEntry.FilePath.LastIndexOf(".") + 1).ToLower();
                if (fileExt == "fpk" || fileExt == "fpkd")
                {
                    fpkFiles.Add(qarEntry.FilePath);
                }

                modQarEntries.Add(new ModQarEntry() { FilePath = qarEntry.FilePath, Compressed = qarEntry.Compressed, Hash = qarEntry.Hash });
            }

            List<ModFpkEntry> fpkEntries = new List<ModFpkEntry>();
            if (fpkFiles.Count > 0)
            {
                Console.WriteLine("Unpacking fox archives...");

                foreach (string fpkFile in fpkFiles)
                {
                    string fpkPath = "_temp\\" + fpkFile;
                    string fpkName = "/" + fpkFile.Replace("\\", "/");
                    string archiveDir = fpkPath.Replace(".", "_");
                    gzsProcess.StartInfo.Arguments = fpkPath;
                    gzsProcess.Start();
                    while (!gzsProcess.HasExited) { }

                    // build fpk list
                    List<string> fpkDirList = Directory.GetFiles(archiveDir, "*.*", SearchOption.AllDirectories).ToList();
                    foreach (string fpkFilePath in fpkDirList)
                    {
                        ModFpkEntry fpkEntry = new ModFpkEntry() { FpkFile = fpkName, FilePath = fpkFilePath.Substring(fpkPath.Length).Replace("\\", "/") };
                        fpkEntries.Add(fpkEntry);
                    }
                }
            }

            // build metadata
            Console.Write("Mod Name: ");
            string modName = Console.ReadLine();
            Console.Write("Mod Version: ");
            string modVersion = Console.ReadLine();
            Console.Write("Mod Author: ");
            string modAuthor = Console.ReadLine();
            Console.Write("Mod Website: ");
            string modWebsite = Console.ReadLine();

            Console.WriteLine("Generating metadata.xml...");
            ModEntry modData = new ModEntry();
            modData.Name = modName;
            modData.Version = modVersion;
            modData.Author = modAuthor;
            modData.Website = modWebsite;
            modData.ModQarEntries = modQarEntries;
            modData.ModFpkEntries = fpkEntries;

            StreamWriter datXmlWriter = new StreamWriter("_temp\\metadata.xml");
            XmlSerializer xmlSer = new XmlSerializer(typeof(ModEntry));
            xmlSer.Serialize(datXmlWriter, modData);
            datXmlWriter.Close();

            Console.WriteLine("Creating modfile...");
            FastZip zipper = new FastZip();
            zipper.CreateZip(datFile + ".mgsv", "_temp", true, "(.*?)");

            Console.WriteLine("Done!");
        }
    }
}