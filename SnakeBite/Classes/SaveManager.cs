using ICSharpCode.SharpZipLib.Zip;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SnakeBite
{
    public static class BackupManager
    {
        private static string GameZero = GamePaths.ZeroPath;
        private static string GameOne = GamePaths.OnePath;
        private static string GameChunkZero = GamePaths.chunk0Path;
        private static string BackupPath = GamePaths.BackupPath;
        private static string SnakeBiteXml = GamePaths.SnakeBiteSettings;

        public static bool OriginalsExist()
        {
            return (File.Exists(OriginalZero) && File.Exists(OriginalOne) && File.Exists(OriginalChunkZero));
        }

        public static bool OriginalZeroOneExist()
        {
            return (File.Exists(OriginalZero) && File.Exists(OriginalOne));
        }

        public static bool c7t7Exist()
        {
            return (File.Exists(GameTexture7) && File.Exists(GameChunk7));
        }

        public static bool ModsDisabled()
        {
            return (File.Exists(ModZero) && File.Exists(ModOne));
        }

        public static void RestoreOriginals()
        {
            // delete existing data
            if (File.Exists(GameZero)) File.Delete(GameZero);
            if (File.Exists(GameOne)) File.Delete(GameOne);
            if (File.Exists(GameChunkZero)) File.Delete(GameChunkZero);
            if (File.Exists(GameChunk7)) File.Delete(GameChunk7);
            if (File.Exists(GameTexture7)) File.Delete(GameTexture7);

            // delete mod data
            if (File.Exists(ModZero)) File.Delete(ModZero);
            if (File.Exists(ModOne)) File.Delete(ModOne);

            // restore backups
            bool fileExists = true;
            while (fileExists)
            {
                Thread.Sleep(100);
                fileExists = false;
                if (File.Exists(GameZero)) fileExists = true;
                if (File.Exists(GameOne)) fileExists = true;
                if (File.Exists(GameChunkZero)) fileExists = true;
                if (File.Exists(GameChunk7)) fileExists = true;
                if (File.Exists(GameTexture7)) fileExists = true;
            }

            File.Move(OriginalZero, GameZero);
            File.Move(OriginalOne, GameOne);
            File.Move(OriginalChunkZero, GameChunkZero);
        }

        public static void DeleteOriginals()
        {
            // delete backups
            if (File.Exists(OriginalZero)) File.Delete(OriginalZero);
            if (File.Exists(OriginalOne)) File.Delete(OriginalOne);
            if (File.Exists(OriginalChunkZero)) File.Delete(OriginalChunkZero);
        }

        public static void SwitchToOriginal()
        {
            if (OriginalZeroOneExist())
            {
                // copy mod files to backup
                File.Copy(GameZero, ModZero, true);
                File.Copy(GameOne, ModOne, true);

                // copy original files
                File.Copy(OriginalZero, GameZero, true);
                File.Copy(OriginalOne, GameOne, true);

                SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
                manager.UpdateDatHash();
            }
        }

        public static void SwitchToMods()
        {
            if (ModsDisabled())
            {
                // restore mod backup
                File.Copy(ModZero, GameZero, true);
                File.Copy(ModOne, GameOne, true);

                // delete mod backup
                File.Delete(ModZero);
                File.Delete(ModOne);
                SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
                manager.UpdateDatHash();
            }
        }

        /// <summary>
        /// Back up dat files
        /// as DoWorkEventHandler
        /// </summary>
        public static void backgroundWorker_CopyBackupFiles(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backupProcessor = (BackgroundWorker)sender;

            object param = Path.GetFileName(GameZero); //TODO: append filesize
            backupProcessor.ReportProgress(0, param);
            File.Copy(GameZero, OriginalZero, true);

            param = Path.GetFileName(GameOne);
            backupProcessor.ReportProgress(0, param);
            File.Copy(GameOne, OriginalOne, true);

            param = Path.GetFileName(GameChunkZero);
            backupProcessor.ReportProgress(0, param);
            File.Copy(GameChunkZero, OriginalChunkZero, true);
        }

        public static void CopyBackupFiles(bool Overwrite = true)
        {
            if (!File.Exists(OriginalOne) || Overwrite)
            {
                // copy one
                File.Copy(GameOne, OriginalOne, true);
            }
            if (!File.Exists(OriginalZero) || Overwrite)
            {
                // copy zero
                File.Copy(GameZero, OriginalZero, true);
            }
            if (!File.Exists(OriginalChunkZero) || Overwrite)
            {
                // copy chunk0
                File.Copy(GameChunkZero, OriginalChunkZero, true);
            }
        }

        public static bool BackupExists()
        {
            return (OriginalsExist());
        }

        public static bool GameFilesExist()
        {
            return (File.Exists(GameZero) && File.Exists(GameOne) && File.Exists(GameChunkZero));
        }

        public static void backgroundWorker_CopyBackupFilesZip(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backupProcessor = (BackgroundWorker)sender;
            Directory.CreateDirectory("_build");

            object param = Path.GetFileName(GameZero); //TODO: append filesize
            backupProcessor.ReportProgress(0, param);
            File.Copy(GameZero, "_build", true);

            param = Path.GetFileName(GameOne);
            backupProcessor.ReportProgress(0, param);
            File.Copy(GameOne, "_build", true);

            param = Path.GetFileName(GameChunkZero);
            backupProcessor.ReportProgress(0, param);
            File.Copy(GameChunkZero, "_build", true);

            FastZip zipper = new FastZip();
            zipper.CreateZip(BackupPath, "_build", true, "(.*?dat)");

            Directory.Delete("_build", true);
        }

        public static void SavePreset(string presetFilePath)
        {
            SettingsManager manager = new SettingsManager(SnakeBiteXml);

            Directory.CreateDirectory("_build");

            File.Copy(GameZero, "_build", true);
            File.Copy(GameOne, "_build", true);
            File.Copy(SnakeBiteXml, "_build", true);

            FastZip zipper = new FastZip();
            zipper.CreateZip(presetFilePath, "_build", true, "(.*?)");

            Directory.Delete("_build", true);
        }

        public static void LoadPreset(string presetFilePath)
        {
            Directory.CreateDirectory("_extr");

            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(presetFilePath, "_extr", "snakebite.xml");

            SettingsManager manager = new SettingsManager("_extr\\snakebite.xml");
            var presetVersion = manager.GetSettingsMGSVersion();
            var MGSVersion = ModManager.GetMGSVersion();
            if (presetVersion != MGSVersion)
            {
                if (MessageBox.Show(string.Format("This preset file is intended for Game Version {0}, but your current Game Version is {1}. Loading this preset will likely cause crashes, infinite loading screens or other significant problems in-game.", presetVersion, MGSVersion) +
                    "\n\nAre you sure you want to load this preset?", "Preset Version Mismatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }
            }
            string modsToInstall = "";
            foreach(var mod in manager.GetInstalledMods())
            {
                modsToInstall += string.Format("\n{0}", mod.Name);
            }
            DialogResult confirmInstall = MessageBox.Show(string.Format("This preset will contain the following mods:\n" + modsToInstall), "Preset Mods", MessageBoxButtons.OKCancel);
            if (confirmInstall == DialogResult.OK)
            {
                ProgressWindow.Show("Loading Preset", "Loading Preset, please wait...", new Action((MethodInvoker)delegate 
                {
                    unzipper.ExtractZip(presetFilePath, "_extr", "(.*?dat)");
                    //todo create revert presets first
                    File.Move("_extr\\00.dat", GameZero);
                    File.Move("_extr\\01.dat", GameOne);
                    File.Move("_extr\\snakebite.xml", SnakeBiteXml);
                }), log); // gonna need to send most of this logic to formMods
                
            }

            Directory.Delete("_extr", true);
        }

        private static void InstallPreset(string presetFilePath, FastZip unzipper)
        {
            unzipper.ExtractZip(presetFilePath, "_extr", "(.*?dat)");
            //todo create revert presets first
            File.Move("_extr\\00.dat", GameZero);
            File.Move("_extr\\01.dat", GameOne);
            File.Move("_extr\\snakebite.xml", SnakeBiteXml);
        }
    }

    public enum BackupState
    {
        Unknown,
        ModActive,
        DefaultActive,
    }
    

    //1.0.15.0.MGSVOriginal
    //  00.dat
    //  01.dat
    //  chunk0.dat

    //MyPreset.MGSVPreset
    //  00.dat
    //  01.dat
    //  Snakebite.xml
}