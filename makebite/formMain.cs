using FolderSelect;
using GzsTool.Core.Fpk;
using SnakeBite;
using SnakeBite.GzsTool;
using System;
using System.IO;
using System.Windows.Forms;

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
            PopulateBoxes(modPath);
            Properties.Settings.Default.LastModDir = modPath;
            Properties.Settings.Default.Save();
        }

        private void PopulateBoxes(string DataPath)
        {
            // unpack existing fpks
            foreach (string fpkFile in Directory.GetFiles(DataPath, "*.fpk*", SearchOption.AllDirectories))
            {
                string fpkDir = Path.Combine(Path.GetDirectoryName(fpkFile), Path.GetFileName(fpkFile).Replace(".", "_"));
                if (!Directory.Exists(fpkDir))
                {
                    //extract fpk
                    GzsLib.ExtractArchive<FpkFile>(fpkFile, fpkDir);
                }
            }

            foreach (string modFile in Directory.GetFiles(DataPath, "*.*", SearchOption.AllDirectories))
            {
                string filePath = modFile.Substring(DataPath.Length).Replace("\\", "/");
                if (Tools.IsValidFile(filePath) && filePath != "/metadata.xml") listModFiles.Items.Add(filePath);
            }

            if (File.Exists(Path.Combine(DataPath, "metadata.xml")))
            {
                ModEntry modMetaData = new ModEntry();
                modMetaData.ReadFromFile(Path.Combine(DataPath, "metadata.xml"));

                textModName.Text = modMetaData.Name;
                textModVersion.Text = modMetaData.Version;
                textModAuthor.Text = modMetaData.Author;
                textModWebsite.Text = modMetaData.Website;
                textModDescription.Text = modMetaData.Description.Replace("\n", "\r\n");
                foreach (string li in comboForVersion.Items)
                {
                    if (modMetaData.MGSVersion == li)
                    {
                        comboForVersion.SelectedIndex = comboForVersion.Items.IndexOf(li);
                        break;
                    }
                }
            }

            if (File.Exists(DataPath + "\\readme.txt"))
            {
                StreamReader s = new StreamReader(DataPath + "\\readme.txt");
                string readme = s.ReadToEnd();
                textModDescription.Text = readme;
            }
        }

        private void buttonBuild_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveMod = new SaveFileDialog();
            saveMod.Filter = "MGSV Mod|*.mgsv";
            DialogResult saveResult = saveMod.ShowDialog();
            if (saveResult != DialogResult.OK) return;

            string modPath = saveMod.FileName;

            DoBuild(modPath);

            MessageBox.Show("Build completed.", "MakeBite", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DoBuild(string BuildFile)
        {
            ModEntry modMetaData = new ModEntry();
            modMetaData.Name = textModName.Text;
            modMetaData.Author = textModAuthor.Text;
            modMetaData.Version = textModVersion.Text;
            modMetaData.MGSVersion = comboForVersion.Text;
            modMetaData.Website = textModWebsite.Text;
            modMetaData.Description = textModDescription.Text;
            modMetaData.SaveToFile(textModPath.Text + "\\metadata.xml");

            Build.BuildArchive(textModPath.Text, modMetaData, BuildFile);
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
            foreach (string li in comboForVersion.Items)
            {
                if (modMetaData.MGSVersion == li)
                {
                    comboForVersion.SelectedIndex = comboForVersion.Items.IndexOf(li);
                    break;
                }
            }
        }

        private void listModFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            string modPath = Properties.Settings.Default.LastModDir;
            if (args.Length == 2) modPath = Path.GetFullPath(args[1]);

            textModPath.Text = modPath;
            comboForVersion.SelectedIndex = comboForVersion.Items.Count - 1;

            if (Directory.Exists(modPath)) // if loaded folder exists
            {
                PopulateBoxes(modPath);
                if (args.Length == 2)      // if command line was specified
                {
                    DoBuild(Path.Combine(modPath, "mod.mgsv"));
                    Application.Exit();    // build and exit
                }
            }
            else
            {
                if (args.Length == 1)      // if folder doesnt exist reset path
                {
                    Properties.Settings.Default.LastModDir = String.Empty;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void comboForVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}