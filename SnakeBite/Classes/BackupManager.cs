using System.IO;

namespace SnakeBite
{
    public static class BackupManager
    {
        private static string GameZero { get { return Path.Combine(ModManager.GameDir, "master\\0\\00.dat"); } }
        private static string GameOne { get { return Path.Combine(ModManager.GameDir, "master\\0\\01.dat"); } }
        private static string ModZero { get { return Path.Combine(ModManager.GameDir, "master\\0\\00.dat.modded"); } }
        private static string ModOne { get { return Path.Combine(ModManager.GameDir, "master\\0\\01.dat.modded"); } }
        private static string OriginalZero { get { return Path.Combine(ModManager.GameDir, "master\\0\\00.dat.original"); } }
        private static string OriginalOne { get { return Path.Combine(ModManager.GameDir, "master\\0\\01.dat.original"); } }

        public static bool OriginalsExist()
        {
            return (File.Exists(OriginalZero) && File.Exists(OriginalOne));
        }

        public static bool ModsDisabled()
        {
            return (File.Exists(ModZero) && File.Exists(ModOne));
        }

        public static void RestoreOriginals()
        {
            // delete existing data
            if (File.Exists(GameOne)) File.Delete(GameOne);
            if (File.Exists(GameZero)) File.Delete(GameZero);

            // delete mod data
            if (File.Exists(ModOne)) File.Delete(ModOne);
            if (File.Exists(ModZero)) File.Delete(ModZero);

            // restore backups
            File.Move(OriginalZero, GameZero);
            File.Move(OriginalOne, GameOne);
        }

        public static void DeleteOriginals()
        {
            // delete backups
            if (File.Exists(OriginalOne)) File.Delete(OriginalOne);
            if (File.Exists(OriginalZero)) File.Delete(OriginalZero);
        }

        public static void SwitchToOriginal()
        {
            if (!File.Exists(ModZero) || !File.Exists(ModOne))
            {
                // copy mod files to backup
                File.Copy(GameZero, ModZero, true);
                File.Copy(GameOne, ModOne, true);

                // copy original files
                File.Copy(OriginalZero, GameZero, true);
                File.Copy(OriginalOne, GameOne, true);

                SettingsManager.UpdateDatHash();
            }
        }

        public static void SwitchToMods()
        {
            if (File.Exists(ModZero) && File.Exists(ModOne))
            {
                // restore mod backup
                File.Copy(ModOne, GameOne, true);
                File.Copy(ModZero, GameZero, true);

                // delete mod backup
                File.Delete(ModOne);
                File.Delete(ModZero);

                SettingsManager.UpdateDatHash();
            }
        }

        public static void CopyBackupFiles(bool Overwrite = false)
        {
            if (!File.Exists(OriginalZero) || Overwrite)
            {
                // copy zero
                File.Copy(GameZero, OriginalZero, true);
            }
            if (!File.Exists(OriginalOne) || Overwrite)
            {
                // copy one
                File.Copy(GameOne, OriginalOne, true);
            }
        }

        public static bool BackupExists()
        {
            if (File.Exists(OriginalZero) || File.Exists(OriginalOne))
            {
                return true;
            }
            return false;
        }

        public static bool GameFilesExist()
        {
            return File.Exists(GameZero) && File.Exists(GameOne);
        }
    }

    public enum BackupState
    {
        Unknown,
        ModActive,
        DefaultActive,
    }
}