using FolderSelect;
using GzsTool.Core.Utility;
using SnakeBite;
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
            foreach (string modFile in Directory.GetFiles(modPath, "*.*", SearchOption.AllDirectories))
            {
                string filePath = modFile.Substring(modPath.Length).Replace("\\", "/");
                if (Tools.IsValidFile(filePath) && filePath != "/metadata.xml") listModFiles.Items.Add(filePath);
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

            string modPath = saveMod.FileName;

            DoBuild(modPath);

            MessageBox.Show("_build completed.", "MakeBite", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string modPath = Properties.Settings.Default.LastModDir;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2) modPath = args[1];

            comboForVersion.SelectedIndex = comboForVersion.Items.Count - 1;

            if (Directory.Exists(modPath))
            {
                ModEntry modMetaData = new ModEntry();
                if (File.Exists(modPath + "\\metadata.xml"))
                {
                    modMetaData.ReadFromFile(modPath + "\\metadata.xml");

                    textModName.Text = modMetaData.Name;
                    textModVersion.Text = modMetaData.Version;
                    textModAuthor.Text = modMetaData.Author;
                    textModWebsite.Text = modMetaData.Website;
                    textModDescription.Text = modMetaData.Description.Replace("\n", "\r\n");
                }

                if (File.Exists(modPath + "\\readme.txt"))
                {
                    StreamReader s = new StreamReader(modPath + "\\readme.txt");
                    string readme = s.ReadToEnd();
                    textModDescription.Text = readme;
                }

                textModPath.Text = modPath;

                foreach (string modFile in Directory.GetFiles(modPath, "*.*", SearchOption.AllDirectories))
                {
                    string filePath = Tools.ToQarPath(modFile.Substring(modPath.Length));
                    if (Tools.IsValidFile(filePath) && filePath != "/metadata.xml") listModFiles.Items.Add(filePath);
                }

                if (args.Length == 2)
                {
                    DoBuild(Path.Combine(args[1], String.Format("{0}.mgsv", modMetaData.Name)));
                    Application.Exit();
                }
            }
            else
            {
                if (args.Length == 0)
                {
                    Properties.Settings.Default.LastModDir = String.Empty;
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}