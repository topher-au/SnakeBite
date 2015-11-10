using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SnakeBite
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Properties.Settings.Default.Upgrade();
            SettingsManager.DisableConflictCheck = false;
            Debug.Clear();

            string InitLog = String.Format(
                "SnakeBite Version: {0}\n" +
                "-------------------------\n" +
                "MGS Install Folder: {1}\n" +
                "MGS Version: {2}\n" +
                "-------------------------\n",
                ModManager.GetSBVersion(),
                Properties.Settings.Default.InstallPath,
                ModManager.GetMGSVersion());

            Debug.LogLine(InitLog, Debug.LogLevel.Basic);

            // Process Command Line args
            // TODO: test all command line args

            // Uninstall SnakeBite
            if (args.Length == 1)
            {
                if (args[0] == "-completeuninstall")
                {
                    // Restore backup and remove settings
                    SettingsManager.DeleteSettings();
                    BackupManager.RestoreOriginals();
                    Application.Exit();
                    return;
                }
            }

            // Parse command line arguments
            bool doCmdLine = false;             // Process command line args?
            bool closeApp = false;              // Close app after?
            bool install = false;               // Install = true, uninstall = false
            bool ignoreConflicts = false;       // Bypass conflict check
            bool resetDatHash = false;          // Rehash dat file
            string installFile = String.Empty;
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "-i":
                            install = true;
                            break;

                        case "-u":
                            install = false;

                            break;

                        case "-c":
                            ignoreConflicts = true;
                            break;

                        case "-d":
                            resetDatHash = true;
                            break;

                        case "-x":
                            closeApp = true;
                            break;

                        default:
                            installFile = arg;
                            doCmdLine = true;
                            break;
                    }
                }
            }

            // Update dat hash in settings
            if (resetDatHash)
            {
                SettingsManager.UpdateDatHash();
            }

            // Show wizard on first run, if folder is invalid or settings out of date
            if (!SettingsManager.SettingsExist() || !SettingsManager.ValidInstallPath || SettingsManager.GetSettingsVersion() < 700)
            {
                // show setup wizard
                SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
                setupWizard.ShowDialog();
            }

            // force user to run setup wizard
            if (!SettingsManager.SettingsExist() || !SettingsManager.ValidInstallPath || SettingsManager.GetSettingsVersion() < 700)
            {
                Application.Exit();
            }

            var checkDat = SettingsManager.ValidateDatHash();

            if (!checkDat)
            {
                MessageBox.Show("The game data appears to have been modified outside of SnakeBite.\n\nThe Setup Wizard will now run.", "Game data hash mismatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
                setupWizard.ShowDialog();
            }

            if (doCmdLine)
            {
                formMods ModForm = new formMods();
                ModForm.Show();
                ModForm.Hide();
                if (install)
                {
                    // install
                    ModForm.ProcessInstallMod(installFile, ignoreConflicts); // install mod
                }
                else
                {
                    // uninstall
                    var mods = SettingsManager.GetInstalledMods();
                    ModEntry mod = mods.FirstOrDefault(entry => entry.Name == installFile); // select mod

                    if (mod != null)
                        ModForm.ProcessUninstallMod(mod); // uninstall mod
                }
                ModForm.Dispose();

                if (closeApp) return; 

            }

            // Check for updates
            UpdateFile updater = new UpdateFile();
            bool updateSuccess = updater.ReadXmlFromInterweb("http://www.xobanimot.com/snakebite/update/update.xml");
            if (updateSuccess)
            {
                if (updater.SnakeBite.Version > ModManager.GetSBVersion())
                {
                    var launchUpdate = MessageBox.Show(String.Format("SnakeBite v{0} is now available.\n\nWould you like to update now?", updater.SnakeBite.Version), "SnakeBite Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
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
            }

            //Application.Run(new formMain());
            Application.Run(new formLauncher());
        }
    }
}