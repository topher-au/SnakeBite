using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.Forms;
using SnakeBite.ModPages;
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
        private SettingsManager manager = new SettingsManager(ModManager.GameDir);

        private ModDescriptionPage modDescription = new ModDescriptionPage();
        private NoInstalledPage noInstallNotice = new NoInstalledPage();

        public formMods()
        {
            InitializeComponent();
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            // Refresh button state
            RefreshInstalledMods(true);

            Location = Properties.Settings.Default.formModsLocation;
            Size = Properties.Settings.Default.formModsSize;

            if (Properties.Settings.Default.formModsMaximized == true)
                WindowState = FormWindowState.Maximized;

            menuItemSkipLauncher.Checked = Properties.Settings.Default.SkipLauncher;
            AdjustSize();

            Show();
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

        private void listInstalledMods_SelectedIndexChanged(object sender, EventArgs e)// Populate mod details pane
        {

            if (listInstalledMods.SelectedIndex >= 0)
            {
                var mods = manager.GetInstalledMods();
                ModEntry selectedMod = mods[listInstalledMods.SelectedIndex];
                modDescription.ShowModInfo(selectedMod);
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
                panelContent.Controls.Clear();
                panelContent.Controls.Add(modDescription);

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
                panelContent.Controls.Clear();
                panelContent.Controls.Add(noInstallNotice);
            }
        }

        private void buttonLaunchGame_Click(object sender, EventArgs e)
        {
            Process.Start("steam://run/287700/");
            Application.Exit();
        }

        private void formMods_Resize(object sender, EventArgs e)
        {
            AdjustSize();
        }

        private void AdjustSize()
        {
            int modListWidth = Width / 3;

            panelModList.Width = modListWidth;

            panelContent.Left = panelModList.Width + 6;
            panelContent.Width = Width - panelModList.Width - 28;

            modDescription.Size = panelContent.Size;
            noInstallNotice.Size = panelContent.Size;
        }

        private void menuItemOpenDir_Click(object sender, EventArgs e)
        {
            string installPath = Properties.Settings.Default.InstallPath;
            try
            {
                Process.Start(installPath);
            }
            catch
            {
                Debug.LogLine(String.Format("Failed to open game directory: {0}", installPath));
            }
        }

        private void menuItemOpenLogs_Click(object sender, EventArgs e)
        {
            Process.Start(Debug.LOG_FILE_PREV);
            Process.Start(Debug.LOG_FILE);
        }

        private void menuItemBrowseMods_Click(object sender, EventArgs e)
        {
            Process.Start(ModManager.SBWMSearchURL);
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItemSkipLauncher_Click(object sender, EventArgs e)
        {
            menuItemSkipLauncher.Checked = !menuItemSkipLauncher.Checked;

            Properties.Settings.Default.SkipLauncher = menuItemSkipLauncher.Checked;
            Properties.Settings.Default.Save();
        }

        private void menuItemOpenSettings_Click(object sender, EventArgs e)
        {
            formSettings Settings = new formSettings();
            Settings.Owner = this;
            Settings.ShowDialog();

            bool modsEnabled = !BackupManager.ModsDisabled(); //TODO: this will require some more methods to ensure that user can't mess with mods while they're disabled

        }

        private void formMods_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.formModsLocation = Location;

            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.formModsSize = Size;
            }
            else
            {
                Properties.Settings.Default.formModsSize = RestoreBounds.Size;
            }

            Properties.Settings.Default.formModsMaximized = (WindowState == FormWindowState.Maximized);

            Properties.Settings.Default.Save();
        }

        private void menuItemHelpInstall_Click(object sender, EventArgs e)
        {
            MessageBox.Show("", "Installing a Mod", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItemHelpUninstall_Click(object sender, EventArgs e)
        {
            MessageBox.Show("", "Uninstalling a Mod", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItemHelpCreate_Click(object sender, EventArgs e) // add a prompt to open makebite
        {
            if(MessageBox.Show("\n\nWould you like to launch MakeBite?", "Installing a Mod", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                LaunchMakeBite();
            }
        }

        private void menuItemOpenMakeBite_Click(object sender, EventArgs e)
        {
            LaunchMakeBite();
        }

        private void LaunchMakeBite()
        {
            string makeBitePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "makebite.exe");

            try
            {
                Process.Start(makeBitePath);
            }
            catch
            {
                MessageBox.Show("MakeBite application could not be opened from " + makeBitePath, "Failed to launch MakeBite", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void menuItemHelpConflicts_Click(object sender, EventArgs e)
        {
            MessageBox.Show("", "Mod Conflicts", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItemOpenBugReport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("If you have found an issue with SnakeBite, please report the issue with as much information as you can gather! Be sure to include the relevant Debug Log in the bug report, and do your best to explain how you are able to reproduce the issue.\n\nAlso, always search through the existing bug reports to make sure that your issue hasn't already been created. If your bug is already reported, add your information to that bug report instead!", "Reporting a Bug", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(ModManager.SBWMBugURL);
        }

        private void menuItemWikiLink_Click(object sender, EventArgs e)
        {
            Process.Start(ModManager.WikiURL);
        }
    }
}
