using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using SnakeBite.ModPages;

namespace SnakeBite
{
    public partial class formSettings : Form
    {
        List<string> themeFiles = new List<string>() { "" };
        SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
        LogPage log = new LogPage();

        public formSettings()
        {
            InitializeComponent();
        }

        private void CheckBackupState()
        {
            if (BackupManager.OriginalsExist())
            {
                labelNoBackups.Text = "";
                buttonRestoreOriginals.Enabled = true;
            }
            else
            {
                if (BackupManager.OriginalZeroOneExist()) {
                    labelNoBackups.Text = "chunk0 backup not detected.\nCannot restore backup game files.";
                    buttonRestoreOriginals.Enabled = false;
                    picModToggle.Enabled = true;
                }
                else
                {
                    labelNoBackups.Text = "No backups detected.\nCertain features are unavailable.";
                    buttonRestoreOriginals.Enabled = false;
                    picModToggle.Enabled = false;
                    picModToggle.Image = Properties.Resources.toggledisabled;
                }
            }
        }

        private void buttonRestoreOriginals_Click(object sender, EventArgs e)
        {
            var restoreData = MessageBox.Show("Your saved backup files will be restored, and any SnakeBite settings and mods will be completely removed.\n\nAre you sure you want to continue?", "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (restoreData != DialogResult.Yes) return;

            BackupManager.RestoreOriginals();
            try
            {
                manager.DeleteSettings();
            }
            catch { }
            Application.Exit();
        }

        private void UpdateModToggle()
        {
            // Enable/disable mods button

            if (BackupManager.ModsDisabled())
            {
                picModToggle.Image = Properties.Resources.toggleoff;
                picModToggle.Enabled = true;
            }
            else
            {
                picModToggle.Image = Properties.Resources.toggleon;
                picModToggle.Enabled = true;
            }
        }

        private void buttonSetup(object sender, EventArgs e)
        {
            SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
            setupWizard.Tag = "closable";
            setupWizard.ShowDialog();
            UpdateModToggle();
            CheckBackupState();
        }

        private void linkNexusLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(labelNexusLink.Text);
        }

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

        private void formSettings_Load(object sender, EventArgs e)
        {
            // Set installation path textbox
            textInstallPath.Text = Properties.Settings.Default.InstallPath;
            checkEnableSound.Checked = Properties.Settings.Default.EnableSound;
            checkBoxSaveRevertPreset.Checked = Properties.Settings.Default.AutosaveRevertPreset;
            //listThemes.SelectedIndex = 0;
            UpdateModToggle();
            CheckBackupState();
            
            /*
            if (Directory.Exists("Themes"))
            {
                foreach(string file in Directory.GetFiles("Themes", "*.sbtheme"))
                {
                    // Read theme names
                    ZipFile themeZip = new ZipFile(file);
                    var themeEntry = themeZip.FindEntry("Theme.xml", true);
                    if(themeEntry >= 0)
                    {
                        var themeStream = themeZip.GetInputStream(themeZip[themeEntry]);
                        using (StreamReader themeReader = new StreamReader(themeStream))
                        {
                            XmlSerializer themeSerializer = new XmlSerializer(typeof(ThemeXml.Theme));
                            var theme = (ThemeXml.Theme)themeSerializer.Deserialize(themeReader);
                            listThemes.Items.Add(theme.Name);
                            themeFiles.Add(file);
                            if (Properties.Settings.Default.ThemeFile == file) listThemes.SelectedIndex = themeFiles.Count-1;
                        }
                    }
                }
            } else
            {
                tabControl.TabPages.RemoveAt(1);
            }
            */
        }

        private void checkEnableSound_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnableSound = checkEnableSound.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBoxSaveRevertPreset_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutosaveRevertPreset = checkBoxSaveRevertPreset.Checked;
            Properties.Settings.Default.Save();
        }

        /*
        private void buttonSetTheme_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ThemeFile = themeFiles[listThemes.SelectedIndex];
            Properties.Settings.Default.Save();

            var o = Owner as formLauncher;
            o.SetupTheme();
            o.Refresh();
        }
        */

        private void buttonOpenLog_Click(object sender, EventArgs e) {
            Process.Start(Debug.LOG_FILE_PREV);
            Process.Start(Debug.LOG_FILE);
        }

        private void picModToggle_Click(object sender, EventArgs e)
        {
            if (BackupManager.ModsDisabled())
            {
                ProgressWindow.Show("Working", "Enabling mods, please wait...", new Action(BackupManager.SwitchToMods), log);
            }
            else
            {
                ProgressWindow.Show("Working", "Disabling mods, please wait...", new Action(BackupManager.SwitchToOriginal), log);
            }
            UpdateModToggle();
        }
    }
}
