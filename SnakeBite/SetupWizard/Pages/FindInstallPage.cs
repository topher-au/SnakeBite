using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SnakeBite.SetupWizard
{
    public partial class FindInstallPage : UserControl
    {
        public FindInstallPage()
        {
            InitializeComponent();
            if(SettingsManager.ValidInstallPath)
                textInstallPath.Text = Properties.Settings.Default.InstallPath;
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            var doValidate = MessageBox.Show("Modded game data and backups will be reset. Although Steam will scan the entire installation, you can cancel the validation after any modified files are deleted by Steam and they will be redownloaded immediately.", "SnakeBite", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (doValidate == DialogResult.Cancel) return;
            System.Diagnostics.Process.Start("steam://validate/287700/");
            BackupManager.DeleteOriginals();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog findMGSV = new OpenFileDialog();
            findMGSV.Filter = "Metal Gear Solid V|MGSVTPP.exe";
            DialogResult findResult = findMGSV.ShowDialog();
            if (findResult != DialogResult.OK) return;

            string filePath = Path.GetDirectoryName(findMGSV.FileName);
            if (filePath != textInstallPath.Text)
            {
                textInstallPath.Text = filePath;
                Properties.Settings.Default.InstallPath = filePath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
