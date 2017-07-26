using System.IO;

namespace SnakeBite
{
    public static class BackupManager
    {
        private static string GameZero { get { return Path.Combine(ModManager.GameDir, "master\\0\\00.dat"); } }
        private static string GameOne { get { return Path.Combine(ModManager.GameDir, "master\\0\\01.dat"); } }
        private static string GameTexture1 { get { return Path.Combine(ModManager.GameDir, "master\\texture1.dat"); } }
        private static string ModZero { get { return Path.Combine(ModManager.GameDir, "master\\0\\00.dat.modded"); } }
        private static string ModOne { get { return Path.Combine(ModManager.GameDir, "master\\0\\01.dat.modded"); } }
        private static string ModTexture1 { get { return Path.Combine(ModManager.GameDir, "master\\texture1.dat.modded"); } }
        private static string OriginalZero { get { return Path.Combine(ModManager.GameDir, "master\\0\\00.dat.original"); } }
        private static string OriginalOne { get { return Path.Combine(ModManager.GameDir, "master\\0\\01.dat.original"); } }
        private static string OriginalTexture1 { get { return Path.Combine(ModManager.GameDir, "master\\texture1.dat.original"); } }

        public static bool OriginalsExist()
        {
            return (File.Exists(OriginalZero) && File.Exists(OriginalOne));
        }

        public static bool texture1Exists()
        {
            return (File.Exists(OriginalTexture1));
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
            if (File.Exists(GameTexture1)) File.Delete(GameTexture1);

            // delete mod data
            if (File.Exists(ModZero)) File.Delete(ModZero);
            if (File.Exists(ModOne)) File.Delete(ModOne);
            if (File.Exists(ModTexture1)) File.Delete(ModTexture1);

            // restore backups
            File.Move(OriginalZero, GameZero);
            File.Move(OriginalOne, GameOne);
            File.Move(OriginalTexture1, GameTexture1);
        }

        public static void DeleteOriginals()
        {
            // delete backups
            if (File.Exists(OriginalZero)) File.Delete(OriginalZero);
            if (File.Exists(OriginalOne)) File.Delete(OriginalOne);
        }

        public static void SwitchToOriginal()
        {
            if (OriginalsExist())
            {
                // copy mod files to backup
                File.Copy(GameZero, ModZero, true);
                File.Copy(GameOne, ModOne, true);

                // copy original files
                File.Copy(OriginalZero, GameZero, true);
                File.Copy(OriginalOne, GameOne, true);

                SettingsManager manager = new SettingsManager(ModManager.GameDir);
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
                SettingsManager manager = new SettingsManager(ModManager.GameDir);
                manager.UpdateDatHash();
            }
        }

        public static void CopyBackupFiles(bool Overwrite = false)
        {
            if (!File.Exists(OriginalOne) || Overwrite)
            {
                // copy one
                File.Copy(GameOne, OriginalOne, true);
            }
            if (!File.Exists(OriginalZero) || Overwrite)
            {
                // copy one
                File.Copy(GameZero, OriginalZero, true);
            }
            if (!File.Exists(OriginalTexture1) || Overwrite)
            {
                // copy one
                File.Copy(GameTexture1, OriginalTexture1, true);
            }
        }

        public static bool BackupExists()
        {
            return (OriginalsExist() && texture1Exists());
        }

        public static bool GameFilesExist()
        {
            return (File.Exists(GameZero) && File.Exists(GameOne) && File.Exists(GameTexture1));
        }
    }

    public enum BackupState
    {
        Unknown,
        ModActive,
        DefaultActive,
    }
}