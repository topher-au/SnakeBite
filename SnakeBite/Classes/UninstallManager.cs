using GzsTool.Core.Fpk;
using GzsTool.Core.Qar;
using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static SnakeBite.GamePaths;

namespace SnakeBite
{
    class UninstallManager
    {
        public static bool UninstallMods(CheckedListBox.CheckedIndexCollection modIndices, bool skipCleanup = false) // Uninstalls mods based on their indices in the list
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.LogLine("[Uninstall] Start", Debug.LogLevel.Basic);
            ModManager.ClearBuildFiles(ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath);
            ModManager.ClearSBGameDir();
            ModManager.CleanupFolders();
            if (Properties.Settings.Default.AutosaveRevertPreset == true)
            {
                Debug.LogLine("[Uninstall] Saving RevertChanges.MGSVPreset.SB_Build", Debug.LogLevel.Basic);
                PresetManager.SavePreset(SavePresetPath + build_ext);
            }
            else
            {
                Debug.LogLine("[Uninstall] Skipping RevertChanges.MGSVPreset Save", Debug.LogLevel.Basic);
            }

            File.Copy(SnakeBiteSettings, SnakeBiteSettings + build_ext, true);
            GzsLib.LoadDictionaries();
            SettingsManager manager = new SettingsManager(SnakeBiteSettings + build_ext);
            List<ModEntry> mods = manager.GetInstalledMods();
            List<ModEntry> uninstallMods = new List<ModEntry>();
            foreach (int index in modIndices)
            {
                ModEntry mod = mods[index];
                uninstallMods.Add(mod);
            }

            Debug.LogLine("[Uninstall] Extracting 00.dat to _working0", Debug.LogLevel.Basic);
            var zeroFiles = GzsLib.ExtractArchive<QarFile>(ZeroPath, "_working0"); // extracts 00.dat and creates a list of filenames, which is pruned throughout the uninstall process and repacked at the end.
            List<string> oneFiles = null;
            bool hasFtexs = ModManager.foundLooseFtexs(uninstallMods);
            if (hasFtexs)
            {
                oneFiles = GzsLib.ExtractArchive<QarFile>(OnePath, "_working1"); // if necessary, extracts 01.dat and creates a list of filenames similar to zeroFiles. only textures are pruned from the list.
            }
            //end of qar extraction
            GameData gameData = manager.GetGameData();
            ModManager.ValidateGameData(ref gameData, ref zeroFiles);

            Debug.LogLine("[Install] Building gameFiles lists", Debug.LogLevel.Basic);
            var baseGameFiles = GzsLib.ReadBaseData();
            try
            {
                ModManager.WriteGameDirSbBuild();
                foreach (ModEntry mod in uninstallMods)
                {
                    //UninstallMod(mod, manager, baseGameFiles, ref zeroFiles, ref oneFiles);
                }

                Debug.LogLine("[Uninstall] Rebuilding 00.dat", Debug.LogLevel.Basic);
                zeroFiles.Sort();
                GzsLib.WriteQarArchive(ZeroPath + build_ext, "_working0", zeroFiles, GzsLib.zeroFlags);

                if (hasFtexs)
                {
                    Debug.LogLine("[Install] Rebuilding 01.dat", Debug.LogLevel.Basic);
                    oneFiles.Sort();
                    GzsLib.WriteQarArchive(OnePath + build_ext, "_working1", oneFiles, GzsLib.oneFlags);
                }
                // end of qar repacking

                Debug.LogLine("[Uninstall] Updating 00.dat hash", Debug.LogLevel.Basic);
                manager.UpdateDatHash();

                foreach (ModEntry mod in uninstallMods)
                {
                    Debug.LogLine("[Uninstall] Removing mod entry from snakebite.xml", Debug.LogLevel.Basic);
                    manager.RemoveMod(mod);
                }
                //DEBUGNOW CleanupDatabase();
                //Debug.LogLine("CleanupFolders", Debug.LogLevel.Basic); //allready logs

                ModManager.PromoteGameDirFiles();
                ModManager.PromoteBuildFiles(ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath);


                if (!skipCleanup)
                {
                    ModManager.CleanupFolders();
                }
                ModManager.ClearSBGameDir();
                Debug.LogLine("[Uninstall] Uninstall complete", Debug.LogLevel.Basic);
                stopwatch.Stop();
                Debug.LogLine($"[Uninstall] Uninstall took {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
                return true;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Debug.LogLine($"[Uninstall] Uninstall failed at {stopwatch.ElapsedMilliseconds} ms", Debug.LogLevel.Basic);
                Debug.LogLine("[Uninstall] Exception: " + e, Debug.LogLevel.Basic);
                MessageBox.Show("An error has occurred during the uninstallation process and SnakeBite could not uninstall the selected mod(s).\nException: " + e);
                ModManager.ClearBuildFiles(ZeroPath, OnePath, SnakeBiteSettings, SavePresetPath);
                ModManager.CleanupFolders();
                ModManager.ClearSBGameDir();
                return false;
            }
        }//UninstallMod batch
    }
}
