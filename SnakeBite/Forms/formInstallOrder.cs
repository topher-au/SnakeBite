using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SnakeBite.ModPages;

namespace SnakeBite.Forms
{
    public partial class formInstallOrder : Form
    {
        /*
         * formInstallOrder
         * Designed to preview and organize mods for installation. Largely inspired by the Nexus Mod Manager.
         * The user can add/remove multiple mods to the install list, preview their metadatas, check for conflicts (with other installing mods)
         * and organize the install order for conflict control.
         * 
         * Excessive mods and conflicts will result in slower refresh times and list "flickering"
         */
        public static Point formLocation = new Point(0, 0);
        public static Size formSize = new Size(0, 0);

        private List<PreinstallEntry> Mods = new List<PreinstallEntry>();
        private SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
        private int selectedIndex;

        private NoAddedPage noModsNotice = new NoAddedPage();
        private ModDescriptionPage modDescription = new ModDescriptionPage();
        private LogPage log = new LogPage();

        public formInstallOrder()
        {
            InitializeComponent();
            panelContent.Controls.Add(noModsNotice);
            panelContent.Controls.Add(modDescription);
            panelContent.Controls.Add(log);
        }

        public void ShowDialog(List<string> Filenames)
        {
            foreach (string file in Filenames)
            {
                PreinstallEntry mod = new PreinstallEntry();
                mod.filename = file;
                Mods.Add(mod);
            }
            ShowDialog();
        }

        private void formInstallOrder_Shown(object sender, EventArgs e)
        {
            SetVisiblePage(log);
            CheckAllModConflicts();
            refreshInstallList();
        }

        private void refreshInstallList()
        {
            listInstallOrder.Items.Clear();
            int modCount = Mods.Count;
            
            if (modCount > 0)
            {
                buttonContinue.Enabled = true;
                buttonRemove.Enabled = true;
                buttonUp.Enabled = true;
                buttonDown.Enabled = true;
                SetVisiblePage(modDescription);

                foreach (PreinstallEntry mod in Mods)
                {
                    listInstallOrder.Items.Add(mod.modInfo.Name);
                }

                selectedIndex = modCount - 1;
                listInstallOrder.Items[selectedIndex].Selected = true;
                updateModDescription();
            }
            else // no mods in list, do nothing
            {
                buttonContinue.Enabled = false;
                buttonRemove.Enabled = false;
                buttonUp.Enabled = false;
                buttonDown.Enabled = false;
                SetVisiblePage(noModsNotice);

            }
            TallyConflicts();
            labelModCount.Text = "Total Count: " + modCount;
        }
        
        private void updateModDescription() //refreshes description panel with current index's metadata.
        {
            if (selectedIndex >= 0)
            {
                PreinstallEntry selectedMod = Mods[selectedIndex];
                modDescription.ShowModInfo(selectedMod.modInfo);
                showConflictColors(); // updates the conflict visualization
            }
        }

        private void CheckAllModConflicts()
        {
            ProgressWindow.Show("Checking Preinstall Conflicts", "Processing mod data, please wait...", new Action((MethodInvoker)delegate {
                PreinstallManager.RefreshAllXml(Mods);
                PreinstallManager.getAllConflicts(Mods);
            }), log);
        }

        private void TallyConflicts()
        {
            int conflictCounter = 0;
            for (int i = 0; i < Mods.Count; i++)
            {
                if (Mods[i].ModConflicts.Count > 0)
                {
                    conflictCounter++;
                }
            }
            labelConflictCount.Text = string.Format("Conflicts Detected: {0}", conflictCounter);
        }

        private void RemoveConflict(string modName)
        {
            foreach (PreinstallEntry remainingEntry in Mods.FindAll(entry => entry.ModConflicts.Contains(modName)))
            {
                remainingEntry.ModConflicts.Remove(modName);
            }
        }

        private void showConflictColors() // Inspired by Nexus Mod Manager, a nice way of visualizing conflicts for the user.
        {
            int lowestIndex = 0;
            for (int i = 0; i < listInstallOrder.Items.Count; i++)
            {
                if (Mods[selectedIndex].ModConflicts.Contains(listInstallOrder.Items[i].Text))
                    if (i < selectedIndex)
                    {
                        listInstallOrder.Items[i].BackColor = Color.IndianRed;
                    }//if the conflicting mod installs before the selected mod, the contents are overwritten (visualized by a red backcolor)
                    else
                    {
                        listInstallOrder.Items[i].BackColor = Color.MediumSeaGreen;
                        lowestIndex = i;//the last index checked will always be lowest on the list.
                    }//if the conflicting mod installs after the selected mod, the selected mod is overwriten (visualized by a green backcolor)
                else
                    listInstallOrder.Items[i].BackColor = Color.Silver;
            }
            if (lowestIndex > selectedIndex) //check against the lowest index to determine the selected mod's color.
                listInstallOrder.Items[selectedIndex].BackColor = Color.IndianRed;
            else
                listInstallOrder.Items[selectedIndex].BackColor = Color.MediumSeaGreen;
        }

        private void AddNewPaths(string[] modFilePaths)
        {
            foreach (string filePath in modFilePaths)
            {
                bool skip = false;
                foreach (PreinstallEntry mod in Mods)
                {
                    if (filePath == mod.filename)
                    {
                        skip = true; break;
                    }
                }
                if (skip) continue;

                PreinstallEntry newEntry = new PreinstallEntry();
                newEntry.filename = filePath;
                PreinstallManager.AddModsToXml(newEntry);
                PreinstallManager.GetConflicts(newEntry, Mods);
                Mods.Add(newEntry);
            }
        }

