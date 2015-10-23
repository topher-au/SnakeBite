using System.Diagnostics;
using System.ComponentModel;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SnakeBite;
using SnakeBite.SetupWizard;

namespace SnakeBite
{
    public partial class formMain : Form
    {
        private formProgress progWindow = new formProgress();
        private List<WebMod> webMods = WebManager.GetOnlineMods();

        public formMain()
        {
            InitializeComponent();
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            labelVersion.Text = Application.ProductVersion;


            // Set installation path textbox
            textInstallPath.Text = Properties.Settings.Default.InstallPath;


            if (!SettingsManager.SettingsExist() || !SettingsManager.ValidInstallPath || !BackupManager.OriginalsExist())
            {
                // show setup wizard
                SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
                setupWizard.ShowDialog();
            }

            // Populate web mod list
            if(webMods.Count > 0)
            {
                foreach (WebMod webMod in webMods)
                {
                    listWebMods.Items.Add(webMod.Name);
                }
                listWebMods.SelectedIndex = 0;
            } else
            {
                tabControl.TabPages.RemoveAt(1);
            }


            RefreshInstalledMods(true);
            UpdateModToggle();
            // Show form before continuing
            this.Show();


            // Process command line arguments

            string[] args = Environment.GetCommandLineArgs();
            if (args.Count() > 1)
            {
                switch (args[1])
                {
                    case "-i":
                        {   
                            string modPath = Path.Combine(Application.StartupPath, args[2]);

                            if (File.Exists(modPath))
                            {
                                ProcessInstallMod(modPath); // install mod ignoring conflicts
                            } else
                            {
                                MessageBox.Show("File not found.", "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            RefreshInstalledMods(true);

                            return;
                        }

                    case "-u":
                        {
                            var mods = SettingsManager.GetInstalledMods();
                            ModEntry mod = mods.FirstOrDefault(entry => entry.Name == args[2]); // find matching mod name

                            if (mod != null)
                                ProcessUninstallMod(mod); // uninstall mod

                            RefreshInstalledMods(true);

                            return;
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

            // Update installed mod list
            RefreshInstalledMods(true);
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

            RefreshInstalledMods();

            listInstalledMods.SelectedIndex = listInstalledMods.Items.Count - 1;
        }

        private void ProcessInstallMod(string ModFile, bool ignoreConflicts = false)
        {

            // extract metadata and load
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(ModFile, ".", "metadata.xml");

            ModEntry metaData = new ModEntry("metadata.xml");
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

                if(modSBVersion < 500)
                {
                    MessageBox.Show(String.Format("The selected version of {0} was created for an older version of SnakeBite and is not compatible, please download the latest version.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            this.Invoke((MethodInvoker)delegate
           {
               progWindow.Owner = this;
               progWindow.StatusText.Text = Text;

               progWindow.ShowInTaskbar = false;
               progWindow.Show();
               this.Enabled = false;
           });
        }

        private void hideProgressWindow()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Enabled = true;
                progWindow.Hide();
            });
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
            //rebuilder.DoWork += (obj, var) => ModManager.CleanupDatabase();
            //rebuilder.RunWorkerAsync();
            while (rebuilder.IsBusy)
            {
                Application.DoEvents();
            }

            SettingsManager.UpdateDatHash();

            RefreshInstalledMods(true);

            hideProgressWindow();

            if (sender == null) tabControl.SelectedIndex = 0;
        }

        private void DoBuildDB()
        {
        }

        private void checkConflicts_CheckedChanged(object sender, EventArgs e)
        {
            if (checkConflicts.Checked)
            {
                MessageBox.Show("Enabling this option will allow mods to overwrite existing files during installation.\nIt is recommended that you create a backup before continuing as the results may be unpredictable.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void listWebMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            // populate details pane
            if (listWebMods.SelectedIndex >= 0)
            {
                WebMod selectedWebMod = webMods[listWebMods.SelectedIndex];
                labelWebName.Text = selectedWebMod.Name;
                labelWebVersion.Text = selectedWebMod.Version;
                labelWebAuthor.Text = "by " + selectedWebMod.Author;
                labelWebAuthor.Left = labelWebName.Left + labelWebName.Width + 4;
                labelWebWebsite.Text = selectedWebMod.Website;
                textWebDescription.Text = selectedWebMod.Description;
                string dlName = Path.Combine("downloaded", selectedWebMod.DownloadUrl.Substring(selectedWebMod.DownloadUrl.LastIndexOf("/") + 1));
                buttonWebInstall.Text = (File.Exists(dlName)) ? "Install" : "Download";
                buttonWebRemove.Visible = (File.Exists(dlName)) ? true : false;
            }
        }

        private void textWebInstall_Click(object sender, EventArgs e)
        {
            var dl = webMods[listWebMods.SelectedIndex];
            System.ComponentModel.BackgroundWorker webInstaller = new System.ComponentModel.BackgroundWorker();
            webInstaller.DoWork += (obj, var) => DownloadAndInstallMod(dl);
            webInstaller.RunWorkerAsync();
        }

        private void DownloadAndInstallMod(WebMod mod)
        {
            string dlName = Path.Combine("downloaded", mod.DownloadUrl.Substring(mod.DownloadUrl.LastIndexOf("/") + 1));
            
            // download mod
            if (!File.Exists(dlName))
            {
                showProgressWindow(String.Format("{0} is being downloaded...", mod.Name));
                if (!Directory.Exists("downloaded")) Directory.CreateDirectory("downloaded");
                BackgroundWorker downloader = new BackgroundWorker();
                downloader.DoWork += (obj,e) => WebManager.DownloadModFile(mod.DownloadUrl, dlName);
                downloader.RunWorkerAsync();
                while (downloader.IsBusy)
                {
                    Application.DoEvents();
                }
                hideProgressWindow();
            }
                

            // reset to main tab
            GoToModList();

            ProcessInstallMod(dlName);

            this.Invoke((MethodInvoker) delegate {
                buttonWebInstall.Text = (File.Exists(dlName)) ? "Install" : "Download";
                buttonWebRemove.Visible = (File.Exists(dlName)) ? true : false;
                RefreshInstalledMods();
                listInstalledMods.SelectedIndex = listInstalledMods.Items.Count - 1;
            });


        }
        private void GoToModList()
        {
            if(tabControl.InvokeRequired)
            {
                tabControl.Invoke(new GoToModListDelegate(GoToModList));
            } else
            {
                tabControl.SelectedIndex = 0;
            }
            
        }

        private delegate void GoToModListDelegate();

        private void buttonWebRemove_Click(object sender, EventArgs e)
        {
            var dl = webMods[listWebMods.SelectedIndex];
            var file = Path.Combine("downloaded", dl.DownloadUrl.Substring(dl.DownloadUrl.LastIndexOf("/") + 1));

            if (File.Exists(file)) File.Delete(file);
            buttonWebInstall.Text = (File.Exists(file)) ? "Install" : "Download";
            buttonWebRemove.Visible = (File.Exists(file)) ? true : false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
            setupWizard.ShowDialog();
        }

        private void buttonToggleMods_Click(object sender, EventArgs e)
        {
            if(BackupManager.ModsDisabled())
            {
                // re-enable mods
                BackupManager.SwitchToMods();
                UpdateModToggle();
            } else
            {
                // disable mods
                BackupManager.SwitchToOriginal();
                UpdateModToggle();
            }
        }

        private void UpdateModToggle()
        {
            bool enabled = !BackupManager.ModsDisabled();
            buttonToggleMods.Text = enabled ? "Disable Mods": "Enable Mods";
            buttonInstallMod.Enabled = enabled;
            buttonSetupWizard.Enabled = enabled;
            buttonUninstallMod.Enabled = enabled;
            buttonWebInstall.Enabled = enabled;
            labelModsDisabled.Visible = !enabled;
        }

        private void buttonRestoreOriginals_Click(object sender, EventArgs e)
        {
            var restoreData = MessageBox.Show("Your backup files will be restored and any SnakeBite settings will be completely removed.\nAre you sure you want to continue?", "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (restoreData == DialogResult.No) return;

            if(BackupManager.OriginalsExist()) {
                BackupManager.RestoreOriginals();
                SettingsManager.DeleteSettings();
                Application.Exit();
            }
        }
    }
   
}