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
         * Greatest concerns: A lot of globals, resulting in ugly code. 
         * Excessive mods and conflicts will result in slower refresh times and list "flickering"
         */
        public formInstallOrder()
        {
            InitializeComponent();
            
        }

        private List<List<string>> ModConflictLists; // Holds unique lists of mod names which conflict with the mod at the correlating index.
        private List<string> ModConflicts;
        private List<ModEntry> ModInfo; // list of unique modentries which contains each mod's metadata
        private ModEntry selectedMod;
        private List<string> ModFiles; // list of unique mod filenames (ex: C:/Users/John/modname.mgsv/). This list is the "foundation stone" for formInstallOrder.
        private string filename;
        private int selectedIndex;

        public void ShowDialog(List<string> Filenames)
        {
            ModFiles = Filenames;
            this.refreshInstallList();
            this.ShowDialog();
        }

        private void refreshInstallList() // Populates install list with updated information, updates globals. Depends greatly on ModFiles list.
        {
            PreinstallManager.RefreshXml(ModFiles);
            listInstallOrder.Items.Clear();
            ModInfo = PreinstallManager.getModEntries();
            int modCount = ModInfo.Count;
            
            if (modCount > 0) // 1 or more mods to install. refresh install list and conflicts.
            {
                buttonRemove.Enabled = true;
                groupBoxNoModsNotice.Visible = false;
                panelInfo.Visible = true;

                foreach (ModEntry mod in ModInfo)
                {
                    listInstallOrder.Items.Add(mod.Name);
                }
                selectedIndex = modCount - 1;
                this.updateModConflicts();
                listInstallOrder.Items[selectedIndex].Selected = true;
                this.updateModDescription();
            }
            else // no mods in list, do nothing
            {
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
                selectedMod = ModInfo[selectedIndex]; filename = ModFiles[selectedIndex]; ModConflicts = ModConflictLists[selectedIndex];
                labelModName.Text = selectedMod.Name;
                labelModAuthor.Text = "By " + selectedMod.Author;
                labelModWebsite.Text = selectedMod.Version;
                textModDescription.Text = selectedMod.Description;
                
                string conflictDescription;
                    if (ModConflicts.Count > 0) { //builds string of conflicting mods to be displayed for the user
                    conflictDescription = string.Format("\r\nThis mod conflicts with: \r\n\r\n", selectedMod.Name);
                    foreach (string modname in ModConflictLists[selectedIndex])
                    {
                        
                        conflictDescription += modname + "\r\n";
                    }
                    
                } else
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
                if (ModConflicts.Contains(listInstallOrder.Items[i].Text))
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
            ModConflictLists = PreinstallManager.getConflictList(ModInfo);
            for (int i = 0; i < ModConflictLists.Count; i++)
            {
                if(ModConflictLists[i].Count > 0)
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
                listInstallOrder.Items[selectedIndex].Remove(); ModInfo.Remove(selectedMod); ModFiles.Remove(filename); ModConflictLists.Remove(ModConflicts);
                selectedIndex--;
                listInstallOrder.Items.Insert(selectedIndex, selectedMod.Name); ModInfo.Insert(selectedIndex, selectedMod); ModFiles.Insert(selectedIndex, filename); ModConflictLists.Insert(selectedIndex, ModConflicts);
                listInstallOrder.Items[selectedIndex].Selected = true;
            }
        }

        private void buttonDown_Click(object sender, EventArgs e) //moves the selected mod down one on the list. installs later.
        {
            if (selectedIndex < listInstallOrder.Items.Count - 1)
            {
                
                listInstallOrder.Items[selectedIndex].Remove(); ModInfo.Remove(selectedMod); ModFiles.Remove(filename); ModConflictLists.Remove(ModConflicts);
                selectedIndex++;
                listInstallOrder.Items.Insert(selectedIndex, selectedMod.Name); ModInfo.Insert(selectedIndex, selectedMod); ModFiles.Insert(selectedIndex, filename); ModConflictLists.Insert(selectedIndex, ModConflicts);
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
                if (!ModFiles.Contains(filename))
                    ModFiles.Add(filename);
            }
            this.refreshInstallList();
        }

        private void buttonRemove_Click(object sender, EventArgs e) // removes one filename from the list and refreshes list.
        {
            if (listInstallOrder.SelectedItems != null)
            {
                ModFiles.RemoveAt(listInstallOrder.SelectedIndices[0]);
            }
            this.refreshInstallList();
        }

        private void buttonContinue_Click(object sender, EventArgs e) // the listed mods are checked against installed mods/gamefiles for conflicts.
        {

            ModFiles = PreinstallManager.FilterModValidity(ModFiles);
            if (ModFiles.Count == 0) { this.refreshInstallList(); return; }//no valid mods. no mods will be installed
            ModFiles = PreinstallManager.FilterModConflicts(ModFiles);
            if (ModFiles.Count == 0) { this.refreshInstallList(); return; } //remaining mods had conflicts, user chose to install none.

            string modsToInstall = "";
            for (int i = 0; i < ModFiles.Count; i++)
            {
                modsToInstall += "\n" + Tools.ReadMetaData(ModFiles[i]).Name;
            }
            DialogResult confirmInstall = MessageBox.Show(String.Format("The following mods will be installed:\n" + modsToInstall), "SnakeBite", MessageBoxButtons.OKCancel);
            if (confirmInstall == DialogResult.OK)
            {
                ProgressWindow.Show("Installing Mod(s)", String.Format("Installing...\n\nNote:\nThe install time depends greatly\non the size and number of mods being installed,\nas well as the mods that are already installed."), new Action((MethodInvoker)delegate { ModManager.InstallMod(ModFiles); }));
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
                Process.Start(selectedMod.Website);
            }
            catch {}
        }

    }
}
