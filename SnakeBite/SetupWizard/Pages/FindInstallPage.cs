using System.ComponentModel;
using System;
using System.IO;
using System.Windows.Forms;

namespace SnakeBite.SetupWizard
{
    public partial class FindInstallPage : UserControl
    {
        public FindInstallPage()
        {
            InitializeComponent();
            textInstallPath.Text = Properties.Settings.Default.InstallPath;
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            //var doValidate = MessageBox.Show("SnakeBite will close the Steam validation window automatically when ready, please do not cancel or close the Steam window.", "SnakeBite", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            var doValidate = MessageBox.Show("Please wait until the Steam validation window says it's complete.", "SnakeBite", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (doValidate == DialogResult.Cancel) return;
            System.Diagnostics.Process.Start("steam://validate/287700/");
            //tex times out too early, just waiting for a period isn't robust.
            /*
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (obj, var) =>
            {
                int sleep = 0;
                int maxSleep = 7500;
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                    sleep += 100;

                    if(!BackupManager.GameFilesExist()) // break when files are removed by Steam
                    {
                        try
                        {
                            Microsoft.VisualBasic.Interaction.AppActivate("Validating Steam files");
                            SendKeys.SendWait("%{F4}");
                        }
                        catch
                        {
                            MessageBox.Show("Unable to locate and close the Steam window, you may need to launch Steam before trying again, or validate manually through the Steam application.", "Steam Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }

                    if (sleep >= maxSleep) // break on timeout
                    {
                        try
                        {
                            Microsoft.VisualBasic.Interaction.AppActivate("Validating Steam files");
                            SendKeys.SendWait("%{F4}");
                        }
                        catch
                        {
                            MessageBox.Show("Timed out waiting for Steam window. Please launch Steam before trying again, or validate manually through the Steam application.", "Steam Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                }
                
                bw.Dispose();
            };
            bw.RunWorkerAsync();
            */        
            BackupManager.DeleteOriginals();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog findMGSV = new OpenFileDialog();
            findMGSV.Filter = "Metal Gear Solid V|MGSVTPP.exe";
            DialogResult findResult = findMGSV.ShowDialog();
            if (findResult != DialogResult.OK) return;

            string fileDir = Path.GetDirectoryName(findMGSV.FileName);
            if (fileDir != textInstallPath.Text)
            {
                textInstallPath.Text = fileDir;
                Properties.Settings.Default.InstallPath = fileDir;
                Properties.Settings.Default.Save();
            }
        }
    }
}