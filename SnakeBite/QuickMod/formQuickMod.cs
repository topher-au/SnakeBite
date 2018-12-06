using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnakeBite.ModPages;

namespace SnakeBite.QuickMod
{
    public partial class formQuickMod : Form
    {
        private int page = 0;
        private SelectZipPanel wp = new SelectZipPanel();
        private CreateModPanel cm = new CreateModPanel();
        LogPage log = new LogPage();

        public formQuickMod()
        {
            InitializeComponent();
            wp.CompatibilityChanged += () =>
            {
                CheckNextState();
            };
        }

        private void formQuickMod_Load(object sender, EventArgs e)
        {
            NextPage();
        }

        private void NextPage()
        {
            switch(page)
            {
                case 0:
                    panelContent.Controls.Clear();
                    panelContent.Controls.Add(wp);
                    labelHeader.Text = "Zip Installer";
                    page++;
                    break;
                case 1:
                    var modName = Path.GetFileNameWithoutExtension(wp.textZipFile.Text);
                    if(modName.Contains("-"))
                        modName = modName.Substring(0, modName.IndexOf("-"));
                    cm.textModName.Text = modName;
                    panelContent.Controls.Clear();
                    panelContent.Controls.Add(cm);
                    labelHeader.Text = "Confirm Installation";
                    page++;
                    break;
                case 2:
                    if (wp.textZipFile.Text.Trim() == string.Empty) return;
                    var exportFileName = cm.textModName.Text + ".mgsv";
                    if(cm.checkExport.Checked )
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "MGSV Mod File|*.mgsv";
                        sfd.FileName = exportFileName;
                        if(sfd.ShowDialog() == DialogResult.OK)
                        {
                            exportFileName = sfd.FileName;
                        } else
                        {
                            exportFileName = "temp.mgsv";
                        }
                    }
                    if (Directory.Exists("_temp")) Directory.Delete("_temp", true);
                    Methods.ExtractFiles(wp.textZipFile.Text, "_temp");
                    Methods.GenerateMgsv(exportFileName, cm.textModName.Text, "_temp");
                    if (PreinstallManager.CheckConflicts(exportFileName))
                    {
                        DoInstall(exportFileName);
                        
                        Close();
                    } else
                    {
                        
                    }
                    if (cm.checkExport.Checked)
                    { 
                        MessageBox.Show("Successfully exported MGSV file", "Quick Install", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    } else
                    {
                        File.Delete(exportFileName);
                    }
                    break;

            }
        }

        private void DoInstall(string OutputFile)
        {
            List<string> InstallFileList = new List<string>();
            InstallFileList.Add(OutputFile);
            ProgressWindow.Show("Installing Mod", String.Format("Installing {0}, please wait...", cm.textModName.Text), new Action((MethodInvoker)delegate { ModManager.InstallMod(InstallFileList); }), log);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            NextPage();
        }

        private void CheckNextState()
        {
            buttonNext.Enabled = wp.Compatible;
        }
    }
}
