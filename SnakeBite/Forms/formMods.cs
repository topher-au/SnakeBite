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
    public partial class formMods : Form
    {
        private formProgress progWindow = new formProgress();
        private List<WebMod> webMods;

        public formMods()
        {
            InitializeComponent();
        }

        private delegate void GoToModListDelegate();



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

            // Refresh button state
            UpdateModToggle();
            RefreshInstalledMods(true);

            // Show form before continuing
            this.Show();

            if (BackupManager.ModsDisabled())
                MessageBox.Show("Mods are currently disabled. To install or uninstall mods, please click Enable Mods in the Settings menu.", "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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

        public void ProcessInstallMod(string ModFile, bool ignoreConflicts = false)
        {
            // extract metadata and load
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(ModFile, ".", "metadata.xml");

            ModEntry metaData = new ModEntry("metadata.xml");
            File.Delete("metadata.xml"); // delete temp metadata

            if (!SettingsManager.DisableConflictCheck && !ignoreConflicts)
            {
                // check version conflicts
                var SBVersion = ModManager.GetSBVersion();
                var MGSVersion = ModManager.GetMGSVersion();

                Version modSBVersion = new Version();
                Version modMGSVersion = new Version();
                try
                {
                    modSBVersion = metaData.SBVersion.AsVersion();
                    modMGSVersion = metaData.MGSVersion.AsVersion();
                } catch
                {
                    MessageBox.Show(String.Format("The selected version of {0} was created with an older version of SnakeBite and is no longer compatible, please download the latest version and try again.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                

                // Check if mod requires SB update
                if (modSBVersion > SBVersion)
                {
                    MessageBox.Show(String.Format("{0} requires a newer version of SnakeBite. Please follow the link on the Settings page to get the latest version.", metaData.Name), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (modSBVersion < new Version(0,8,0,0))
                {
                    MessageBox.Show(String.Format("The selected version of {0} was created with an older version of SnakeBite and is no longer compatible, please download the latest version and try again.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check MGS version compatibility
                if (MGSVersion != modMGSVersion && modMGSVersion != new Version(0,0,0,0))
                {
                    if (MGSVersion > modMGSVersion && modMGSVersion > new Version(0,0,0,0))
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

                Debug.LogLine(String.Format("[Mod] Checking conflicts for {0}", metaData.Name));
                int confCounter = 0;
                // search installed mods for conflicts
                var mods = SettingsManager.GetInstalledMods();
                List<string> conflictingMods = new List<string>();
                int confIndex = -1;
                foreach (ModEntry mod in mods) // iterate through installed mods
                {
                    foreach (ModQarEntry qarEntry in metaData.ModQarEntries) // iterate qar files from new mod
                    {
                        if (qarEntry.FilePath.Contains(".fpk")) continue;
                        ModQarEntry conflicts = mod.ModQarEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FilePath, qarEntry.FilePath));
                        if (conflicts != null)
                        {
                            if (confIndex == -1) confIndex = mods.IndexOf(mod);
                            if(!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                            Debug.LogLine(String.Format("[{0}] Conflict in 00.dat: {1}", mod.Name, conflicts.FilePath));
                            confCounter++;
                        }
                    }

                    foreach (ModFpkEntry fpkEntry in metaData.ModFpkEntries) // iterate fpk files from new mod
                    {
                        ModFpkEntry conflicts = mod.ModFpkEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FpkFile, fpkEntry.FpkFile) &&
                                                                                               Tools.CompareHashes(entry.FilePath, fpkEntry.FilePath));
                        if (conflicts != null)
                        {
                            if (confIndex == -1) confIndex = mods.IndexOf(mod);
                            if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                            Debug.LogLine(String.Format("[{0}] Conflict in {2}: {1}", mod.Name, conflicts.FilePath, Path.GetFileName(conflicts.FpkFile)));
                            confCounter++;
                        }
                    }
                }

                // if the mod conflicts, display message

                if (conflictingMods.Count > 0)
                {
                    Debug.LogLine(String.Format("[Mod] Found {0} conflicts", confCounter));
                    string msgboxtext = "The selected mod conflicts with these mods:\n";
                    foreach (string Conflict in conflictingMods)
                    {
                        msgboxtext += Conflict + "\n";
                    }
                    msgboxtext += "\nMore information regarding the conflicts has been output to the logfile. Double click the version number shown in the Launcher to view the current logfile.";
                    this.Invoke((MethodInvoker)delegate { listInstalledMods.SelectedIndex = confIndex; });
                    MessageBox.Show(msgboxtext, "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Debug.LogLine("[Mod] No conflicts found");

                bool sysConflict = false;
                // check for system file conflicts
                var gameData = SettingsManager.GetGameData();
                foreach (ModQarEntry gameQarFile in gameData.GameQarEntries.FindAll(entry => entry.SourceType == FileSource.System))
                {
                    if (metaData.ModQarEntries.Count(entry => Tools.ToQarPath(entry.FilePath) == Tools.ToQarPath(gameQarFile.FilePath)) > 0) sysConflict = true;
                }

                foreach (ModFpkEntry gameFpkFile in gameData.GameFpkEntries.FindAll(entry => entry.SourceType == FileSource.System))
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

            ProgressWindow.Show("Installing Mod", "", new Action((MethodInvoker)delegate { ModManager.InstallMod(ModFile); }));

            this.Invoke((MethodInvoker)delegate { RefreshInstalledMods(); });
            hideProgressWindow();
        }

        public void ProcessUninstallMod(ModEntry mod)
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
            buttonInstallMod.Enabled = enabled;
            buttonUninstallMod.Enabled = enabled;
            buttonWebInstall.Enabled = enabled;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (webMods != null) return;

            webMods  = WebManager.GetOnlineMods();
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
                tabControl.SelectedIndex = 0;
                tabControl.TabPages.RemoveAt(1);
            }
        }
    }
}