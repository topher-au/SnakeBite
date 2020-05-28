﻿using SnakeBite.Forms;
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
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = 437;
            SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
            bool updateQarFilenames = false;
            if (Properties.Settings.Default.LastSBVersion == null || new Version(Properties.Settings.Default.LastSBVersion) < ModManager.GetSBVersion())
            {
                if (Properties.Settings.Default.LastSBVersion != null)
                {
                    updateQarFilenames = true;
                }
                Properties.Settings.Default.Upgrade();
            }

            Properties.Settings.Default.LastSBVersion = ModManager.GetSBVersion().ToString();
            Properties.Settings.Default.Save();

            Debug.Clear();

            Debug.LogLine(String.Format(
                "SnakeBite {0}\n" +
                "{1}\n" +
                "-------------------------",
                ModManager.GetSBVersion(),
                Environment.OSVersion.VersionString));

            // Delete old settings file
            if (File.Exists(GamePaths.GameDir + "\\sbmods.xml"))
            {
                Debug.LogLine("Settings v0.7 or less detected, removing");
                File.Delete(GamePaths.GameDir + "\\sbmods.xml");
                MessageBox.Show("Due to fundamental changes from version 0.8 onwards, your settings have been reset. Please re-verify or restore the game files and run the setup wizard before continuing.", "Version Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            bool showSetupWizard = true;

            if (manager.SettingsExist() && manager.ValidInstallPath)
            {
                showSetupWizard = false;
            }

            // Show wizard on first run, if folder is invalid or settings out of date
            while (showSetupWizard)
            {
                // show setup wizard
                Debug.LogLine("[Setup] Starting Setup Wizard");
                try
                {
                    SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
                    var wizResult = setupWizard.ShowDialog();
                    if (wizResult == DialogResult.Cancel) return;
                    if (wizResult == DialogResult.OK) showSetupWizard = false;
                }
                catch (Exception e)
                {
                    Debug.LogLine("[Setup] Setup Wizard error: " + e.ToString());
                }
            }
            manager = new SettingsManager(GamePaths.SnakeBiteSettings);


            string InitLog = String.Format(
                "MGS Install Folder: {0}\n" +
                "MGS Version: {1}\n" +
                "-------------------------",
                Properties.Settings.Default.InstallPath,
                ModManager.GetMGSVersion());

            Debug.LogLine(InitLog, Debug.LogLevel.Basic);

            // Process Command Line args
            // Uninstall SnakeBite
            if (args.Length == 1)
            {
                if (args[0] == "-completeuninstall")
                {
                    Debug.LogLine("Complete uninstall");
                    // Restore backup and remove settings
                    manager.DeleteSettings();
                    BackupManager.RestoreOriginals();
                    return;
                }
            }

            // Parse command line arguments
            bool doCmdLine = false;             // Process command line args?
            bool closeApp = false;              // Close app after?
            bool install = false;               // Install = true, uninstall = false
            bool resetDatHash = false;          // Rehash dat file
            bool skipConflictChecks = false;
            bool skipCleanup = false;            // Skip CleanupDatabase
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

                        case "-d":
                            resetDatHash = true;
                            break;

                        case "-x":
                            closeApp = true;
                            break;

                        case "-s":
                            skipCleanup = true;
                            break;

                        case "-c":
                            skipConflictChecks = true;
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
                Debug.LogLine("Resetting dat hash");
                manager.UpdateDatHash();
            }
            if(ModManager.GetMGSVersion() > SettingsManager.IntendedGameVersion)
            {
                var contSB = MessageBox.Show("Due to a recent game update, this version of SnakeBite is outdated, and some features will not function properly.\n\nIt is highly recommended that you do not continue, and update to the latest version of Snakebite when it becomes available.\n\nWould you still like to continue? ", "Game Version Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (contSB == DialogResult.No)
                    return;
            }
            if (!File.Exists(GamePaths.ZeroPath) || !File.Exists(GamePaths.OnePath))
            {
                MessageBox.Show(string.Format("Critical Error: SnakeBite could not locate critical game data! \n\n({0})\nand/or\n({1}).\n\nRestore your game files with backups, MGSVPreset files, or revalidating through Steam!", GamePaths.ZeroPath, GamePaths.OnePath), "Archive(s) Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    bool checkDat = false;

                    if (updateQarFilenames)
                        manager.updateQarFileNames();

                    checkDat = manager.ValidateDatHash();

                    if (!checkDat || manager.IsVanilla0001DatHash())
                    {
                        if (manager.IsVanilla0001DatHash() || manager.IsVanilla0001Size())
                        {
                            MessageBox.Show("Fresh 00.dat/01.dat detected. The setup wizard will now run.", "Game data hash mismatch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Game archive has been modified. The setup wizard will now run.", "Game data hash mismatch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
                        setupWizard.ShowDialog();
                    }
                    else if (!BackupManager.c7t7Exist()) // chunk7 and/or texture7 are missing, despite the dathash validating.
                    {
                        MessageBox.Show("To continue, SnakeBite must build a_chunk7.dat and a_texture7.dat from your current archives. The setup wizard will now run.", "Setup required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
                        setupWizard.ShowDialog();
                    }
                }
                catch (InvalidOperationException e)
                {
                    if (File.Exists(GamePaths.OnePath) && File.Exists(GamePaths.ZeroPath))
                    {
                        if (manager.IsVanilla0001Size())
                        {
                            File.Delete(GamePaths.SnakeBiteSettings);
                            SetupWizard.SetupWizard setupWizard = new SetupWizard.SetupWizard();
                            setupWizard.ShowDialog();
                        }
                    }
                    MessageBox.Show(string.Format("Critical Error: SnakeBite could not read settings data! \n\n({0})\n\nRestore your game files with backups, MGSVPreset files, or revalidating through Steam!", GamePaths.SnakeBiteSettings), "Failed To Read Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.LogLine("Error: ValidateDatHash failed: " + e);
                } 
            }

            if (doCmdLine)
            {
                Debug.LogLine("Doing cmd line args");
                formMods ModForm = new formMods();
                ModForm.Show();
                ModForm.Hide();
                if (install)
                {
                    ModForm.ProcessInstallMod(installFile, skipConflictChecks, skipCleanup); // install mod
                } else
                {
                    // uninstall
                    var mods = manager.GetInstalledMods();
                    ModEntry mod = mods.FirstOrDefault(entry => entry.Name == installFile); // select mod
                    if (mod != null)
                        ModForm.ProcessUninstallMod(mod, skipCleanup); // uninstall mod
                }
                ModForm.Dispose();

                if (closeApp) return;
            }
            
            if (Properties.Settings.Default.SkipLauncher)
                Application.Run(new formMods());
            else
                Application.Run(new formLauncher());
        }
    }
}