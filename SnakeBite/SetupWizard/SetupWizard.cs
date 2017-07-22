using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace SnakeBite.SetupWizard
{
    public partial class SetupWizard : Form
    {
        private IntroPage introPage = new IntroPage();
        private FindInstallPage findInstallPage = new FindInstallPage();
        private CreateBackupPage createBackupPage = new CreateBackupPage();
        private MergeDatPage mergeDatPage = new MergeDatPage();
        private int displayPage = 0;
        private SettingsManager manager = new SettingsManager(ModManager.GameDir);

        public SetupWizard()
        {
            InitializeComponent();
            this.FormClosing += formSetupWizard_Closing;
        }

        private void formSetupWizard_Load(object sender, EventArgs e)
        {
            buttonSkip.Visible = false;
            this.contentPanel.Controls.Add(introPage);
        }

        private void formSetupWizard_Closing(object sender, FormClosingEventArgs e)
        {
            if ((string)Tag == "noclose" && !(displayPage == 5))
                e.Cancel = true;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            switch (displayPage)
            {
                case -1:
                    buttonBack.Visible = false;
                    this.contentPanel.Controls.Clear();
                    this.contentPanel.Controls.Add(introPage);
                    displayPage = 0;
                    break;

                case 0:
                    // move to find installation
                    buttonBack.Visible = true;
                    buttonSkip.Visible = false;
                    this.contentPanel.Controls.Clear();
                    this.contentPanel.Controls.Add(findInstallPage);
                    displayPage = 1;
                    break;

                case 1:
                    if (!manager.ValidInstallPath)
                    {
                        MessageBox.Show("Please select a valid installation directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!BackupManager.GameFilesExist())
                    {
                        MessageBox.Show("Some game data appears to be missing. If you have just revalidated the game data, please wait for Steam to finish downloading the new files before continuing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // show create backup page, without processing panel, enable skip
                    createBackupPage.panelProcessing.Visible = false;
                    this.contentPanel.Controls.Clear();
                    this.contentPanel.Controls.Add(createBackupPage);
                    buttonSkip.Visible = true;

                    displayPage = 2;
                    break;

                case 2:
                    if (BackupManager.BackupExists())
                    {
                        var overWrite = MessageBox.Show("Some backup data already exists. Continuing will overwrite any existing backups.\n\nIf you have modified the game files seperately, you should restore them or skip this step.\n\nIf MGSV has just been updated, you should overwrite.\n\nAre you sure?", "SnakeBite", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (overWrite == DialogResult.No) return;
                    }

                    // create backup
                    buttonSkip.Visible = false;
                    buttonBack.Visible = false;
                    buttonNext.Enabled = false;
                    createBackupPage.panelProcessing.Visible = true;

                    // do backup processing
                    BackgroundWorker backupProcessor = new BackgroundWorker();
                    backupProcessor.DoWork += (obj, var) => BackupManager.CopyBackupFiles(true);
                    backupProcessor.RunWorkerAsync();

                    while (backupProcessor.IsBusy)
                    {
                        Application.DoEvents();
                        Thread.Sleep(10);
                    }

                    // move to merge dats
                    mergeDatPage.panelProcessing.Visible = false;
                    this.contentPanel.Controls.Clear();
                    this.contentPanel.Controls.Add(mergeDatPage);
                    buttonNext.Enabled = true;

                    displayPage = 3;
                    break;

                case 3:
                    // do cleanup processing
                    buttonNext.Enabled = false;
                    buttonBack.Visible = false;
                    Tag = "noclose";
                    mergeDatPage.panelProcessing.Visible = true;

                    BackgroundWorker mergeProcessor = new BackgroundWorker();
                    mergeProcessor.DoWork += (obj, var) => ModManager.MergeAndCleanup();
                    mergeProcessor.RunWorkerAsync();

                    while (mergeProcessor.IsBusy)
                    {
                        Application.DoEvents();
                        Thread.Sleep(10);
                    }

                    manager.UpdateDatHash();

                    mergeDatPage.panelProcessing.Visible = false;
                    mergeDatPage.labelWelcome.Text = "Setup complete";
                    mergeDatPage.labelWelcomeText.Text = "SnakeBite is configured and ready to use.";

                    buttonBack.Visible = false;
                    buttonNext.Text = "Do&ne";
                    buttonNext.Enabled = true;

                    displayPage = 4;
                    break;

                case 4:
                    displayPage = 5;
                    DialogResult = DialogResult.OK;
                    Close();
                    break;
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            displayPage -= 2;
            buttonNext_Click(null, null);
        }

        private void buttonSkip_Click(object sender, EventArgs e)
        {
            buttonSkip.Visible = false;
            mergeDatPage.panelProcessing.Visible = false;
            this.contentPanel.Controls.Clear();
            this.contentPanel.Controls.Add(mergeDatPage);
            buttonNext.Enabled = true;

            displayPage = 3;
        }
    }
}