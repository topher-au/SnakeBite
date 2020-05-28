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

        private void PopulateBoxes(string modPath)
        {
            // unpack existing fpks if extracted folders don't exist
            foreach (string fpkFile in Directory.GetFiles(modPath, "*.fpk*", SearchOption.AllDirectories))
            {
                //tex chunk0\Assets\tpp\pack\collectible\common\col_common_tpp_fpk\Assets\tpp\pack\resident\resident00.fpkl is the only fpkl, don't know what a fpkl is, but gzcore crashes on it. also checks for xml in case user opened the fpk with gzstool and produced a xml file
                if (fpkFile.EndsWith(".fpkl") || fpkFile.EndsWith(".xml")) {
                    continue;
                }

                string fpkDir = Path.Combine(Path.GetDirectoryName(fpkFile), Path.GetFileName(fpkFile).Replace(".", "_"));
                if (!Directory.Exists(fpkDir))
                {
                    //extract fpk
                    GzsLib.ExtractArchive<FpkFile>(fpkFile, fpkDir);
                }
            }

            foreach (string modFile in Directory.GetFiles(modPath, "*.*", SearchOption.AllDirectories))
            {
                string filePath = modFile.Substring(modPath.Length).Replace("\\", "/");
                //GOTCHA: IsValidFile is only roughly accurate for this purpose, but listModFiles is only currently being used as non interactive user feedback so no big issue.
                if ((Tools.IsValidFile(filePath) || filePath.Contains("/GameDir")) && filePath != "/metadata.xml")
                {
                    listModFiles.Items.Add(filePath);
                }
            }

            if (File.Exists(Path.Combine(modPath, "metadata.xml")))
            {
                ModEntry modMetaData = new ModEntry();
                modMetaData.ReadFromFile(Path.Combine(modPath, "metadata.xml"));

                textModName.Text = modMetaData.Name;
                textModVersion.Text = modMetaData.Version;
                textModAuthor.Text = modMetaData.Author;
                textModWebsite.Text = modMetaData.Website;
                textModDescription.Text = modMetaData.Description.Replace("\n", "\r\n");
                string mgsvVersion = modMetaData.MGSVersion.AsString(); 
                bool foundVersion = false;
                foreach (string li in comboForVersion.Items)
                {
                    if (mgsvVersion == li)
                    {
                        comboForVersion.SelectedIndex = comboForVersion.Items.IndexOf(li);
                        foundVersion = true;
                        break;
                    }
                }
                if (!foundVersion) {
                    comboForVersion.Items.Add(mgsvVersion);
                    comboForVersion.Text = mgsvVersion;
                }
            }

            if (File.Exists(modPath + "\\readme.txt"))
            {
                StreamReader s = new StreamReader(modPath + "\\readme.txt");
                string readme = s.ReadToEnd();
                textModDescription.Text = readme;
            }
        }

        private void buttonBuild_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveMod = new SaveFileDialog();
            saveMod.Filter = "MGSV Mod File|*.mgsv";
            DialogResult saveResult = saveMod.ShowDialog();
            if (saveResult != DialogResult.OK) return;

            string modPath = saveMod.FileName;

            DoBuild(modPath);

            MessageBox.Show("Build completed.", "MakeBite", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DoBuild(string outputFilePath)
        {
            ModEntry modMetaData = new ModEntry();
            modMetaData.Name = textModName.Text;
            modMetaData.Author = textModAuthor.Text;
            modMetaData.Version = textModVersion.Text;
            modMetaData.MGSVersion.Version = comboForVersion.Text;
            modMetaData.Website = textModWebsite.Text;
            modMetaData.Description = textModDescription.Text;
            modMetaData.SaveToFile(textModPath.Text + "\\metadata.xml");

            Build.BuildArchive(textModPath.Text, modMetaData, outputFilePath);
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
            modMetaData.MGSVersion.Version = comboForVersion.Text;
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
                if (modMetaData.MGSVersion.AsString() == li)
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

        private void labelToggleHelp_Click(object sender, EventArgs e)
        {
            formHelp help = new formHelp();
            help.Show();
        }
    }
}