using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private List<PreinstallEntry> Mods = new List<PreinstallEntry>();
        private int selectedIndex;

        public formInstallOrder()
        {
            InitializeComponent();
            
        }

        public void ShowDialog(List<string> Filenames)
        {
            foreach (string file in Filenames)
            {
                PreinstallEntry mod = new PreinstallEntry();
                mod.filename = file;
                Mods.Add(mod);
            }
            this.refreshInstallList();
            this.ShowDialog();
        }

        private void refreshInstallList() // Populates install list with updated information, updates globals. Depends greatly on ModFiles list.
        {
            PreinstallManager.RefreshXml(Mods);
            listInstallOrder.Items.Clear();
            int modCount = Mods.Count;
            
            if (modCount > 0) // 1 or more mods to install. refresh install list and conflicts.
            {
                buttonContinue.Enabled = true;
                buttonRemove.Enabled = true;
                groupBoxNoModsNotice.Visible = false;
                panelInfo.Visible = true;
                foreach (PreinstallEntry mod in Mods)
                {
                    listInstallOrder.Items.Add(mod.modInfo.Name);
                }
                selectedIndex = modCount - 1;
                this.updateModConflicts();
                listInstallOrder.Items[selectedIndex].Selected = true;
                this.updateModDescription();
            }
            else // no mods in list, do nothing
            {
                buttonContinue.Enabled = false;
                buttonRemove.Enabled = false;
                groupBoxNoModsNotice.Visible = true;
                panelInfo.Visible = false;
            }

            labelModCount.Text = "Total Count: " + modCount;
        }

        private void listInstallOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listInstallOrder.SelectedItems.Count == 1)
            {
                selectedIndex = listInstallOrder.SelectedIndices[0];
                this.updateModDescription();
            }
        }

        private void updateModDescription() //refreshes description panel with current index's metadata.
        {
            if (selectedIndex >= 0)
            {
                PreinstallEntry selectedMod = Mods[selectedIndex];
                labelModName.Text = selectedMod.modInfo.Name;
                labelModAuthor.Text = "By " + selectedMod.modInfo.Author;
                labelModWebsite.Text = selectedMod.modInfo.Version;
                textModDescription.Text = selectedMod.modInfo.Description;

                if (ModManager.GetMGSVersion() != selectedMod.modInfo.MGSVersion.AsVersion() && selectedMod.modInfo.MGSVersion.AsVersion() != new Version(0, 0, 0, 0))
                {
                    labelVersionWarning.ForeColor = Color.Yellow; labelVersionWarning.BackColor = Color.Chocolate; labelVersionWarning.Text = "!";
                }
                else
                {
                    labelVersionWarning.ForeColor = Color.MediumSeaGreen; labelVersionWarning.BackColor = Color.Gainsboro; labelVersionWarning.Text = "✔";
                }


                string conflictDescription;
                if (Mods[selectedIndex].ModConflicts.Count > 0)
                {
                    conflictDescription = "\r\nThis mod conflicts with: \r\n\r\n";
                    foreach (string modname in selectedMod.ModConflicts)
                    {
                        conflictDescription += modname + "\r\n";
                    }
                }
                else
                {
                    conflictDescription = "\r\nThis mod does not conflict with any other mods being installed.";
                }
                textConflictDescription.Text = conflictDescription;
                showConflictColors(); // updates the conflict visualization
            }
        }

        private void showConflictColors() // Inspired by Nexus Mod Manager, a nice way of visualizing conflicts for the user.
        {
            int lowestIndex = 0;
            for (int i = 0; i < listInstallOrder.Items.Count; i++)
            {
                if (Mods[selectedIndex].ModConflicts.Contains(listInstallOrder.Items[i].Text))
                    if (i < selectedIndex) {
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

        private void updateModConflicts() // Very computation-heavy, used sparingly. Checks current install list for conflicts.
        {
            int conflictCounter = 0;
            PreinstallManager.getConflictList(Mods);
            
            for (int i = 0; i < Mods.Count; i++)
            {
                if(Mods[i].ModConflicts.Count > 0)
                {
                    conflictCounter++;
                }
            }
            if (conflictCounter == 0)
                labelConflictCount.Text = "0 Conflicts Detected";
            else
                labelConflictCount.Text = conflictCounter.ToString() + " Mods With Conflicts";
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
            foreach (string filename in openModFile.FileNames)
            {
                bool skip = false;
                foreach (PreinstallEntry mod in Mods)
                {
                    if (filename == mod.filename)
                    {
                        skip = true; break;
                    }
                }
                if (skip) continue;

                PreinstallEntry newEntry = new PreinstallEntry();
                newEntry.filename = filename;
                Mods.Add(newEntry);
            }
            this.refreshInstallList();
        }

        private void buttonRemove_Click(object sender, EventArgs e) // removes one filename from the list and refreshes list.
        {
            if (listInstallOrder.SelectedItems != null)
            {
                Mods.RemoveAt(selectedIndex);
            }
            this.refreshInstallList();
        }

        private void buttonContinue_Click(object sender, EventArgs e) // the listed mods are checked against installed mods/gamefiles for conflicts.
        {
            List<string> modFiles = new List<string>();
            foreach (PreinstallEntry entry in Mods)
            {
                modFiles.Add(entry.filename);
            }
            PreinstallManager.FilterModValidity(modFiles);
            if (modFiles.Count == 0) { this.refreshInstallList(); return; }//no valid mods. no mods will be installed
            PreinstallManager.FilterModConflicts(modFiles);
            if (modFiles.Count == 0) { this.refreshInstallList(); return; } //remaining mods had conflicts, user chose to install none.

            string modsToInstall = "";
            for (int i = 0; i < modFiles.Count; i++)
            {
                modsToInstall += "\n" + Tools.ReadMetaData(modFiles[i]).Name;
            }
            DialogResult confirmInstall = MessageBox.Show(String.Format("The following mods will be installed:\n" + modsToInstall), "SnakeBite", MessageBoxButtons.OKCancel);
            if (confirmInstall == DialogResult.OK)
            {
                string progressText = String.Format("Installing...\n\nNote:\nThe install time depends greatly on\nthe mod's contents, number of mods being installed\nand the mods that are already installed.");
                ProgressWindow.Show("Installing Mod(s)", progressText, new Action((MethodInvoker)delegate { ModManager.InstallMod(modFiles); }));
                this.Close(); // the form closes upon installation. If the install is cancelled, the form remains open.
            }
        }
      
        private void labelInstallWarning_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A conflict occurs when two or more mods attempt to modify the same game file. Whichever mod installs last on the list will overwrite any conflicting game files of the mods above it. " +
       "In other words: The lower the mod, the higher the priority.\n\nThe user can adjust the installation order by using the arrow buttons. " +
       "Conflicts can also be resolved by removing mods from the list (removed mods will not be installed). \n\n" +
       "Warning: overwriting a mod's data may cause significant problems in-game, which could affect your enjoyment. Install at your own risk.", "Resolving Mod Conflicts", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void labelModWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try //if the mod author didn't include a website, an exception occurs.
            {
                Process.Start(Mods[selectedIndex].modInfo.Website);
            }
            catch {}
        }

        private void labelVersionWarning_Click(object sender, EventArgs e)
        {
            PreinstallEntry selectedMod = Mods[selectedIndex];
            var currentMGSVersion = ModManager.GetMGSVersion();
            var modMGSVersion = selectedMod.modInfo.MGSVersion.AsVersion();
            if (currentMGSVersion != modMGSVersion && modMGSVersion != new Version(0, 0, 0, 0))
            {
                if (currentMGSVersion > modMGSVersion && modMGSVersion > new Version(0, 0, 0, 0))
                {
                    MessageBox.Show(String.Format("{0} appears to be for MGSV Version {1}.\n\nIt is recommended that you at least check for an updated version before installing.", selectedMod.modInfo.Name, modMGSVersion), "Game version mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (currentMGSVersion < modMGSVersion)
                {
                    MessageBox.Show(String.Format("{0} is intended for MGSV version {1}, but your installation is version {2}. This mod may not be compatible with MGSV version {2}", selectedMod.modInfo.Name, modMGSVersion, currentMGSVersion), "Update recommended", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            else
            {
                MessageBox.Show(String.Format("This mod is up to date with the current MGSV version {0}", currentMGSVersion), "Mod is up to date", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
    }
}
