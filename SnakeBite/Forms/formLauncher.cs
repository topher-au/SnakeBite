using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
using System.Media;
using System.IO;

namespace SnakeBite
{
    public partial class formLauncher : Form
    {
        private CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        public formLauncher()
        {
            InitializeComponent();
        }

        private void formLauncher_Load(object sender, EventArgs e)
        {
            textInfo = cultureInfo.TextInfo;

            // Retrieve and display version info
            var MGSVersionInfo = FileVersionInfo.GetVersionInfo(Properties.Settings.Default.InstallPath + "\\mgsvtpp.exe");

            string SBVersion = Application.ProductVersion;
            string MGSVersion = MGSVersionInfo.ProductVersion;

            string VersionText = String.Format("MGSV {0} / SB {1}", MGSVersion, SBVersion);
            labelVersion.Text = VersionText;
            UpdateVersionLabel();

            // Fade in form
            Opacity = 0;
            int duration = 100;//in milliseconds
            int steps = 30;
            Timer timer = new Timer();
            timer.Interval = duration / steps;

            int currentStep = 0;
            timer.Tick += (arg1, arg2) =>
            {
                Opacity = ((double)currentStep) / steps;
                currentStep++;

                if (Opacity == 1)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };

            timer.Start();
        }

        private void formLauncher_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Handle keypresses on launcher
            switch(e.KeyChar)
            {
                case (char)Keys.Escape:
                    ExitLauncher();
                    break;
                case (char)Keys.Enter:
                    StartGame();
                    break;
            }
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            Control control = sender as Control;
            control.ForeColor = Color.Red;
            control.Text = control.Text.ToUpper();
            PlaySound("ui_move");
        }

        private void OnMouseExit(object sender, EventArgs e)
        {
            Control control = sender as Control;
            control.ForeColor = Color.White;
            control.Text = textInfo.ToTitleCase(control.Text.ToLower());
        }

        private void PlaySound(string ResourceName)
        {
            if (!Properties.Settings.Default.EnableSound) return;

            BackgroundWorker soundWorker = new BackgroundWorker();
            soundWorker.DoWork += (obj, var) =>
            {
                Stream str = Properties.Resources.ResourceManager.GetStream(ResourceName);
                SoundPlayer snd = new SoundPlayer(str);
                snd.Play();
                System.Threading.Thread.Sleep(250);
                soundWorker.Dispose();
            };
            soundWorker.RunWorkerAsync();
            
        }

        private void StartGame()
        {
            // TODO: Implement StartGame()

            ExitLauncher();
        }

        private void ShowMods()
        {
            PlaySound("ui_select");
            formMods Mods = new formMods();
            Mods.ShowDialog();
        }

        private void ShowConfiguration()
        {
            PlaySound("ui_select");
            formSettings Settings = new formSettings();
            Settings.ShowDialog();
        }

        private void ExitLauncher()
        {
            PlaySound("ui_select");
            Hide();
            System.Threading.Thread.Sleep(200);
            Close();
        }
        

        private void buttonStartGame_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void buttonMods_Click(object sender, EventArgs e)
        {
            ShowMods();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            ShowConfiguration();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            ExitLauncher();
        }

        private void labelVersion_DoubleClick(object sender, EventArgs e)
        {
            Process.Start("log.txt");
        }

        private void labelVersion_TextChanged(object sender, EventArgs e)
        {
            UpdateVersionLabel();
        }

        private void UpdateVersionLabel()
        {
            labelVersion.Refresh();
            labelVersion.Left = Width - labelVersion.Width - 8;
            labelVersion.Top = Height - labelVersion.Height - 8;
        }
    }
}
