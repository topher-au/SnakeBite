using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SnakeBite
{
    public partial class formMods : Form
    {
        private formProgress progWindow = new formProgress();
        private int countCheckedMods = 0;
        private int modDescriptionMaxLeft = 300;
        private SettingsManager manager = new SettingsManager(ModManager.GameDir);

        public formMods()
        {
            InitializeComponent();
        }

        private void formMain_Load(object sender, EventArgs e)
        {

            // Refresh button state
            RefreshInstalledMods(true);

            // Show form before continuing
            this.Show();

        }

        private delegate void GoToModListDelegate();

        private void checkBoxMarkAll_Click(object sender, EventArgs e) //Checks all mods if one or more are unchecked, and unchecks all mods if they're all checked.
        {
            checkBoxMarkAll.CheckState = CheckState.Checked; // Keeps the checkbox checked, even after the user has clicked it. using _Click avoids infinite recursion.
            bool isAllChecked = true; // assume all are checked

            for (int i = 0; i < listInstalledMods.Items.Count; i++)
            {
                if (listInstalledMods.GetItemCheckState(i) == CheckState.Unchecked)
                {
                    isAllChecked = false;
                    listInstalledMods.SetItemCheckState(i, CheckState.Checked);
                }
            }
            if (isAllChecked == true) // if still true after the first loop, all boxes are checked. Second loop will uncheck all boxes.
            {
                for (int i = 0; i < listInstalledMods.Items.Count; i++)
                {
                    listInstalledMods.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
        }

        private void buttonInstall_Click(object sender, EventArgs e) //opens directory browser for .mgsv mods, and sends the selected mods to formInstallOrder.
        {
            // Show 'open files' dialog for mod files
            OpenFileDialog openModFile = new OpenFileDialog();
            List<string> ModNames = new List<string>();

            openModFile.Filter = "MGSV Mod Files|*.mgsv|All Files|*.*";
            openModFile.Multiselect = true;
            DialogResult ofdResult = openModFile.ShowDialog();
            if (ofdResult != DialogResult.OK) return;
            foreach (string filename in openModFile.FileNames)
                ModNames.Add(filename);

            formInstallOrder installer = new formInstallOrder();
            installer.ShowDialog(ModNames); // send to formInstallOrder for installation prep.
            RefreshInstalledMods();

            listInstalledMods.SelectedIndex = listInstalledMods.Items.Count - 1;
        }

        private void buttonUninstall_Click(object sender, EventArgs e) //sends checked indices to ModManager for uninstallation.
        {
            // Get the indices of all checked mods, and their names.
            CheckedListBox.CheckedIndexCollection checkedModIndices = listInstalledMods.CheckedIndices;
            CheckedListBox.CheckedItemCollection checkedModItems = listInstalledMods.CheckedItems;
            string markedModNames = "";

            foreach (object mod in checkedModItems)
            {
                markedModNames += "\n" + mod.ToString();
            }
            if (!(MessageBox.Show("The following mods will be uninstalled:\n" + markedModNames, "SnakeBite", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)) return;

            ProgressWindow.Show("Uninstalling Mod(s)", "Uninstalling...\nNote:\nThe uninstall time depends greatly on\nthe mod's contents, the number of mods being uninstalled\nand the mods that are still installed.", new Action((MethodInvoker)delegate { ModManager.UninstallMod(checkedModIndices); }));
            // Update installed mod list
            RefreshInstalledMods(true);
        }

        private void buttonOpenLogs_Click(object sender, EventArgs e)
        {
            Process.Start(Debug.LOG_FILE_PREV);
            Process.Start(Debug.LOG_FILE);
        }

        private void labelModWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //inspired by Nexus Mod Manager, the version number doubles as a link to the webpage.
        {

            var mods = manager.GetInstalledMods();
            ModEntry selectedMod = mods[listInstalledMods.SelectedIndex];
            try
            {
                Process.Start(selectedMod.Website);
            } catch { }
        }

        private void listInstalledMods_SelectedIndexChanged(object sender, EventArgs e)// Populate mod details pane
        {

            if (listInstalledMods.SelectedIndex >= 0)
            {
                var mods = manager.GetInstalledMods();
                ModEntry selectedMod = mods[listInstalledMods.SelectedIndex];
                labelModName.Text = selectedMod.Name;
                labelModAuthor.Text = "By " + selectedMod.Author;
                labelModWebsite.Text = selectedMod.Version;
                textDescription.Text = selectedMod.Description;

                if (manager.IsUpToDate(selectedMod.MGSVersion.AsVersion()))
                {
                    labelVersionWarning.ForeColor = Color.MediumSeaGreen; labelVersionWarning.BackColor = Color.Gainsboro; labelVersionWarning.Text = "✔";
                }
                else
                {
                    labelVersionWarning.ForeColor = Color.Yellow; labelVersionWarning.BackColor = Color.Chocolate; labelVersionWarning.Text = "!";
                }
            }
        }

        private void listInstalledMods_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                countCheckedMods++;
                buttonUninstall.Enabled = true;
            } else
            {
                countCheckedMods--;
                if (countCheckedMods == 0)
                    buttonUninstall.Enabled = false;
            }

        }

        /// <summary>
        /// command-line install.
        /// </summary>
        internal void ProcessInstallMod(string installModPath, bool skipConflictChecks, bool skipCleanup) 
        {
            List<string> InstallFileList = null;
            if (File.Exists(installModPath) && installModPath.Contains(".mgsv"))
            {
                InstallFileList = new List<string> { installModPath };
            } else
            {
                if (Directory.Exists(installModPath))
                {
                    InstallFileList = Directory.GetFiles(installModPath, "*.mgsv").ToList();
                    if (InstallFileList.Count == 0)
                    {
                        Debug.LogLine($"[Install] Could not find any .mgsv files in {installModPath}.");
                        return;
                    }
                } else
                {
                    Debug.LogLine($"[Install] Could not find file or directory {installModPath}.");
                    return;
                }
            }
            if (InstallFileList == null)
            {
                return;
            }
            if (!skipConflictChecks)
            {
                foreach (string modPath in InstallFileList)
                {
                    if (!PreinstallManager.CheckConflicts(modPath)) return;
                }
            }
            ProgressWindow.Show("Installing Mod", $"Installing {installModPath}...",
                new Action((MethodInvoker)delegate { ModManager.InstallMod(InstallFileList, skipCleanup); }
            ));
            this.Invoke((MethodInvoker)delegate { RefreshInstalledMods(); });
        }

        public void ProcessUninstallMod(ModEntry mod)// command-line uninstall. This checks the mod it was passed, and puts it in a 1-item list to be uninstalled.
        {
            for (int i = 0; i < listInstalledMods.Items.Count; i++)
            {
                listInstalledMods.SetItemCheckState(i, CheckState.Unchecked);
            }
            var mods = manager.GetInstalledMods();
            listInstalledMods.SetItemCheckState(mods.IndexOf(mod), CheckState.Checked);
            CheckedListBox.CheckedIndexCollection checkedModIndex = listInstalledMods.CheckedIndices;
            ProgressWindow.Show("Uninstalling Mod", "Uninstalling...", new Action((MethodInvoker)delegate { ModManager.UninstallMod(checkedModIndex); }));
        }

        private void RefreshInstalledMods(bool resetSelection = false) // Clears and then repopulates the installed mod list
        {
            var mods = manager.GetInstalledMods();
            listInstalledMods.Items.Clear();
            countCheckedMods = 0;
            buttonUninstall.Enabled = false;

            if (mods.Count > 0)
            {
                groupBoxNoModsNotice.Visible = false;
                panelModDescription.Visible = true;

                foreach (ModEntry mod in mods)
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
            } else
            {

                groupBoxNoModsNotice.Visible = true;
                panelModDescription.Visible = false;
            }
        }

        private void linkLabelSnakeBiteModsList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // opens the [SBWM] search filter on nexus mods, randomly sorted.
        {
            Process.Start("https://www.nexusmods.com/metalgearsolidvtpp/search/?search_description=SBWM");
        }

        private void buttonLaunchGame_Click(object sender, EventArgs e)
        {
            Process.Start("steam://run/287700/");
            Application.Exit();
        }

        private void buttonOpenGameDir_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Properties.Settings.Default.InstallPath);
            } catch { }
        }

        private void labelVersionWarning_Click(object sender, EventArgs e)
        {
            if (listInstalledMods.SelectedIndex >= 0)
            {
                var mods = manager.GetInstalledMods();
                ModEntry selectedMod = mods[listInstalledMods.SelectedIndex];
                var currentMGSVersion = ModManager.GetMGSVersion();
                var modMGSVersion = selectedMod.MGSVersion.AsVersion();
                if (!manager.IsUpToDate(modMGSVersion)) // checks against intended gameversion
                {
                    if (currentMGSVersion > modMGSVersion) // checks against mgsvtpp.exe productversion
                    {
                        MessageBox.Show(String.Format("{0} appears to be for MGSV Version {1}, and may not be compatible with {2}.\n\nIt is recommended that you check for an updated version.", selectedMod.Name, modMGSVersion,currentMGSVersion), "Game version mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    if (currentMGSVersion < modMGSVersion)
                    {
                        MessageBox.Show(String.Format("{0} is intended for MGSV version {1}, but your installation is version {2}.\n\nThis mod may not be compatible with MGSV version {2}", selectedMod.Name, modMGSVersion, currentMGSVersion), "Update recommended", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                } else
                {
                    MessageBox.Show(String.Format("This mod is up to date with the current MGSV version {0}", currentMGSVersion), "Mod is up to date", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void formMods_Resize(object sender, EventArgs e)
        {
            int modListWidth = this.Width / 3;

            panelModList.Width = modListWidth;
            //if (modListWidth > modDescriptionMaxLeft)
            panelModDescription.Left = panelModList.Width + 6;
            panelModDescription.Width = this.Width - panelModList.Width - 28;
        }
    }
}
