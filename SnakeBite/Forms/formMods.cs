using SnakeBite.Forms;
using SnakeBite.ModPages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SnakeBite
{
    public partial class formMods : Form
    {
        private formProgress progWindow = new formProgress();
        private int countCheckedMods = 0;
        private SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);

        private ModDescriptionPage modDescription = new ModDescriptionPage();
        private NoInstalledPage noInstallNotice = new NoInstalledPage();
        private LogPage log = new LogPage();

        public formMods()
        {
            InitializeComponent();
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            // Refresh button state
            panelContent.Controls.Add(noInstallNotice);
            panelContent.Controls.Add(log);
            panelContent.Controls.Add(modDescription);
            RefreshInstalledMods(true);

            Location = Properties.Settings.Default.formModsLocation;
            Size = Properties.Settings.Default.formModsSize;

            if (Properties.Settings.Default.formModsMaximized == true)
                WindowState = FormWindowState.Maximized;

            menuItemSkipLauncher.Checked = Properties.Settings.Default.SkipLauncher;
            SetModsEnabled(!BackupManager.ModsDisabled());
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
            log.ClearPage();
            SetVisiblePage(log);
            ProgressWindow.Show("Uninstalling Mod(s)", "Uninstalling, please wait...", new Action((MethodInvoker)delegate { UninstallManager.UninstallMods(checkedModIndices); }), log);

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
            }
            else
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
            }
            else
            {
                if (Directory.Exists(installModPath))
                {
                    InstallFileList = Directory.GetFiles(installModPath, "*.mgsv").ToList();
                    if (InstallFileList.Count == 0)
                    {
                        Debug.LogLine($"[Install] Could not find any .mgsv files in {installModPath}.", Debug.LogLevel.Basic);
                        return;
                    }
                }
                else
                {
                    Debug.LogLine($"[Install] Could not find file or directory {installModPath}.", Debug.LogLevel.Basic);
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
                new Action((MethodInvoker)delegate { InstallManager.InstallMods(InstallFileList, skipCleanup); }
            ), log);
            this.Invoke((MethodInvoker)delegate { RefreshInstalledMods(); });
        }

        public void ProcessUninstallMod(ModEntry mod, bool skipcleanup)// command-line uninstall. This checks the mod it was passed, and puts it in a 1-item list to be uninstalled.
        {
            for (int i = 0; i < listInstalledMods.Items.Count; i++)
            {
                listInstalledMods.SetItemCheckState(i, CheckState.Unchecked);
            }
            var mods = manager.GetInstalledMods();
            listInstalledMods.SetItemCheckState(mods.IndexOf(mod), CheckState.Checked);
            CheckedListBox.CheckedIndexCollection checkedModIndex = listInstalledMods.CheckedIndices;
            ProgressWindow.Show("Uninstalling Mod", "Uninstalling...", new Action((MethodInvoker)delegate { UninstallManager.UninstallMods(checkedModIndex, skipcleanup); }), log);
        }

        private void RefreshInstalledMods(bool resetSelection = false) // Clears and then repopulates the installed mod list
        {
            var mods = manager.GetInstalledMods();
            listInstalledMods.Items.Clear();
            countCheckedMods = 0;
            buttonUninstall.Enabled = false;

            if (mods.Count > 0)
            {
                SetVisiblePage(modDescription);

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
                SetVisiblePage(noInstallNotice);

            }

            AdjustSize();
        }

        private void buttonLaunchGame_Click(object sender, EventArgs e)
        {
            {
                SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
                if (manager.ValidInstallPath)
                {
                    Debug.LogLine("Launching game...", Debug.LogLevel.Basic);
                    try
                    {
                        Process.Start("steam://run/287700/");
                        Application.Exit();
                    }
                    catch
                    {
                        try
                        {
                            Debug.LogLine("Failed to launch game through Steam (a bit suspicious tbh). Attempting to run mgsvtpp.exe...", Debug.LogLevel.Basic);
                            Process.Start(GamePaths.GameDir + "\\mgsvtpp.exe");
                            Application.Exit();
                        }
                        catch
                        {
                            Debug.LogLine("Failed to run mgsvtpp.exe", Debug.LogLevel.Basic);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Unable to locate MGSVTPP.exe. Please check the Settings and try again.", "Error launching MGSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void menuItemSavePreset_Click(object sender, EventArgs e)
        {
            OneTimePresetHelp();
            SavePreset();
        }

        private void SavePreset()
        {
            SaveFileDialog savePreset = new SaveFileDialog();
            savePreset.Filter = "MGSV Preset File|*.MGSVPreset";
            DialogResult saveResult = savePreset.ShowDialog();
            if (saveResult != DialogResult.OK) return;

            string presetPath = savePreset.FileName;
            log.ClearPage();
            SetVisiblePage(log);
            bool success = false;
            ProgressWindow.Show("Saving Preset", "Saving Preset, please wait...", new Action((MethodInvoker)delegate { success = PresetManager.SavePreset(presetPath); }), log);
            if (success) MessageBox.Show(string.Format("'{0}' Saved.", Path.GetFileName(presetPath)), "Preset Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshInstalledMods(true);
        }

        private void menuItemLoadPreset_Click(object sender, EventArgs e)
        {
            OneTimePresetHelp();
            /* This might be useful but I'm worried that the user might mistake the prompt and end up overwriting the preset that they wanted to load
            DialogResult saveModsResult = MessageBox.Show("Would you like to save your current mods as a Preset before loading a new Preset?", "Save current mods?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (saveModsResult == DialogResult.Yes) SavePreset();
            else if (saveModsResult == DialogResult.Cancel) return;
            */
            OpenFileDialog getPresetFile = new OpenFileDialog();
            getPresetFile.Filter = "MGSV Preset File|*.MGSVPreset|All Files|*.*";
            getPresetFile.Multiselect = true;

            DialogResult getPresetResult = getPresetFile.ShowDialog();
            if (getPresetResult != DialogResult.OK) return;
            string presetPath = getPresetFile.FileName;
            Settings presetSettings = PresetManager.ReadSnakeBiteSettings(presetPath);
            
            bool success = false;
            try
            {
                if (!PresetManager.isPresetUpToDate(presetSettings))
                {
                    if (MessageBox.Show(string.Format("This Preset file is intended for Game Version {0}, but your current Game Version is {1}. Loading this preset will likely cause crashes, infinite loading screens or other significant problems in-game.", presetSettings.MGSVersion.AsVersion(), ModManager.GetMGSVersion()) +
                         "\n\nAre you sure you want to load this preset?", "Preset Version Mismatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                }

                string modsToInstall = "This Preset will contain the following mods:\n";
                if (presetSettings.ModEntries.Count != 0)
                {
                    foreach (var mod in presetSettings.ModEntries)
                    {
                        modsToInstall += string.Format("\n{0}", mod.Name);
                    }
                }
                else
                {
                    modsToInstall += "\n[NONE]";
                }
                if (MessageBox.Show(modsToInstall, "Install Preset", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
                log.ClearPage();
                SetVisiblePage(log);
                ProgressWindow.Show("Loading Preset", "Loading Preset, please wait...", new Action((MethodInvoker)delegate { success = PresetManager.LoadPreset(presetPath); }), log);
            }
            catch (Exception f)
            {
                MessageBox.Show("An error has occurred and the .MGSVPreset could not be loaded. Maybe the Preset file was packed improperly, or is being used by another program?\nException: " + f);
                success = false;
            }
            RefreshInstalledMods(true);
            if (success) MessageBox.Show(string.Format("'{0}' Loaded", Path.GetFileName(presetPath)), "Preset Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                Debug.LogLine(String.Format("Failed to open game directory: {0}", installPath), Debug.LogLevel.Basic);
            }
        }

        private void menuItemOpenLogs_Click(object sender, EventArgs e)
        {
            Process.Start(Debug.LOG_FILE_PREV);
            Process.Start(Debug.LOG_FILE);
        }

        private void menuItemBrowseMods_Click(object sender, EventArgs e)
        {
            Process.Start(GamePaths.SBWMSearchURLPath);
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

            bool modsEnabled = !BackupManager.ModsDisabled();
            SetModsEnabled(modsEnabled);
        }

        private void SetModsEnabled(bool enabled)
        {
            labelInstalledMods.Text = "Installed Mods";
            if (!enabled)
            {
                for (int i = 0; i < listInstalledMods.Items.Count; i++)
                {
                    listInstalledMods.SetItemCheckState(i, CheckState.Unchecked);
                }

                labelInstalledMods.Text += " [Disabled]";
            }
            buttonInstall.Enabled = enabled;
            listInstalledMods.Enabled = enabled;
            checkBoxMarkAll.Enabled = enabled;
            labelInstalledMods.Enabled = enabled;
            menuItemLoadPreset.Enabled = enabled;
            menuItemSavePreset.Enabled = enabled;
        }

        private void menuItemOpenMakeBite_Click(object sender, EventArgs e)
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

        private void menuItemWikiLink_Click(object sender, EventArgs e)
        {
            Process.Start(GamePaths.WikiURLPath);
        }

        private void menuItemHelpInstall_Click(object sender, EventArgs e)
        {
            MessageBox.Show("SnakeBite can install mods that have the '.MGSV' file extensions. After downloading a .MGSV file, the user can install it with SnakeBite by clicking the \"Install .MGSV File(s)\" button in the bottom-left corner of the menu." +
                "\n\nMultiple mods can be selected at once when choosing what to install. Upon selecting a file, SnakeBite will open the Installation Manager submenu, where the mod will be listed and previewable. Additionally, mods can be added, removed and sorted in this menu." +
                "\n\nWhen the user is ready to install the selected mods, they can click the \"Continue Installation\" button in the bottom-right corner of the submenu. The install time depends greatly on the mod's contents, number of mods being installed and the mods that are already installed." +
                "\n\nThe installation is complete when the user is returned to the main menu. All mods installed with SnakeBite will now be listed on the menu, and will appear in-game. It is not necessary to launch the game using SnakeBite in order to use the installed mods.", "Installing a Mod", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void menuItemHelpUninstall_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To uninstall a mod, the user must simply click on the checkbox beside the mod's name, and then click the \"Uninstall Checked Mod(s)\" button in the bottom-left corner of the menu." +
                "\n\nMultiple mods can be uninstalled at once by clicking on their checkboxes. In addition, the user can mark all mods by clicking on the checkbox in the top-left corner of the menu, beside the \"Installed Mods\" text." +
                "\n\nThe uninstall time depends greatly on the number of mods being uninstalled, their contents, and the mods that remain installed.", "Uninstalling a Mod", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void menuItemHelpCreate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("To create a mod for SnakeBite, the user must build a .MGSV file using MakeBite (which was installed automatically alongside SnakeBite). MakeBite can create mods by packing all of the files from a specified folder into a new .MGSV file. " +
                "\n\nIn fact, .MGSV files are .zip files with a different file extension.\n\nThere are a number of tools and tutorials available for users to learn how to modify and prepare game files for MakeBite.\nWould you like to visit the MakeBite Wiki page for more information?", "Creating a Mod", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start(GamePaths.WikiURLPath + "SnakeBite#MakeBite");
            }
        }

        private void menuItemHelpConflicts_Click(object sender, EventArgs e) // TODO: move all of these messageboxes to a static 'Tips' class? 
        {
            MessageBox.Show("A 'Mod Conflict' is when two or more mods compete to modify the same game file. Whichever mod which was installed last will overwrite any conflicting files of the mods above it. " +
                "\n\nIf the mods have already been installed, the user can only adjust the overwrite order by uninstalling all of the conflicting mods and then reinstalling them in the preferred order. " +
                "\n\nFurthermore, uninstalling only one of the mods will NOT fix a conflict! When a mod is uninstalled, SnakeBite will remove all files that were included in that mod. This creates a 'hole' for the rest of the conflicting mods that were competing for the game file. This hole is not filled by the remaining mods, regardless of their order in the mod list." +
                "\n\nWarning: overwriting a mod's data may cause significant problems in-game, which could affect your enjoyment. Install at your own risk.", "What is a Mod Conflict?", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void menuItemOpenBugReport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("If you have found an issue with SnakeBite, please report the bug with as much information as you can gather! Be sure to include the relevant Debug Log in the bug report, and do your best to explain how you are able to reproduce the issue." +
                "\n\nAlso, always search through the existing bug reports to make sure that your issue hasn't already been created. If your bug is already reported, add your information to that bug report instead!", "Reporting a Bug", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(GamePaths.SBWMBugURLPath);
        }

        private void menuItemHelpPresets_Click(object sender, EventArgs e)
        {
            ShowPresetHelp();
        }

        private void ShowPresetHelp()
        {
            MessageBox.Show("A 'Mod Preset' is a collection of mods which can be saved and loaded with SnakeBite. Saving a Preset will pack your current modded game data into a .MGSVPreset file. Loading a Preset will simply replace your game data with the files stored in the .MGSVPreset file." +
                "\n\nPresets are a fast and simple method of organizing your favorite mods or trying new mod combinations." +
                "\n\nYou can also utilize Presets as restore checkpoints if SnakeBite encounters a serious error or your game data becomes corrupted. By default, SnakeBite creates 'RevertChanges.MGSVPreset' before a mod installation/uninstallation, so you can easily undo an action if it caused a critical error." +
                "\n\nHowever, saving a large number of mods needs a bit of time and storage space, so you can choose to skip this option by unchecking the 'Save RevertChanges.MGSVPreset' checkbox in the Settings menu.\nRevertChanges.MGSVPreset is saved to your Game Directory.", "Mod Preset Files", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void OneTimePresetHelp()
        {
            if (Properties.Settings.Default.showOneTimePresetHelp)
            {
                ShowPresetHelp();
            }
            Properties.Settings.Default.showOneTimePresetHelp = false;
            Properties.Settings.Default.Save();
        }

        private void formMods_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                AdjustSize();
            }
        }

        private void AdjustSize()
        {
            int modListWidth = Width / 3;

            panelModList.Width = modListWidth;

            panelContent.Left = panelModList.Width + 6;
            panelContent.Width = Width - panelModList.Width - 28;

            log.Size = panelContent.Size;
            modDescription.Size = panelContent.Size;
            noInstallNotice.Size = panelContent.Size;
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

        // The ModDescriptionPage would become slightly distorted over time using panelContent.Controls to clear or add a new page
        // I'm not sure why it does this exactly (something to do with the form resizing?), but setting page visibility is more reliable
        private void SetVisiblePage(UserControl visiblePage)
        {
            UserControl[] pages = { log, modDescription, noInstallNotice};
            foreach(UserControl page in pages)
            {
                if (page == visiblePage)
                    page.Visible = true;
                else
                    page.Visible = false;
            }
        }
    }
}
