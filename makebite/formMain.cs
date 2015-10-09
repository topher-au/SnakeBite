using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnakeBite.GzsTool;
using SnakeBite;
using ICSharpCode.SharpZipLib.Zip;

namespace makebite
{
    public partial class formMain : Form
    {
        public formMain()
        {
            InitializeComponent();
        }

        private void buttonSelectPath_Click(object sender, EventArgs e)
        {
            // user selects path and files are added to listbox
            FolderBrowserDialog selectPath = new FolderBrowserDialog();
            DialogResult selectResult = selectPath.ShowDialog();

            if (selectResult != DialogResult.OK) return;

            listModFiles.Items.Clear();

            string modPath = selectPath.SelectedPath;
            textModPath.Text = modPath;
            foreach (string modFile in Directory.GetFiles(modPath, "*.*", SearchOption.AllDirectories)) {
                listModFiles.Items.Add(modFile.Substring(modPath.Length).Replace("\\","/"));
            }
        }

        private void buttonBuild_Click(object sender, EventArgs e)
        {
            string modPath = textModPath.Text;
            // delete existing temp directory
            if (Directory.Exists("_temp")) Directory.Delete("_temp", true);

            // create new temp fs and QAR file XML
            Directory.CreateDirectory("_temp\\makebite");
            QarFile makeQar = new QarFile();
            makeQar.Flags = 3150048;
            makeQar.Name = "makebite.dat";
            makeQar.QarEntries = new List<QarEntry>();

            // do files
            foreach (string modFile in Directory.GetFiles(modPath, "*.*", SearchOption.AllDirectories))
            {
                string subDir = modFile.Substring(0, modFile.LastIndexOf("\\")); // the subdirectory for XML output
                subDir = subDir.Substring(modPath.Length);
                if (!Directory.Exists("_temp\\makebite" + subDir)) Directory.CreateDirectory("_temp\\makebite" + subDir); // create file structure
                File.Copy(modFile, "_temp\\makebite" + modFile.Substring(modPath.Length), true);
                makeQar.QarEntries.Add(new QarEntry() { FilePath = modFile.Substring(modPath.Length+1) }); // add to xml
            }

            // write xml
            makeQar.WriteToFile("_temp\\makebite.dat.xml");

            // create metadata
            ModEntry modMetadata = new ModEntry();
            modMetadata.Name = textModName.Text;
            modMetadata.Version = textModVersion.Text;
            modMetadata.Author = textModAuthor.Text;
            modMetadata.Website = textModWebsite.Text;
            modMetadata.Description = textModDescription.Text;
            modMetadata.ModQarEntries = new List<ModQarEntry>();
            modMetadata.ModFpkEntries = new List<ModFpkEntry>();

            // create file data
            foreach(QarEntry newQarEntry in makeQar.QarEntries)
            {
                modMetadata.ModQarEntries.Add(new ModQarEntry() { FilePath = "/" + newQarEntry.FilePath.Replace("\\","/"), Compressed = newQarEntry.Compressed, Hash = newQarEntry.Hash });
                string fileExt = newQarEntry.FilePath.Substring(newQarEntry.FilePath.LastIndexOf(".")+1).ToLower();
                if (fileExt == "fpk" || fileExt == "fpkd" )
                {
                    newQarEntry.Compressed = true;
                    // decompress fpk files and create metadata
                    string fpkDir = "_temp\\makebite\\" + newQarEntry.FilePath.Replace(".", "_");
                    SnakeBite.GzsTool.GzsTool.Run("_temp\\makebite\\" + newQarEntry.FilePath);
                    foreach(string fpkSubFile in Directory.GetFiles(fpkDir,"*.*",SearchOption.AllDirectories))
                    {
                        modMetadata.ModFpkEntries.Add(new ModFpkEntry() {
                            FilePath = fpkSubFile.Substring(fpkSubFile.LastIndexOf("\\Assets")).Replace("\\","/"),
                            FpkFile = "/" + newQarEntry.FilePath.Replace("\\", "/") }
                        );
                    }
                    Directory.Delete(fpkDir, true);
                    File.Delete("_temp\\makebite\\" + newQarEntry.FilePath + ".xml");
                }
            }

            modMetadata.SaveToFile("_temp\\makebite\\metadata.xml");

            // compress to file
            FastZip zipper = new FastZip();
            zipper.CreateZip(textModName.Text +".mgsv", "_temp\\makebite", true, "(.*?)");

            Directory.Delete("_temp", true);

            MessageBox.Show("Done");
        }
    }
}
