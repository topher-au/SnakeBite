﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using static SnakeBite.GamePaths;

namespace SnakeBite
{
    public static class BackupManager
    {

        public static bool OriginalsExist()
        {
            return (File.Exists(ZeroPath + original_ext) && File.Exists(OnePath + original_ext) && File.Exists(chunk0Path + original_ext));
        }

        public static bool OriginalZeroOneExist()
        {
            return (File.Exists(ZeroPath + original_ext) && File.Exists(OnePath + original_ext));
        }

        public static bool c7t7Exist()
        {
            return (File.Exists(t7Path) && File.Exists(c7Path));
        }

        public static bool ModsDisabled()
        {
            return (File.Exists(ZeroPath + modded_ext) && File.Exists(OnePath + modded_ext));
        }

        public static void RestoreOriginals()
        {
            // delete existing data
            File.Delete(ZeroPath);
            File.Delete(OnePath);
            File.Delete(chunk0Path);
            File.Delete(c7Path);
            File.Delete(t7Path);

            // delete mod data
            File.Delete(ZeroPath + modded_ext);
            File.Delete(OnePath + modded_ext);

            // delete GameDir data
            List<string> fileEntryDirs = new List<string>();
            foreach (string externalFile in new SettingsManager(SnakeBiteSettings).GetModExternalFiles())
            {
                string fileModPath = Tools.ToWinPath(externalFile);
                string sourceFullPath = Path.Combine(GameDir, fileModPath);
                string fullPathDir = Path.GetDirectoryName(sourceFullPath);

                try
                {
                    if (File.Exists(sourceFullPath))
                    {
                        File.Delete(sourceFullPath);
                        if (!fileEntryDirs.Contains(fullPathDir)) fileEntryDirs.Add(fullPathDir);
                    }
                }
                catch
                {
                    Debug.LogLine("[Uninstall] Could not delete " + fileModPath);
                }
                
            }

            foreach (string fileEntryDir in fileEntryDirs)
            {
                if (Directory.Exists(fileEntryDir))
                {
                    try
                    {
                        if (Directory.GetFiles(fileEntryDir).Length == 0)
                        {
                            Directory.Delete(fileEntryDir);
                        }
                    }

                    catch
                    {
                        Debug.LogLine("[Uninstall] Could not delete: " + fileEntryDir);
                    }
                }
            }

            // restore backups
            bool fileExists = true;
            while (fileExists)
            {
                Thread.Sleep(100);
                fileExists = false;
                if (File.Exists(ZeroPath)) fileExists = true;
                if (File.Exists(OnePath)) fileExists = true;
                if (File.Exists(chunk0Path)) fileExists = true;
                if (File.Exists(c7Path)) fileExists = true;
                if (File.Exists(t7Path)) fileExists = true;
            }

            File.Move(ZeroPath + original_ext, ZeroPath);
            File.Move(OnePath + original_ext, OnePath);
            File.Move(chunk0Path + original_ext, chunk0Path);
        }

        public static void DeleteOriginals()
        {
            // delete backups
            File.Delete(ZeroPath + original_ext);
            File.Delete(OnePath + original_ext);
            File.Delete(chunk0Path + original_ext);
        }

        public static void SwitchToOriginal()
        {
            if (OriginalZeroOneExist())
            {
                // copy mod files to backup
                File.Copy(ZeroPath, ZeroPath + modded_ext, true);
                File.Copy(OnePath, OnePath + modded_ext, true);

                // copy original files
                File.Copy(ZeroPath + original_ext, ZeroPath, true);
                File.Copy(OnePath + original_ext, OnePath, true);

                SettingsManager manager = new SettingsManager(SnakeBiteSettings);
                manager.UpdateDatHash();
            }
        }

        public static void SwitchToMods()
        {
            if (ModsDisabled())
            {
                // restore mod backup
                File.Copy(ZeroPath + modded_ext, ZeroPath, true);
                File.Copy(OnePath + modded_ext, OnePath, true);

                // delete mod backup
                File.Delete(ZeroPath + modded_ext);
                File.Delete(OnePath + modded_ext);
                SettingsManager manager = new SettingsManager(SnakeBiteSettings);
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

            object param = Path.GetFileName(ZeroPath);
            backupProcessor.ReportProgress(0, string.Format("{0:n0}", $"{param} ({Tools.GetFileSizeKB(ZeroPath)} KB)"));
            File.Copy(ZeroPath, ZeroPath + original_ext, true);

            param = Path.GetFileName(OnePath);
            backupProcessor.ReportProgress(0, string.Format("{0:n0}", $"{param} ({Tools.GetFileSizeKB(OnePath)} KB)"));
            File.Copy(OnePath, OnePath + original_ext, true);

            param = Path.GetFileName(chunk0Path);
            backupProcessor.ReportProgress(0, string.Format("{0:n0}", $"{param} ({Tools.GetFileSizeKB(chunk0Path)} KB)"));
            File.Copy(chunk0Path, chunk0Path + original_ext, true);
        }

        public static bool BackupExists()
        {
            return (OriginalsExist());
        }

        public static bool GameFilesExist()
        {
            return (File.Exists(ZeroPath) && File.Exists(OnePath) && File.Exists(chunk0Path));
        }
        
    }

    public enum BackupState
    {
        Unknown,
        ModActive,
        DefaultActive,
    }
}