using GzsTool.Utility;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using FolderSelect;

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
            FolderSelectDialog fsd = new FolderSelectDialog();

            if (fsd.ShowDialog() != true) return;

            listModFiles.Items.Clear();

            string modPath = fsd.FileName;
            textModPath.Text = modPath;
            foreach (string modFile in Directory.GetFiles(modPath, "*.*", SearchOption.AllDirectories))
            {
                string filePath = modFile.Substring(modPath.Length).Replace("\\", "/");
                if(Hashing.ValidFileExtension(filePath)) listModFiles.Items.Add(filePath);
            }

            Properties.Settings.Default.LastModDir = modPath;
            Properties.Settings.Default.Save();
        }

        private void buttonBuild_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveMod = new SaveFileDialog();
            saveMod.Filter = "MGSV Mod|*.mgsv";
            DialogResult saveResult = saveMod.ShowDialog();
            if (saveResult != DialogResult.OK) return;

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
                if (!Hashing.ValidFileExtension(modFile.Substring(modPath.Length))) continue;
                string subDir = modFile.Substring(0, modFile.LastIndexOf("\\")); // the subdirectory for XML output
                subDir = subDir.Substring(modPath.Length);
                if (!Directory.Exists("_temp\\makebite" + subDir)) Directory.CreateDirectory("_temp\\makebite" + subDir); // create file structure
                File.Copy(modFile, "_temp\\makebite" + modFile.Substring(modPath.Length), true);
                makeQar.QarEntries.Add(new QarEntry() { FilePath = modFile.Substring(modPath.Length + 1) }); // add to xml
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
            foreach (QarEntry newQarEntry in makeQar.QarEntries)
            {
                modMetadata.ModQarEntries.Add(new ModQarEntry() { FilePath = Hashing.DenormalizeFilePath(newQarEntry.FilePath), Compressed = newQarEntry.Compressed, Hash = newQarEntry.Hash });
                string fileExt = newQarEntry.FilePath.Substring(newQarEntry.FilePath.LastIndexOf(".") + 1).ToLower();
                if (fileExt == "fpk" || fileExt == "fpkd")
                {
                    newQarEntry.Compressed = true;
                    // decompress fpk files and create metadata
                    string fpkDir = "_temp\\makebite\\" + newQarEntry.FilePath.Replace(".", "_");
                    SnakeBite.GzsTool.GzsTool.Run("_temp\\makebite\\" + newQarEntry.FilePath);
                    foreach (string fpkSubFile in Directory.GetFiles(fpkDir, "*.*", SearchOption.AllDirectories))
                    {
                        modMetadata.ModFpkEntries.Add(new ModFpkEntry()
                        {
                            FilePath = fpkSubFile.Substring(fpkSubFile.LastIndexOf("\\Assets")).Replace("\\", "/"),
                            FpkFile = "/" + newQarEntry.FilePath.Replace("\\", "/")
                        }
                        );
                    }
                    Directory.Delete(fpkDir, true);
                    File.Delete("_temp\\makebite\\" + newQarEntry.FilePath + ".xml");
                }
            }

            modMetadata.SaveToFile("_temp\\makebite\\metadata.xml");

            // compress to file
            FastZip zipper = new FastZip();
            zipper.CreateZip(saveMod.FileName, "_temp\\makebite", true, "(.*?)");

            Directory.Delete("_temp", true);

            MessageBox.Show("Done");
        }

        private void buttonMetaSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveMeta = new SaveFileDialog();
            SaveMeta.Filter = "Metadata XML|*.xml";
            DialogResult SaveResult = SaveMeta.ShowDialog();
            if (SaveResult != DialogResult.OK) return;

            ModEntry modMetaData = new ModEntry();
            modMetaData.Name = textModName.Text;
            modMetaData.Author = textModAuthor.Text;
            modMetaData.Version = textModVersion.Text;
            modMetaData.MGSVersion = comboForVersion.Text;
            modMetaData.Website = textModWebsite.Text;
            modMetaData.Description = textModDescription.Text;
            modMetaData.SaveToFile(SaveMeta.FileName);
        }

        private void buttonMetaLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog LoadMeta = new OpenFileDialog();
            LoadMeta.Filter = "Metadata XML|*.xml";
            DialogResult LoadResult = LoadMeta.ShowDialog();
            if (LoadResult != DialogResult.OK) return;

            ModEntry modMetaData = new ModEntry();
            modMetaData.ReadFromFile(LoadMeta.FileName);

            textModName.Text = modMetaData.Name;
            textModVersion.Text = modMetaData.Version;
            textModAuthor.Text = modMetaData.Author;
            textModWebsite.Text = modMetaData.Website;
            textModDescription.Text = modMetaData.Description.Replace("\n", "\r\n");
            foreach(string li in comboForVersion.Items)
            {
                if(modMetaData.MGSVersion == li)
                {
                    comboForVersion.SelectedIndex = comboForVersion.Items.IndexOf(li);
                    break;
                }
            }
        }

        private void listModFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Build.BuildFpk(textModPath.Text);
            return;
            string msgboxtext = String.Empty;
            foreach (string folder in Build.ListQarFiles(textModPath.Text)) {
                msgboxtext += folder + "\n";
            }
            MessageBox.Show(msgboxtext);
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            comboForVersion.SelectedIndex = comboForVersion.Items.Count - 1;
            string modPath = Properties.Settings.Default.LastModDir;
            if (Directory.Exists(modPath))
            {
                textModPath.Text = modPath;
                foreach (string modFile in Directory.GetFiles(modPath, "*.*", SearchOption.AllDirectories))
                {
                    string filePath = modFile.Substring(modPath.Length).Replace("\\", "/");
                    if (Hashing.ValidFileExtension(filePath)) listModFiles.Items.Add(filePath);
                }
            } else
            {
                Properties.Settings.Default.LastModDir = String.Empty;
                Properties.Settings.Default.Save();
            }
        }
    }
}