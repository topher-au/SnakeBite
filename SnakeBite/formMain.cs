using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using SnakeBite.GzsTool;
using GzsTool.Utility;

namespace SnakeBite
{
    public partial class formMain : Form
    {
        private Settings objSettings = new Settings();
        private formProgress progWindow = new formProgress();

        public formMain()
        {
            InitializeComponent();
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            // check if user has specified valid install path
            if (!ModManager.ValidInstallPath)
            {
                MessageBox.Show("Please locate your MGSV installation to continue.", "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Show();
                tabControl.SelectedIndex = 1;
                buttonFindMGSV_Click(null, null);
                if (!ModManager.ValidInstallPath)
                    Application.Exit();
            }

            // Refresh mod list
            LoadInstalledMods(true);

            textInstallPath.Text = Properties.Settings.Default.InstallPath;

            this.Show();

            // hash 01.dat and check against last known hash
            // if hash doesn't match, prompt user to regen game file database
            string datHash = ModManager.HashFile(ModManager.GameArchivePath);
            if(datHash != Properties.Settings.Default.DatHash)
            {
                MessageBox.Show("Game data modified outside of SnakeBite. Game cache must be rebuilt and previously installed mods will no longer appear.",
                                "Game data hash mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonBuildGameDB_Click(null, null);
                Properties.Settings.Default.DatHash = ModManager.HashFile(ModManager.GameArchivePath);
                Properties.Settings.Default.Save();
            }

            



        }

        private void LoadInstalledMods(bool resetSelection = false)
        {
            if (File.Exists("settings.xml"))
            {
                objSettings.LoadSettings();
            }
            else
            {
                objSettings.ModEntries = new List<ModEntry>();
            }

            listInstalledMods.Items.Clear();
            foreach (ModEntry mod in objSettings.ModEntries)
            {
                listInstalledMods.Items.Add(mod.Name);
            }

            if (resetSelection)
            {
                if (listInstalledMods.Items.Count > 0)
                {
                    listInstalledMods.SelectedIndex = 0;
                } else
                {
                    listInstalledMods.SelectedIndex = -1;
                }

            }
        }

        private void buttonUninstallMod_Click(object sender, EventArgs e)
        {
            // Get selected mod
            ModEntry mod = objSettings.ModEntries[listInstalledMods.SelectedIndex];

            showProgressWindow(String.Format("Please wait while {0} is uninstalled...", mod.Name));

            // Uninstall mod
            ModManager.UninstallMod(mod);

            // Remove from mod database
            objSettings.ModEntries.Remove(mod);
            objSettings.SaveSettings();

            // Update DAT hash
            Properties.Settings.Default.DatHash = ModManager.HashFile(ModManager.GameArchivePath);
            Properties.Settings.Default.Save();

            // Update installed mod list
            LoadInstalledMods(true);
            hideProgressWindow();
        }

        private void buttonInstallMod_Click(object sender, EventArgs e)
        {
            // Show open file dialog for mod file
            OpenFileDialog openModFile = new OpenFileDialog();
            openModFile.Filter = "MGSV Mod Files|*.mgsv|All Files|*.*";
            DialogResult ofdResult = openModFile.ShowDialog();
            if (ofdResult != DialogResult.OK) return;

            // extract metadata and load
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(openModFile.FileName, ".", "metadata.xml");

            ModEntry modMetadata = new ModEntry();
            modMetadata.ReadFromFile("metadata.xml");
            File.Delete("metadata.xml"); // delete temp metadata

            // search installed mods for conflicts
            List<string> conflictingMods = new List<string>();

            foreach (ModEntry modEntry in objSettings.ModEntries) // iterate through installed mods
            {
                foreach (ModQarEntry qarEntry in modMetadata.ModQarEntries) // iterate qar files from new mod
                {
                    ModQarEntry conflicts = modEntry.ModQarEntries.FirstOrDefault(entry => entry.FilePath == qarEntry.FilePath);
                    if (conflicts != null)
                    {
                        conflictingMods.Add(modEntry.Name);
                        break;
                    }
                }

                foreach (ModFpkEntry fpkEntry in modMetadata.ModFpkEntries) // iterate fpk files from new mod
                {
                    ModFpkEntry conflicts = modEntry.ModFpkEntries.FirstOrDefault(entry => entry.FpkFile == fpkEntry.FpkFile && entry.FilePath == fpkEntry.FilePath);
                    if (conflicts != null)
                    {
                        if (!conflictingMods.Contains(modEntry.Name))
                        {
                            conflictingMods.Add(modEntry.Name);
                            break;
                        }
                    }
                }
            }

            // if the mod conflicts, display message
            if (conflictingMods.Count > 0)
            {
                string msgboxtext = "The selected mod conflicts with these mods:\n";
                foreach (string Conflict in conflictingMods)
                {
                    msgboxtext += Conflict + "\n";
                }
                msgboxtext += "\nPlease uninstall the mods above and try again.";
                MessageBox.Show(msgboxtext, "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult confirmInstall = MessageBox.Show(String.Format("You are about to install {0}, continue?", modMetadata.Name), "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmInstall == DialogResult.No) return;

            showProgressWindow(String.Format("Installing {0}, please wait...", modMetadata.Name));

            // Install mod to 01.dat
            ModManager.InstallMod(openModFile.FileName);

            // Update DAT hash
            Properties.Settings.Default.DatHash = ModManager.HashFile(ModManager.GameArchivePath);
            Properties.Settings.Default.Save();

            // Install mod to game database
            objSettings.ModEntries.Add(modMetadata);
            objSettings.SaveSettings();
            LoadInstalledMods();
            listInstalledMods.SelectedIndex = listInstalledMods.Items.Count - 1;

            hideProgressWindow();
        }

        private void listInstalledMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listInstalledMods.SelectedIndex >= 0)
            {
                ModEntry selectedMod = objSettings.ModEntries[listInstalledMods.SelectedIndex];
                labelModName.Text = selectedMod.Name;
                labelModVersion.Text = selectedMod.Version;
                labelModAuthor.Text = selectedMod.Author;
                labelModWebsite.Text = selectedMod.Website;
                textDescription.Text = selectedMod.Description;
            }
        }


        private void buttonFindMGSV_Click(object sender, EventArgs e)
        {
            OpenFileDialog findMGSV = new OpenFileDialog();
            findMGSV.Filter = "Metal Gear Solid V|MGSVTPP.exe";
            DialogResult findResult = findMGSV.ShowDialog();
            if (findResult != DialogResult.OK) return;

            string filePath = findMGSV.FileName.Substring(0, findMGSV.FileName.LastIndexOf("\\"));
            textInstallPath.Text = filePath;
            Properties.Settings.Default.InstallPath = filePath;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ulong test = Hashing.HashFileExtension("fpk");
        }

        private void showProgressWindow(string Text = "Processing...")
        {
            progWindow.Owner = this;
            progWindow.StatusText.Text = Text;

            progWindow.Show(this);
            this.Enabled = false;
        }

        private void hideProgressWindow()
        {

            this.Enabled = true;
            progWindow.Hide();
            
        }

        private void buttonBuildGameDB_Click(object sender, EventArgs e)
        {
            // clears all mods, resets game data files
            if(sender!=null)
            {
                DialogResult areYouSure = MessageBox.Show("Currently installed mods will be merged with base game data and you will no longer be able to uninstall them. Are you sure?", "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (areYouSure == DialogResult.No) return;
            }

            showProgressWindow("Rebuilding game data cache...");

            objSettings.GameData = ModManager.BuildGameData();
            objSettings.ModEntries = new List<ModEntry>();
            objSettings.SaveSettings();

            LoadInstalledMods();

            hideProgressWindow();
            tabControl.SelectedIndex = 0;
        }

        private void checkConflicts_CheckedChanged(object sender, EventArgs e)
        {
            if(checkConflicts.Checked)
            {
                MessageBox.Show("Disabling conflict checking will allow SnakeBite to overwrite existing files, potentially causing unpredictable results. It is recommended you backup 01.dat before continuing!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}