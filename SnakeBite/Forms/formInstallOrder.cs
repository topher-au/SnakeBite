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
        public formInstallOrder()
        {
            InitializeComponent();
            
        }

        List<List<string>> ModConflictLists;
        List<string> ModConflicts;
        List<ModEntry> ModInfo;
        ModEntry selectedMod;
        List<string> ModFiles;
        string filename;        //add string historicFilename to ModEntry? It would cut down on globals and clean up some code.
        int selectedIndex;

        public void ShowDialog(List<string> Filenames)
        {
            ModFiles = Filenames;
            this.refreshInstallList();
            this.ShowDialog();
        }

        private void refreshInstallList()
        {
            PreinstallManager.RefreshXml(ModFiles);
            listInstallOrder.Items.Clear();
            ModInfo = PreinstallManager.getModEntries();
            int modCount = ModInfo.Count;
            
            if (modCount > 0)
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
            else
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

        private void updateModDescription()
        {
            if (selectedIndex >= 0)
            {
                selectedMod = ModInfo[selectedIndex]; filename = ModFiles[selectedIndex]; ModConflicts = ModConflictLists[selectedIndex];
                labelModName.Text = selectedMod.Name;
                labelModAuthor.Text = "By " + selectedMod.Author;
                labelModWebsite.Text = selectedMod.Version;
                textModDescription.Text = selectedMod.Description;
                
                string conflictDescription;
                    if (ModConflicts.Count > 0) {
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
                showConflictColors();
            }
        }

        private void showConflictColors()
        {
            int lowestIndex = 0;
            for (int i = 0; i < listInstallOrder.Items.Count; i++)
            {
                if (ModConflicts.Contains(listInstallOrder.Items[i].Text))
                    if (i < selectedIndex) {
                        listInstallOrder.Items[i].BackColor = Color.IndianRed;
                    }
                    else
                    {
                        listInstallOrder.Items[i].BackColor = Color.MediumSeaGreen;
                        lowestIndex = i;
                    }
                else
                    listInstallOrder.Items[i].BackColor = Color.Silver;
            }
            if (lowestIndex > selectedIndex)
                listInstallOrder.Items[selectedIndex].BackColor = Color.IndianRed;
            else
                listInstallOrder.Items[selectedIndex].BackColor = Color.MediumSeaGreen;
        }

        private void updateModConflicts()
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

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (selectedIndex > 0)
            {
                listInstallOrder.Items[selectedIndex].Remove(); ModInfo.Remove(selectedMod); ModFiles.Remove(filename); ModConflictLists.Remove(ModConflicts);
                selectedIndex--;
                listInstallOrder.Items.Insert(selectedIndex, selectedMod.Name); ModInfo.Insert(selectedIndex, selectedMod); ModFiles.Insert(selectedIndex, filename); ModConflictLists.Insert(selectedIndex, ModConflicts);
                listInstallOrder.Items[selectedIndex].Selected = true;
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (selectedIndex < listInstallOrder.Items.Count - 1)
            {
                
                listInstallOrder.Items[selectedIndex].Remove(); ModInfo.Remove(selectedMod); ModFiles.Remove(filename); ModConflictLists.Remove(ModConflicts);
                selectedIndex++;
                listInstallOrder.Items.Insert(selectedIndex, selectedMod.Name); ModInfo.Insert(selectedIndex, selectedMod); ModFiles.Insert(selectedIndex, filename); ModConflictLists.Insert(selectedIndex, ModConflicts);
                listInstallOrder.Items[selectedIndex].Selected = true;
            }

        }

        private void buttonAdd_Click(object sender, EventArgs e)
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

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listInstallOrder.SelectedItems != null)
            {
                ModFiles.RemoveAt(listInstallOrder.SelectedIndices[0]);
            }
            this.refreshInstallList();
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {

            ModFiles = ModManager.FilterModValidity(ModFiles);
            if (ModFiles.Count == 0) { this.refreshInstallList(); return; }//no valid mods. no mods will be installed }
            ModFiles = ModManager.FilterModConflicts(ModFiles);
            if (ModFiles.Count == 0) { this.refreshInstallList(); return; } //remaining mods had conflicts, user chose to install none.

            string modsToInstall = "";
            for (int i = 0; i < ModFiles.Count; i++)
            {
                modsToInstall += "\n" + Tools.ReadMetaData(ModFiles[i]).Name;
            }
            DialogResult confirmInstall = MessageBox.Show(String.Format("The following mods will be installed:\n" + modsToInstall), "SnakeBite", MessageBoxButtons.OKCancel);
            if (confirmInstall == DialogResult.OK)
            {
                ProgressWindow.Show("Installing Mod(s)", String.Format("Installing...\n\nNote: The install time depends greatly\non the size and number of mods being installed,\nas well as the mods that are already installed."), new Action((MethodInvoker)delegate { ModManager.InstallMod(ModFiles); }));
                this.Close();
            }
        }
      
        private void labelInstallWarning_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A conflict occurs when two or more mods attempt to modify the same game file. Whichever mod installs last on the list will overwrite any conflicting game files of the mods above it. " +
       "In other words: The lower the mod, the higher the priority.\n\nThe user can choose the installation order by dragging the mod name or using the arrow buttons. " +
       "Conflicts can also be resolved by removing mods from the list (removed mods will not be installed). \n\n" +
       "Warning: overwriting a mod's data may cause significant problems in-game, which could affect your enjoyment. Install at your own risk.", "Resolving Mod Conflicts", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void labelModWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(selectedMod.Website);
            }
            catch {}
        }

    }
}
