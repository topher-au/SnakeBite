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
        private static string GameChunkSeven = GamePaths.c7Path;
        private static string GameTextureSeven = GamePaths.t7Path;
        private static string BackupPath = GamePaths.BackupPath;
        private static string SnakeBiteXml = GamePaths.SnakeBiteSettings;
        private static string original_ext = GamePaths.original_ext;
        private static string modded_ext = GamePaths.modded_ext;

        public static bool OriginalsExist()
        {
            return (File.Exists(GameZero + original_ext) && File.Exists(GameOne + original_ext) && File.Exists(GameChunkZero + original_ext));
        }

        public static bool OriginalZeroOneExist()
        {
            return (File.Exists(GameZero + original_ext) && File.Exists(GameOne + original_ext));
        }

        public static bool c7t7Exist()
        {
            return (File.Exists(GameTextureSeven) && File.Exists(GameChunkSeven));
        }

        public static bool ModsDisabled()
        {
            return (File.Exists(GameZero + modded_ext) && File.Exists(GameOne + modded_ext));
        }

        public static void RestoreOriginals()
        {
            // delete existing data
            File.Delete(GameZero);
            File.Delete(GameOne);
            File.Delete(GameChunkZero);
            File.Delete(GameChunkSeven);
            File.Delete(GameTextureSeven);

            // delete mod data
            File.Delete(GameZero + modded_ext);
            File.Delete(GameOne + modded_ext);

            // restore backups
            bool fileExists = true;
            while (fileExists)
            {
                Thread.Sleep(100);
                fileExists = false;
                if (File.Exists(GameZero)) fileExists = true;
                if (File.Exists(GameOne)) fileExists = true;
                if (File.Exists(GameChunkZero)) fileExists = true;
                if (File.Exists(GameChunkSeven)) fileExists = true;
                if (File.Exists(GameTextureSeven)) fileExists = true;
            }

            File.Move(GameZero + original_ext, GameZero);
            File.Move(GameOne + original_ext, GameOne);
            File.Move(GameChunkZero + original_ext, GameChunkZero);
        }

        public static void DeleteOriginals()
        {
            // delete backups
            File.Delete(GameZero + original_ext);
            File.Delete(GameOne + original_ext);
            File.Delete(GameChunkZero + original_ext);
        }

        public static void SwitchToOriginal()
        {
            if (OriginalZeroOneExist())
            {
                // copy mod files to backup
                File.Copy(GameZero, GameZero + modded_ext, true);
                File.Copy(GameOne, GameOne + modded_ext, true);

                // copy original files
                File.Copy(GameZero + original_ext, GameZero, true);
                File.Copy(GameOne + original_ext, GameOne, true);

                SettingsManager manager = new SettingsManager(SnakeBiteXml);
                manager.UpdateDatHash();
            }
        }

        public static void SwitchToMods()
        {
            if (ModsDisabled())
            {
                // restore mod backup
                File.Copy(GameZero + modded_ext, GameZero, true);
                File.Copy(GameOne + modded_ext, GameOne, true);

                // delete mod backup
                File.Delete(GameZero + modded_ext);
                File.Delete(GameOne + modded_ext);
                SettingsManager manager = new SettingsManager(SnakeBiteXml);
                manager.UpdateDatHash();
            }
            if (ModsDisabled())
            {

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
            File.Copy(GameZero, GameZero + original_ext, true);

            param = Path.GetFileName(GameOne);
            backupProcessor.ReportProgress(0, param);
            File.Copy(GameOne, GameOne + original_ext, true);

            param = Path.GetFileName(GameChunkZero);
            backupProcessor.ReportProgress(0, param);
            File.Copy(GameChunkZero, GameChunkZero + original_ext, true);
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
    }

    public enum BackupState
    {
        Unknown,
        ModActive,
        DefaultActive,
    }
}