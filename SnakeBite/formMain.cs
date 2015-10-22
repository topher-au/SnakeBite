using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SnakeBite
{
    public partial class formMain : Form
    {
        private formProgress progWindow = new formProgress();

        public formMain()
        {
            InitializeComponent();
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            labelVersion.Text = Application.ProductVersion;

            bool resetPath = false;

            // Verify the saved MGSV install directory
            if (!SettingsManager.ValidInstallPath)
            {
                MessageBox.Show("Please locate your MGSV installation to continue. If this is your first time running SnakeBite, it is recommended that you reset your MGSV installation before continuing.", "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Show();
                tabControl.SelectedIndex = 1;
                buttonFindMGSV_Click(null, null);
                if (!SettingsManager.ValidInstallPath)
                    Application.Exit(); // Force user to specify valid installation directory

                resetPath = true;
            }

            // Set installation path textbox
            textInstallPath.Text = Properties.Settings.Default.InstallPath;

            // Check if we need to migrate the old settings.xml to new location
            if (File.Exists("settings.xml"))
            {
                File.Move("settings.xml", ModManager.GameDir + "\\sbmods.xml");
            }

            if (!File.Exists(Path.Combine(ModManager.GameDir, "sbmods.xml")))
            {
                Settings settings = new Settings();
                settings.Save();
            }

            // Load settings and update installed mod list
            RefreshInstalledMods(true);

            var checkDat = SettingsManager.ValidateDatHash();

            // Show form before continuing
            this.Show();

            // check hash for dat file, if changed, rebuild database
            if (!checkDat)
            {
                if (!resetPath)
                    MessageBox.Show("Game data modified outside of SnakeBite. SnakeBite will now attempt to recache game data.",
                                    "Game data hash mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonBuildGameDB_Click(null, null);
            }

            // Process command line arguments

            string[] args = Environment.GetCommandLineArgs();
            if (args.Count() > 1)
            {
                switch (args[1])
                {
                    case "-i":
                        {
                            ProcessInstallMod(args[2], true); // install mod ignoring conflicts
                            break;
                        }

                    case "-u":
                        {
                            var mods = SettingsManager.GetInstalledMods();
                            ModEntry mod = mods.FirstOrDefault(entry => entry.Name == args[2]); // find matching mod name
                            if (mod == null) return;
                            ProcessUninstallMod(mod); // uninstall mod
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void RefreshInstalledMods(bool resetSelection = false)
        {
            var mods = SettingsManager.GetInstalledMods();

            listInstalledMods.Items.Clear();

            if (mods.Count > 0)
            {
                panelModDetails.Visible = true;
                labelNoMods.Visible = false;
                foreach (ModEntry mod in mods)
                {
                    listInstalledMods.Items.Add(mod.Name);
                }

                if (resetSelection)
                {
                    if (listInstalledMods.Items.Count > 0)
                    {
                        listInstalledMods.SelectedIndex = 0;
                    }
                    else
                    {
                        listInstalledMods.SelectedIndex = -1;
                    }
                }
            }
            else
            {
                panelModDetails.Visible = false;
                labelNoMods.Visible = true;
            }
        }

        private void buttonUninstallMod_Click(object sender, EventArgs e)
        {
            if (!(listInstalledMods.SelectedIndex >= 0)) return;

            // Get selected mod
            var mods = SettingsManager.GetInstalledMods();
            ModEntry mod = mods[listInstalledMods.SelectedIndex];
            if (!(MessageBox.Show(String.Format("{0} will be uninstalled.", mod.Name), "SnakeBite", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)) return;

            ProcessUninstallMod(mod);
        }

        private void ProcessUninstallMod(ModEntry mod)
        {
            showProgressWindow(String.Format("Please wait while {0} is uninstalled...", mod.Name));

            // Uninstall mod
            System.ComponentModel.BackgroundWorker uninstaller = new System.ComponentModel.BackgroundWorker();
            uninstaller.DoWork += (obj, e) => ModManager.UninstallMod(mod);
            uninstaller.RunWorkerAsync();

            while (uninstaller.IsBusy)
            {
                Application.DoEvents();
            }

            // Remove from mod database
            SettingsManager.RemoveMod(mod);
            SettingsManager.UpdateDatHash();

            // Update installed mod list
            RefreshInstalledMods(true);
            hideProgressWindow();
        }

        private void buttonInstallMod_Click(object sender, EventArgs e)
        {
            // Show open file dialog for mod file
            OpenFileDialog openModFile = new OpenFileDialog();
            openModFile.Filter = "MGSV Mod Files|*.mgsv|All Files|*.*";
            DialogResult ofdResult = openModFile.ShowDialog();
            if (ofdResult != DialogResult.OK) return;

            ProcessInstallMod(openModFile.FileName);
        }

        private void ProcessInstallMod(string ModFile, bool ignoreConflicts = false)
        {
            // extract metadata and load
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(ModFile, ".", "metadata.xml");

            ModEntry metaData = new ModEntry();
            metaData.ReadFromFile("metadata.xml");
            File.Delete("metadata.xml"); // delete temp metadata

            if (!checkConflicts.Checked && !ignoreConflicts)
            {
                // check version conflicts
                int SBVersion = ModManager.GetSBVersion();
                int MGSVersion = ModManager.GetMGSVersion();

                int modSBVersion = Convert.ToInt32(metaData.SBVersion);
                int modMGSVersion = Convert.ToInt32(metaData.MGSVersion);

                // Check if mod requires SB update
                if (modSBVersion > SBVersion)
                {
                    MessageBox.Show(String.Format("{0} requires a newer version of SnakeBite. Please follow the link on the Settings page to get the latest version.", metaData.Name), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check MGS version compatibility
                if (MGSVersion != modMGSVersion && modMGSVersion != 0)
                {
                    if (MGSVersion > modMGSVersion) MessageBox.Show(String.Format("{0} requires MGSV version {1}, but your installation is version {2}. Please update {0} and try again.", metaData.Name, modMGSVersion, MGSVersion), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (MGSVersion < modMGSVersion) MessageBox.Show(String.Format("{0} requires MGSV version {1}, but your installation is version {2}. Please update MGSV and try again.", metaData.Name, modMGSVersion, MGSVersion), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // search installed mods for conflicts
                var mods = SettingsManager.GetInstalledMods();
                List<string> conflictingMods = new List<string>();

                foreach (ModEntry mod in mods) // iterate through installed mods
                {
                    foreach (ModQarEntry qarEntry in metaData.ModQarEntries) // iterate qar files from new mod
                    {
                        if (qarEntry.FilePath.Contains(".fpk")) continue;
                        ModQarEntry conflicts = mod.ModQarEntries.FirstOrDefault(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(qarEntry.FilePath));
                        if (conflicts != null)
                        {
                            conflictingMods.Add(mod.Name);
                            break;
                        }
                    }

                    foreach (ModFpkEntry fpkEntry in metaData.ModFpkEntries) // iterate fpk files from new mod
                    {
                        ModFpkEntry conflicts = mod.ModFpkEntries.FirstOrDefault(entry => Tools.NameToHash(entry.FpkFile) == Tools.NameToHash(fpkEntry.FpkFile) &&
                                                                                               Tools.NameToHash(entry.FilePath) == Tools.NameToHash(fpkEntry.FilePath));
                        if (conflicts != null)
                        {
                            if (!conflictingMods.Contains(mod.Name))
                            {
                                conflictingMods.Add(mod.Name);
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

                bool sysConflict = false;
                // check for system file conflicts
                var gameData = SettingsManager.GetGameData();
                foreach (ModQarEntry gameQarFile in gameData.GameQarEntries)
                {
                    if (metaData.ModQarEntries.Count(entry => Tools.ToQarPath(entry.FilePath) == Tools.ToQarPath(gameQarFile.FilePath)) > 0) sysConflict = true;
                }

                foreach (ModFpkEntry gameFpkFile in gameData.GameFpkEntries)
                {
                    if (metaData.ModFpkEntries.Count(entry => entry.FilePath == gameFpkFile.FilePath && entry.FpkFile == gameFpkFile.FpkFile) > 0) sysConflict = true;
                }
                if (sysConflict)
                {
                    MessageBox.Show("The selected mod conflicts with existing MGSV system files.", "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            DialogResult confirmInstall = MessageBox.Show(String.Format("You are about to install {0}, continue?", metaData.Name), "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmInstall == DialogResult.No) return;

            showProgressWindow(String.Format("Installing {0}, please wait...", metaData.Name));

            // Install mod to 01.dat
            System.ComponentModel.BackgroundWorker installer = new System.ComponentModel.BackgroundWorker();
            installer.DoWork += (obj, e) => ModManager.InstallMod(ModFile);
            installer.RunWorkerAsync();

            while (installer.IsBusy)
            {
                Application.DoEvents();
            }

            SettingsManager.UpdateDatHash();

            // Install mod to game database
            SettingsManager.AddMod(metaData);

            RefreshInstalledMods();
            listInstalledMods.SelectedIndex = listInstalledMods.Items.Count - 1;

            hideProgressWindow();
        }

        private void listInstalledMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            // populate details pane
            if (listInstalledMods.SelectedIndex >= 0)
            {
                var mods = SettingsManager.GetInstalledMods();
                ModEntry selectedMod = mods[listInstalledMods.SelectedIndex];
                labelModName.Text = selectedMod.Name;
                labelModVersion.Text = selectedMod.Version;
                labelModAuthor.Text = "by " + selectedMod.Author;
                labelModAuthor.Left = labelModName.Left + labelModName.Width + 4;
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
            if (filePath != textInstallPath.Text)
            {
                textInstallPath.Text = filePath;
                Properties.Settings.Default.InstallPath = filePath;
                Properties.Settings.Default.Save();
                MessageBox.Show("SnakeBite will now restart.", "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start("SnakeBite.exe");
                Application.Exit();
            }
        }

        private void showProgressWindow(string Text = "Processing...")
        {
            progWindow.Owner = this;
            progWindow.StatusText.Text = Text;

            progWindow.ShowInTaskbar = false;
            progWindow.Show();
            this.Enabled = false;
        }

        private void hideProgressWindow()
        {
            this.Enabled = true;
            progWindow.Hide();
        }

        private void buttonBuildGameDB_Click(object sender, EventArgs e)
        {
            // attempt to determine which files are game files and which are mod files
            if (sender != null)
            {
                DialogResult areYouSure = MessageBox.Show("SnakeBite will read the installed game data and remove any mod files that are missing. Corrupted mods may then be uninstalled.\n\nThis may take some time.", "SnakeBite", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (areYouSure == DialogResult.Cancel) return;
            }

            showProgressWindow("Rebuilding game data cache...");

            // REBUILD GAME DATA
            // CLEAN UP MOD SETTINGS

            System.ComponentModel.BackgroundWorker rebuilder = new System.ComponentModel.BackgroundWorker();
            rebuilder.DoWork += (obj, var) => ModManager.CleanupDatabase();
            rebuilder.RunWorkerAsync();
            while (rebuilder.IsBusy)
            {
                Application.DoEvents();
            }

            SettingsManager.UpdateDatHash();

            RefreshInstalledMods(true);

            hideProgressWindow();

            if (sender != null) tabControl.SelectedIndex = 0;
        }

        private void DoBuildDB()
        {
        }

        private void checkConflicts_CheckedChanged(object sender, EventArgs e)
        {
            if (checkConflicts.Checked)
            {
                MessageBox.Show("Disabling conflict checking will allow SnakeBite to overwrite existing files, potentially causing unpredictable results. It is recommended you backup 01.dat before continuing!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void labelModAuthor_Click(object sender, EventArgs e)
        {
        }

        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("steam://run/287700/");
            Application.Exit();
        }

        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.labelGithub.Text);
        }

        private void buttonCreateBackup_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("_backup")) Directory.Delete("_backup", true);
            Directory.CreateDirectory("_backup");

            // prompt user for backup filename
            SaveFileDialog saveBackup = new SaveFileDialog();
            saveBackup.Filter = "SnakeBite Backup|*.sbb";
            DialogResult saveResult = saveBackup.ShowDialog();
            if (saveResult != DialogResult.OK) return;

            // copy current settings
            File.Copy(ModManager.GameDir + "\\sbmods.xml", "_backup\\sbmods.xml");

            // copy current 01.dat
            File.Copy(ModManager.DatPath, "_backup\\01.dat");

            // compress to backup
            FastZip zipper = new FastZip();
            zipper.CreateZip(saveBackup.FileName, "_backup", true, "(.*?)");

            Directory.Delete("_backup", true);

            MessageBox.Show("Backup complete.", "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonRestoreBackup_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("_backup")) Directory.Delete("_backup", true);

            OpenFileDialog openBackup = new OpenFileDialog();
            openBackup.Filter = "SnakeBite Backup|*.sbb";
            DialogResult openResult = openBackup.ShowDialog();
            if (openResult != DialogResult.OK) return;

            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(openBackup.FileName, "_backup", "(.*?)");

            File.Copy("_backup\\01.dat", ModManager.DatPath, true);
            File.Copy("_backup\\sbmods.xml", ModManager.GameDir + "\\sbmods.xml", true);

            Directory.Delete("_backup", true);

            MessageBox.Show("Backup successfully restored!\nSnakeBite will now restart.", "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Information);
            System.Diagnostics.Process.Start("SnakeBite.exe");
            Application.Exit();
        }

        private void labelModWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.labelModWebsite.Text);
        }
    }
}