using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Xml.Serialization;

namespace SnakeBite
{
    public partial class formSettings : Form
    {
        List<string> themeFiles = new List<string>() { "" };
        SettingsManager manager = new SettingsManager(ModManager.GameDir);
        public formSettings()
        {
            InitializeComponent();
        }

        private void CheckBackupState()
        {
            bool backupExists = BackupManager.OriginalsExist();
            {
                buttonRestoreOriginals.Enabled = backupExists;
            }
        }

        private void buttonRestoreOriginals_Click(object sender, EventArgs e)
        {
            var restoreData = MessageBox.Show("Your original backup files will be restored, and any SnakeBite settings and mods will be completely removed.\n\nAre you sure you want to continue?", "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (restoreData == DialogResult.No) return;

            if (BackupManager.OriginalsExist())
            {
                BackupManager.RestoreOriginals();
                manager.DeleteSettings();
                Application.Exit();
            }
        }

        private void buttonToggleMods_Click(object sender, EventArgs e)
        {
        }

        private void buttonSetup(object sender, EventArgs e)
        {
            SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
            setupWizard.Tag = "closable";
            setupWizard.ShowDialog();
            CheckBackupState();
        }


        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.labelGithub.Text);
        }

        /*
        private void checkConflicts_CheckedChanged(object sender, EventArgs e)
        {
            if (checkConflicts.Checked && manager.DisableConflictCheck == false)
            {
                MessageBox.Show("Enabling this option completely disables any warnings when installing mods, and may overwrite existing mod or game data.\n\n"+
                                "This may cause issues with some mods - or cause the game to hang, crash or worse - and it is recommended that you make a seperate backup before continuing.\n\n"+
                                "This option will only persist until you exit SnakeBite.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            manager.DisableConflictCheck = checkConflicts.Checked;
        }
        */

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
            //checkConflicts.Checked = manager.DisableConflictCheck;
            checkEnableSound.Checked = Properties.Settings.Default.EnableSound;
            listThemes.SelectedIndex = 0;

            if(Directory.Exists("Themes"))
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
        }

        private void checkEnableSound_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnableSound = checkEnableSound.Checked;
            Properties.Settings.Default.Save();
        }

        private void buttonSetTheme_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ThemeFile = themeFiles[listThemes.SelectedIndex];
            Properties.Settings.Default.Save();

            var o = this.Owner as formLauncher;
            o.SetupTheme();
            o.Refresh();
        }

        private void buttonOpenLog_Click(object sender, EventArgs e) {
            Process.Start(Debug.LOG_FILE_PREV);
            Process.Start(Debug.LOG_FILE);
        }
    }
}