        private void listInstallOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listInstallOrder.SelectedItems.Count == 1)
            {
                selectedIndex = listInstallOrder.SelectedIndices[0];
                updateModDescription();
            }
        }

        private void buttonUp_Click(object sender, EventArgs e) //moves the selected mod up one on the list. Installs earlier.
        {
            if (selectedIndex > 0)
            {
                PreinstallEntry mod = Mods[selectedIndex];
                listInstallOrder.Items[selectedIndex].Remove(); Mods.RemoveAt(selectedIndex);
                selectedIndex--;
                listInstallOrder.Items.Insert(selectedIndex, mod.modInfo.Name); Mods.Insert(selectedIndex, mod);
                listInstallOrder.Items[selectedIndex].Selected = true;
            }
        }

        private void buttonDown_Click(object sender, EventArgs e) //moves the selected mod down one on the list. installs later.
        {
            if (selectedIndex < listInstallOrder.Items.Count - 1)
            {
                PreinstallEntry mod = Mods[selectedIndex];
                listInstallOrder.Items[selectedIndex].Remove(); Mods.RemoveAt(selectedIndex);
                selectedIndex++;
                listInstallOrder.Items.Insert(selectedIndex, mod.modInfo.Name); Mods.Insert(selectedIndex, mod);
                listInstallOrder.Items[selectedIndex].Selected = true;
            }

        }

        private void buttonAdd_Click(object sender, EventArgs e) //adds unique filenames to the list and refreshes list.
        {

            OpenFileDialog openModFile = new OpenFileDialog();
            openModFile.Filter = "MGSV Mod Files|*.mgsv|All Files|*.*";
            openModFile.Multiselect = true;
            
            DialogResult ofdResult = openModFile.ShowDialog();
            if (ofdResult != DialogResult.OK) return;

            log.ClearPage();
            SetVisiblePage(log);
            ProgressWindow.Show("Checking Preinstall Data", "Processing mod data, please wait...", new Action((MethodInvoker)delegate { AddNewPaths(openModFile.FileNames); }), log);
            refreshInstallList();

        }

        private void buttonRemove_Click(object sender, EventArgs e) // removes one filename from the list and refreshes list.
        {
            string modName = Mods[selectedIndex].modInfo.Name;
            if (listInstallOrder.SelectedItems != null)
            {
                PreinstallManager.RemoveFromXml(Mods[selectedIndex]);
                Mods.RemoveAt(selectedIndex);
                RemoveConflict(modName);
                refreshInstallList();
            }
        }

        private void buttonContinue_Click(object sender, EventArgs e) // the listed mods are checked against installed mods/gamefiles for conflicts.
        {
            List<string> modFiles = new List<string>();
            foreach (PreinstallEntry entry in Mods)
            {
                modFiles.Add(entry.filename);
            }
            log.ClearPage();
            SetVisiblePage(log);
            ProgressWindow.Show("Checking Validity", "Checking mod validity...", new Action((MethodInvoker)delegate { PreinstallManager.FilterModValidity(modFiles); }), log);            
            if (modFiles.Count == 0) { refreshInstallList(); return; }//no valid mods. no mods will be installed

            formLocation = Location; // to center the conflict window
            formSize = Size;
            ProgressWindow.Show("Checking Conflicts", "Checking for conflicts with installed mods...", new Action((MethodInvoker)delegate { PreinstallManager.FilterModConflicts(modFiles); }), log);            
            if (modFiles.Count == 0) { refreshInstallList(); return; } //remaining mods had conflicts, user chose to install none.

            string modsToInstall = "";
            for (int i = 0; i < modFiles.Count; i++)
            {
                modsToInstall += "\n" + Tools.ReadMetaData(modFiles[i]).Name;
            }
            DialogResult confirmInstall = MessageBox.Show(String.Format("The following mods will be installed:\n" + modsToInstall), "SnakeBite", MessageBoxButtons.OKCancel);
            if (confirmInstall == DialogResult.OK)
            {
                ProgressWindow.Show("Installing Mod(s)", "Installing, please wait...", new Action((MethodInvoker)delegate { InstallManager.InstallMods(modFiles); }), log);
                Close(); // the form closes upon installation. If the install is cancelled, the form remains open.
            } else
            {
                refreshInstallList();
            }
        }
      
        private void labelExplainConflict_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A 'Mod Conflict' is when two or more mods attempt to modify the same game file. Whichever mod installs last in the Installation Order will overwrite any conflicting files of the mods above it. " +
       "In other words: The lower the mod, the higher the priority.\n\nThe user can adjust the Installation Order by using the arrow buttons. " +
       "Conflicts can also be resolved by removing mods from the list (removed mods will not be installed). \n\n" +
       "Warning: overwriting a mod's data may cause significant problems in-game, which could affect your enjoyment. Install at your own risk.", "What is a Mod Conflict?", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void formInstallOrder_FormClosed(object sender, FormClosedEventArgs e)
        {
            ModManager.CleanupFolders();
        }

        private void SetVisiblePage(UserControl visiblePage)
        {
            UserControl[] pages = { log, modDescription, noModsNotice };
            foreach (UserControl page in pages)
            {
                if (page == visiblePage)
                    page.Visible = true;
                else
                    page.Visible = false;
            }
        }
    }
}
