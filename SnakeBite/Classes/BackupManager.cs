using System.IO;

namespace SnakeBite
{
    public static class BackupManager
    {
        private static string GameZero { get { return Path.Combine(ModManager.GameDir, "master\\0\\00.dat"); } }
        private static string GameOne { get { return Path.Combine(ModManager.GameDir, "master\\0\\01.dat"); } }
        private static string GameTexture7 { get { return Path.Combine(ModManager.GameDir, "master\\a_texture7.dat"); } }
        private static string GameChunk7 { get { return Path.Combine(ModManager.GameDir, "master\\a_chunk7.dat"); } }
        private static string ModZero { get { return Path.Combine(ModManager.GameDir, "master\\0\\00.dat.modded"); } }
        private static string ModOne { get { return Path.Combine(ModManager.GameDir, "master\\0\\01.dat.modded"); } }
        private static string OriginalZero { get { return Path.Combine(ModManager.GameDir, "master\\0\\00.dat.original"); } }
        private static string OriginalOne { get { return Path.Combine(ModManager.GameDir, "master\\0\\01.dat.original"); } }

        public static bool OriginalsExist()
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

            // delete mod data
            if (File.Exists(ModZero)) File.Delete(ModZero);
            if (File.Exists(ModOne)) File.Delete(ModOne);

            // restore backups
            File.Move(OriginalZero, GameZero);
            File.Move(OriginalOne, GameOne);
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
        }

        public static bool BackupExists()
        {
            return (OriginalsExist());
        }

        public static bool GameFilesExist()
        {
            return (File.Exists(GameZero) && File.Exists(GameOne));
        }
    }

    public enum BackupState
    {
        Unknown,
        ModActive,
        DefaultActive,
    }
}