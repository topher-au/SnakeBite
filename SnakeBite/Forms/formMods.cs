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

        public void ProcessInstallMod(string ModFile, bool ignoreConflicts = false,bool skipCleanup=false)
        {
            var metaData = Tools.ReadMetaData(ModFile);
            if (metaData == null) return;

            if (!ModManager.CheckConflicts(ModFile, ignoreConflicts)) return;

            ProgressWindow.Show("Installing Mod", String.Format("Installing {0}...", metaData.Name), new Action((MethodInvoker)delegate { ModManager.InstallMod(ModFile,skipCleanup); }));

            this.Invoke((MethodInvoker)delegate { RefreshInstalledMods(); });
        }

        public void ProcessUninstallMod(ModEntry mod)
        {
            ProgressWindow.Show("Uninstalling Mod", String.Format("Uninstalling {0}...", mod.Name), new Action((MethodInvoker)delegate { ModManager.UninstallMod(mod); }));
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
            // Check if web mods have already been loaded
            if (webMods != null) return;

            // Download and populate mod list
            webMods = WebManager.GetOnlineMods();
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

        private void buttonInstallZip_Click(object sender, EventArgs e)
        {
            QuickMod.formQuickMod qm = new QuickMod.formQuickMod();
            qm.ShowDialog();
            RefreshInstalledMods(true);
        }
    }
}