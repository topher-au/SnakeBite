using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

        private delegate void GoToModListDelegate();

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

        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            Process.Start("steam://run/287700/");
            Application.Exit();
        }

        private void buttonRestoreOriginals_Click(object sender, EventArgs e)
        {
            var restoreData = MessageBox.Show("Your original backup files will be restored, and any SnakeBite settings and mods will be completely removed.\n\nAre you sure you want to continue?", "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (restoreData == DialogResult.No) return;

            if (BackupManager.OriginalsExist())
            {
                BackupManager.RestoreOriginals();
                SettingsManager.DeleteSettings();
                Application.Exit();
            }
        }

        private void buttonSetup(object sender, EventArgs e)
        {
            SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
            setupWizard.Tag = "closable";
            setupWizard.ShowDialog();
            CheckBackupState();
        }

        private void buttonToggleMods_Click(object sender, EventArgs e)
        {
            showProgressWindow("Please wait...");

            BackgroundWorker toggleWorker = new BackgroundWorker();

            if (BackupManager.ModsDisabled())
            {
                // re-enable mods
                toggleWorker.DoWork += (obj, var) => BackupManager.SwitchToMods();
            }
            else
            {
                // disable mods
                toggleWorker.DoWork += (obj, var) => BackupManager.SwitchToOriginal();
            }

            toggleWorker.RunWorkerAsync();
            while (toggleWorker.IsBusy)
            {
                Application.DoEvents();
            }

            UpdateModToggle();

            hideProgressWindow();
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

        private void buttonWebRemove_Click(object sender, EventArgs e)
        {
            var dl = webMods[listWebMods.SelectedIndex];
            var file = Path.Combine("downloaded", dl.DownloadUrl.Substring(dl.DownloadUrl.LastIndexOf("/") + 1));

            if (File.Exists(file)) File.Delete(file);
            buttonWebInstall.Text = (File.Exists(file)) ? "Install" : "Download";
            buttonWebRemove.Visible = (File.Exists(file)) ? true : false;
        }

        private void CheckBackupState()
        {
            bool backupExists = BackupManager.OriginalsExist();
            {
                buttonToggleMods.Enabled = backupExists;
                buttonRestoreOriginals.Enabled = backupExists;
            }
        }

        private void checkConflicts_CheckedChanged(object sender, EventArgs e)
        {
            if (checkConflicts.Checked)
            {
                MessageBox.Show("Enabling this option completely disables any warnings when installing mods, and may overwrite existing mod or game data.\n\nThis may cause issues with the mods or cause the game to hang, proceed with caution.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                downloader.DoWork += (obj, e) => WebManager.DownloadModFile(mod.DownloadUrl, dlName);
                downloader.RunWorkerAsync();
                while (downloader.IsBusy)
                {
                    Application.DoEvents();
                }
                hideProgressWindow();
            }
            else
            {
                GoToModList();
                ProcessInstallMod(dlName);
                this.Invoke((MethodInvoker)delegate
                {
                    RefreshInstalledMods();
                    listInstalledMods.SelectedIndex = listInstalledMods.Items.Count - 1;
                });
            }

            this.Invoke((MethodInvoker)delegate
            {
                // update buttons
                buttonWebInstall.Text = (File.Exists(dlName)) ? "Install" : "Download";
                buttonWebRemove.Visible = (File.Exists(dlName)) ? true : false;
            });
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            labelVersion.Text = Application.ProductVersion;

            // Set installation path textbox
            textInstallPath.Text = Properties.Settings.Default.InstallPath;

            // Show wizard on first run, if folder is invalid or settings out of date
            if (!SettingsManager.SettingsExist() || !SettingsManager.ValidInstallPath || SettingsManager.GetSettingsVersion() < 600)
            {
                // show setup wizard
                SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
                setupWizard.ShowDialog();
            }

            // force user to run setup wizard
            if (!SettingsManager.SettingsExist() || !SettingsManager.ValidInstallPath || SettingsManager.GetSettingsVersion() < 600)
            {
                Application.Exit();
            }

            // Populate web mod list
            if (webMods.Count > 0)
            {
                foreach (WebMod webMod in webMods)
                {
                    listWebMods.Items.Add(webMod.Name);
                }
                listWebMods.SelectedIndex = 0;
            }
            else
            {
                tabControl.TabPages.RemoveAt(1);
            }

            // Process command line arguments

            string[] args = Environment.GetCommandLineArgs();
            bool doCmdLine = false;
            bool closeApp = false;
            bool install = false;
            bool ignoreConflicts = false;
            bool resetDatHash = false;
            string installFile = String.Empty;
            if (args.Count() > 1)
            {
                foreach (string arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "-i":
                            install = true;
                            break;

                        case "-u":
                            install = false;
                            break;

                        case "-c":
                            ignoreConflicts = true;
                            break;

                        case "-d":
                            resetDatHash = true;
                            break;

                        case "-x":
                            closeApp = true;
                            break;

                        default:
                            if (Path.GetExtension(arg) != ".mgsv") break;
                            if (File.Exists(arg))
                            {
                                installFile = arg;
                                doCmdLine = true;
                            }
                            break;
                    }
                }
            }

            if(resetDatHash)
            {
                SettingsManager.UpdateDatHash();
            }

            var checkDat = SettingsManager.ValidateDatHash();
            if (!checkDat)
            {
                MessageBox.Show("The game data appears to have been modified outside of SnakeBite.\n\nThe Setup Wizard will now run.", "Game data hash mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
                setupWizard.ShowDialog();
            }

            RefreshInstalledMods(true);

            if (doCmdLine)
            {
                if (install)
                {
                    // install
                    ProcessInstallMod(installFile, ignoreConflicts); // install mod
                }
                else
                {
                    // uninstall
                    var mods = SettingsManager.GetInstalledMods();
                    ModEntry mod = mods.FirstOrDefault(entry => entry.Name == args[2]); // select mod

                    if (mod != null)
                        ProcessUninstallMod(mod); // uninstall mod
                }
                if (closeApp) Application.Exit();
            }

            // Refresh button state
            UpdateModToggle();
            CheckBackupState();

            // Show form before continuing
            this.Show();
        }

        private void GoToModList()
        {
            if (tabControl.InvokeRequired)
            {
                tabControl.Invoke(new GoToModListDelegate(GoToModList));
            }
            else
            {
                tabControl.SelectedIndex = 0;
            }
        }

        private void hideProgressWindow()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Enabled = true;
                progWindow.Hide();
            });
        }

        private void labelModWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.labelModWebsite.Text);
        }

        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.labelGithub.Text);
        }

        private void listInstalledMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Populate mod details pane
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

        private void listWebMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Populate online mod details pane
            if (listWebMods.SelectedIndex >= 0)
            {
                WebMod selectedWebMod = webMods[listWebMods.SelectedIndex];
                labelWebName.Text = selectedWebMod.Name;
                labelWebVersion.Text = selectedWebMod.Version;
                labelWebAuthor.Text = "by " + selectedWebMod.Author;
                labelWebAuthor.Left = labelWebName.Left + labelWebName.Width + 4;
                labelWebWebsite.Text = selectedWebMod.Website;
                textWebDescription.Text = selectedWebMod.Description;
                string modUrl = selectedWebMod.DownloadUrl;
                if(modUrl == "browse:")
                {
                    buttonWebInstall.Text = "Website";
                    buttonWebRemove.Visible = false;
                } else
                {
                    string dlName = Path.Combine("downloaded", selectedWebMod.DownloadUrl.Substring(selectedWebMod.DownloadUrl.LastIndexOf("/") + 1));
                    buttonWebInstall.Text = (File.Exists(dlName)) ? "Install" : "Download";
                    buttonWebRemove.Visible = (File.Exists(dlName)) ? true : false;
                }
            }
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

                if (modSBVersion < 500)
                {
                    MessageBox.Show(String.Format("The selected version of {0} was created with an older version of SnakeBite may cause issues, please download the latest version and try again.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check MGS version compatibility
                if (MGSVersion != modMGSVersion && modMGSVersion != 0)
                {
                    if (MGSVersion > modMGSVersion)
                    {
                        var contInstall = MessageBox.Show(String.Format("{0} appears to be for an older version of MGSV. It is recommended that you at least check for an updated version before installing.\n\nContinue installation?", metaData.Name, modMGSVersion, MGSVersion), "Game version mismatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (contInstall == DialogResult.No) return;
                    }
                    if (MGSVersion < modMGSVersion)
                    {
                        MessageBox.Show(String.Format("{0} requires MGSV version {1}, but your installation is version {2}. Please update MGSV and try again.", metaData.Name, modMGSVersion, MGSVersion), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // search installed mods for conflicts
                var mods = SettingsManager.GetInstalledMods();
                List<string> conflictingMods = new List<string>();
                int confIndex = -1;
                foreach (ModEntry mod in mods) // iterate through installed mods
                {
                    foreach (ModQarEntry qarEntry in metaData.ModQarEntries) // iterate qar files from new mod
                    {
                        if (qarEntry.FilePath.Contains(".fpk")) continue;
                        ModQarEntry conflicts = mod.ModQarEntries.FirstOrDefault(entry => Tools.NameToHash(entry.FilePath) == Tools.NameToHash(qarEntry.FilePath));
                        if (conflicts != null)
                        {
                            if (confIndex == -1) confIndex = mods.IndexOf(mod);
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
                                if (confIndex == -1) confIndex = mods.IndexOf(mod);
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
                    this.Invoke((MethodInvoker)delegate { listInstalledMods.SelectedIndex = confIndex; });
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

                DialogResult confirmInstall = MessageBox.Show(String.Format("You are about to install {0}, continue?", metaData.Name), "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmInstall == DialogResult.No) return;
            }

            showProgressWindow(String.Format("Installing {0}, please wait...", metaData.Name));

            // Install mod to 01.dat
            System.ComponentModel.BackgroundWorker installer = new System.ComponentModel.BackgroundWorker();
            installer.DoWork += (obj, e) => ModManager.InstallMod(ModFile);
            installer.RunWorkerAsync();

            while (installer.IsBusy)
            {
                Application.DoEvents();
            }
            this.Invoke((MethodInvoker)delegate { RefreshInstalledMods(); });
            hideProgressWindow();
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

        private void RefreshInstalledMods(bool resetSelection = false)
        {
            var mods = SettingsManager.GetInstalledMods();
            textInstallPath.Text = Properties.Settings.Default.InstallPath;
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

        private void textWebInstall_Click(object sender, EventArgs e)
        {
            var dl = webMods[listWebMods.SelectedIndex];
            if(dl.DownloadUrl == "browse:")
            {
                Process.Start(dl.Website);
            } else
            {
                BackgroundWorker webInstaller = new System.ComponentModel.BackgroundWorker();
                webInstaller.DoWork += (obj, var) => DownloadAndInstallMod(dl);
                webInstaller.RunWorkerAsync();
            }
        }

        private void UpdateModToggle()
        {
            bool enabled = !BackupManager.ModsDisabled();
            buttonToggleMods.Text = enabled ? "Disable Mods" : "Enable Mods";
            buttonInstallMod.Enabled = enabled;
            buttonSetupWizard.Enabled = enabled;
            buttonUninstallMod.Enabled = enabled;
            buttonWebInstall.Enabled = enabled;
            labelModsDisabled.Visible = !enabled;
        }
    }
}