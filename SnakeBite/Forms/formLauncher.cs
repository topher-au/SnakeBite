using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Serialization;

namespace SnakeBite
{
    public partial class formLauncher : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);


        private CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
        private TextInfo textInfo;

        private Color baseColour;
        private Color hoverColour;
        private Color exitColour;
        private SoundPlayer playerMove;
        private SoundPlayer playerSelect;
        

        public formLauncher()
        {
            
            InitializeComponent();
        }

        private void formLauncher_Load(object sender, EventArgs e)
        {
            textInfo = cultureInfo.TextInfo;

            // Check for updates
            /* //tex OFF since topher has left it
            Debug.LogLine("[Update] Checking for updates");
            UpdateFile updater = new UpdateFile();
            bool updateSuccess = updater.ReadXmlFromInterweb("http://www.xobanimot.com/snakebite/update/update.xml");
            if (updateSuccess)
            {
                if (updater.SnakeBite.Version.AsVersion() > ModManager.GetSBVersion())
                {
                    Debug.LogLine(String.Format("Update found! Version {0} is available", updater.SnakeBite.Version.AsVersion()));
                    labelUpdate.Text = String.Format("SnakeBite version {0} now available!", updater.SnakeBite.Version.AsString());
                    labelUpdate.Show();
                } else
                {
                    Debug.LogLine("No update found");
                }
            } else {
                Debug.LogLine("Could not read update.xml");
            }
            */

            // Retrieve and display version info
            var MGSVersionInfo = FileVersionInfo.GetVersionInfo(Properties.Settings.Default.InstallPath + "\\mgsvtpp.exe");

            string SBVersion = Application.ProductVersion;
            string MGSVersion = MGSVersionInfo.ProductVersion;

            // Update version text
            string VersionText = String.Format("MGSV {0} / SB {1}", MGSVersion, SBVersion);
            labelVersion.Text = VersionText;
            UpdateVersionLabel();

            UpdateModToggle();

            SetupTheme();

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

        public void SetupTheme()
        {

            // Load theme
            if (File.Exists(Properties.Settings.Default.ThemeFile))
            {
                // attempt to load data from theme file
                try
                {
                    ZipFile themeFile = new ZipFile(Properties.Settings.Default.ThemeFile);
                    var themeEntry = themeFile.FindEntry("Theme.xml", true);
                    if (themeEntry >= 0)
                    {
                        var themeStream = themeFile.GetInputStream(themeFile[themeEntry]);
                        using (StreamReader themeReader = new StreamReader(themeStream))
                        {
                            XmlSerializer themeSerializer = new XmlSerializer(typeof(ThemeXml.Theme));
                            var theme = (ThemeXml.Theme)themeSerializer.Deserialize(themeReader);
                            baseColour = Color.FromArgb(theme.BaseColour.alpha, theme.BaseColour.red, theme.BaseColour.green, theme.BaseColour.blue);
                            hoverColour = Color.FromArgb(theme.HoverColour.alpha, theme.HoverColour.red, theme.HoverColour.green, theme.HoverColour.blue);
                            exitColour = Color.FromArgb(theme.ExitColour.alpha, theme.ExitColour.red, theme.ExitColour.green, theme.ExitColour.blue);
                        }
                    }
                    var bgEntry = themeFile.FindEntry("launcherbg.png", true);
                    if (bgEntry >= 0)
                    {
                        BackgroundImage = Image.FromStream(themeFile.GetInputStream(themeFile[bgEntry]));
                    }
                    // TODO: implemenmt theme sound effects
                    var soundMoveEntry = themeFile.FindEntry("ui_move.wav", true);
                    if(soundMoveEntry >= 0)
                    {
                        playerMove = new SoundPlayer(themeFile.GetInputStream(themeFile[soundMoveEntry]));
                    }
                    var soundSelectEntry = themeFile.FindEntry("ui_select.wav", true);
                    if (soundSelectEntry >= 0)
                    {
                        playerSelect = new SoundPlayer(themeFile.GetInputStream(themeFile[soundSelectEntry]));
                    }
                }
                catch
                {

                }
            } else
            {
                // Setup default theme
                BackgroundImage = Properties.Resources.LAUNCHERBG;
                baseColour = Color.White;
                hoverColour = Color.Red;
                exitColour = Color.DarkGray;
                playerMove = new SoundPlayer(Properties.Resources.ui_move);
                playerSelect = new SoundPlayer(Properties.Resources.ui_select);
            }

            buttonStartGame.ForeColor = baseColour;
            buttonMods.ForeColor = baseColour;
            buttonSettings.ForeColor = baseColour;
            buttonExit.ForeColor = baseColour;
            labelClose.ForeColor = exitColour;
            labelVersion.ForeColor = baseColour;
        }

        private void formLauncher_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Handle keypresses on launcher
            switch (e.KeyChar)
            {
                case (char)Keys.Escape:
                    ExitLauncher(true);
                    break;

                case (char)Keys.Enter:
                    StartGame(true);
                    break;
            }
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            PlaySound("ui_move");
            Control control = sender as Control;
            control.ForeColor = hoverColour;
            control.Text = textInfo.ToUpper(control.Text);
        }

        private void OnMouseExit(object sender, EventArgs e)
        {
            Control control = sender as Control;
            control.ForeColor = baseColour;
            control.Text = textInfo.ToTitleCase(control.Text.ToLower());
        }

        private void PlaySound(string SoundName)
        {
            if (!Properties.Settings.Default.EnableSound) return;

            BackgroundWorker soundWorker = new BackgroundWorker();
            soundWorker.DoWork += (obj, var) =>
            {
                switch(SoundName)
                {
                    case "ui_move":
                        playerMove.Play();
                        break;
                    case "ui_select":
                        playerSelect.Play();
                        break;
                    default:
                        Stream str = Properties.Resources.ResourceManager.GetStream(SoundName);
                        SoundPlayer snd = new SoundPlayer(str);
                        snd.Play();
                        break;
                }
                
                System.Threading.Thread.Sleep(200);
                soundWorker.Dispose();
            };
            soundWorker.RunWorkerAsync();
        }

        private void StartGame(bool silent = false)
        {
            SettingsManager manager = new SettingsManager(ModManager.GameDir);
            if (manager.ValidInstallPath)
            {
                Process.Start(ModManager.GameDir + "\\mgsvtpp.exe");
                ExitLauncher(silent);
            }
            else
            {
                MessageBox.Show("Unable to locate MGSVTPP.exe. Please check the Settings and try again.", "Error launching MGSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            Settings.Owner = this;
            Settings.ShowDialog();
        }

        private void ExitLauncher(bool silent = false)
        {
            if (!silent) PlaySound("ui_select");
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
            UpdateModToggle();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            ExitLauncher();
        }

        private void labelVersion_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(Debug.LOG_FILE);
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
            labelUpdate.Left = 8;
            labelUpdate.Top = Height - labelUpdate.Height - 8;
        }

        private void UpdateModToggle()
        {
            // Enable/disable mods button

            if(BackupManager.ModsDisabled())
            {
                buttonMods.Enabled = false;
                picModToggle.Image = Properties.Resources.toggleoff;
            } else
            {
                buttonMods.Enabled = true;
                picModToggle.Image = Properties.Resources.toggleon;
            }
            

        }

        private void labelClose_Click(object sender, EventArgs e)
        {
            ExitLauncher(true);
        }

        private void labelUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var launchUpdate = MessageBox.Show(String.Format("A new version of SnakeBite is available!\n\nWould you like to update now?"), "SnakeBite Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (launchUpdate == DialogResult.Yes)
            {
                if (File.Exists("sbupdater.exe"))
                {
                    Process.Start("sbupdater.exe", "-u");
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show("SnakeBite updater appears to be missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void formLauncher_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void picModToggle_Click(object sender, EventArgs e)
        {
            PlaySound("ui_select");
            if (BackupManager.ModsDisabled())
            {
                ProgressWindow.Show("Working","Enabling mods, please wait...", new Action(BackupManager.SwitchToMods));
            }
            else
            {
                ProgressWindow.Show("Working","Disabling mods, please wait...", new Action(BackupManager.SwitchToOriginal));
            }
            UpdateModToggle();
        }

    }
}