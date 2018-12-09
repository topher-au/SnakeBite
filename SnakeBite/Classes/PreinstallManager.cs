using ICSharpCode.SharpZipLib.Zip;
using SnakeBite.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBite
{
    internal static class PreinstallManager
    {
        /*
         * PreinstallManager
         * Designed to support formInstallOrder with file reading/writing, conflict checking and installation processing
         * 
         */

        public static void RefreshXml(List<PreinstallEntry> Mods) // adds mods to an .xml file. Similar to snakebite.xml, but with yet-to-be-installed mods.
        {
            FastZip unzipper = new FastZip();
            SettingsManager infoXml = new SettingsManager("_extr\\buildInfo.xml");
            // SettingsManager was originally a static class for managing snakebite.xml. It has been modified to manage buildInfo.xml as well.

            infoXml.ClearAllMods();
            foreach (PreinstallEntry mod in Mods)
            {
                unzipper.ExtractZip(mod.filename, "_extr", "metadata.xml");
                ModEntry metaData = new ModEntry("_extr\\metadata.xml");
                infoXml.AddMod(metaData);
                mod.modInfo = metaData;
            } // adds each user-selected filename to the metadata list.
        }

        public static List<ModEntry> getModEntries()
        {
            return new SettingsManager("_extr\\buildInfo.xml").GetInstalledMods();
        }

        public static void getConflictList(List<PreinstallEntry> Mods) // checks each mod against one another for conflicts, and adds conflicting mods to a list.
        {

            for (int i = 0; i < Mods.Count; i++)
            {
                Mods[i].ModConflicts = new List<string>();
                for (int j = 0; j < Mods.Count; j++)
                {
                    if (hasConflict(Mods[i].modInfo, Mods[j].modInfo))
                    {
                        if ((i != j) && (!Mods[i].ModConflicts.Contains(Mods[j].modInfo.Name)))
                            Mods[i].ModConflicts.Add(Mods[j].modInfo.Name);
                    }
                }//the list of names is saved to a list. The conflictingModLists' index corresponds with ModFile's index, allowing snakebite to easily look up any given mod's conflicts.
            }
        }

        public static bool hasConflict(ModEntry mod1, ModEntry mod2) //returns true as soon as a conflict is found, for the sake of speed efficiency.
        {
            foreach (ModQarEntry qarEntry in mod1.ModQarEntries) // iterate qar files from new mod
            {
                if (qarEntry.FilePath.Contains(".fpk")) continue;
                ModQarEntry conflicts = mod2.ModQarEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FilePath, qarEntry.FilePath));
                if (conflicts != null)
                {
                    return true;
                }
            }

            foreach (ModFpkEntry fpkEntry in mod1.ModFpkEntries) // iterate fpk files from new mod
            {
                ModFpkEntry conflicts = mod2.ModFpkEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FpkFile, fpkEntry.FpkFile) &&
                                                                                       Tools.CompareHashes(entry.FilePath, fpkEntry.FilePath));
                if (conflicts != null)
                {
                    return true;
                }
            }

            foreach (ModFileEntry fileEntry in mod1.ModFileEntries) // iterate external files from new mod
            {
                ModFileEntry conflicts = mod2.ModFileEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FilePath, fileEntry.FilePath));
                if (conflicts != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static void FilterModValidity(List<string> ModFiles)// checks if mods are too old for snakebite, or if snakebite is too old for mods, and whether mods were for an older version of the game.
        {
            // remove from the list if the mod is too old for snakebite or snakebite is too old for mod. ask user for input if the mod is for an older version of the game.
            // return a list of mods that Snakebite/user has OK'd

            ModEntry metaData;
            for (int i = ModFiles.Count() - 1; i >= 0; i--)
            {

                // check if mod contains metadata.xml
                metaData = Tools.ReadMetaData(ModFiles[i]);
                if (metaData == null)
                {
                    MessageBox.Show(String.Format("{0} does not contain a metadata.xml and cannot be installed.", ModFiles[i]));
                    ModFiles.RemoveAt(i);
                    continue;
                }

                // check version conflicts
                var SBVersion = ModManager.GetSBVersion();
                var MGSVersion = ModManager.GetMGSVersion();

                Version modSBVersion = new Version();
                Version modMGSVersion = new Version();
                try
                {
                    modSBVersion = metaData.SBVersion.AsVersion();
                    modMGSVersion = metaData.MGSVersion.AsVersion();
                }
                catch
                {
                    MessageBox.Show(String.Format("The selected version of {0} was created with an older version of SnakeBite and is no longer compatible, please download the latest version and try again.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ModFiles.RemoveAt(i);
                    continue;
                }

                // Check if mod requires SB update
                if (modSBVersion > SBVersion)
                {
                    MessageBox.Show(String.Format("{0} requires SnakeBite version {1} or newer. Please follow the link on the Settings page to get the latest version.", metaData.Name, metaData.SBVersion), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ModFiles.RemoveAt(i);
                    continue;
                }

                if (modSBVersion < new Version(0, 8, 0, 0)) // 0.8.0.0
                {
                    MessageBox.Show(String.Format("The selected version of {0} was created with an older version of SnakeBite and is no longer compatible, please download the latest version and try again.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ModFiles.RemoveAt(i);
                    continue;
                }

                // Check MGS version compatibility 
                 
               /* if (MGSVersion != modMGSVersion && modMGSVersion != new Version(0, 0, 0, 0))
                {
                    if (MGSVersion > modMGSVersion && modMGSVersion > new Version(0, 0, 0, 0))
                    {
                        var contInstall = MessageBox.Show(String.Format("{0} appears to be for an older version of MGSV. It is recommended that you at least check for an updated version before installing.\n\nWould you still like to install this mod?", metaData.Name), "Game version mismatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (contInstall == DialogResult.No)
                        {
                            ModFiles.RemoveAt(i);
                            continue;
                        }
                    }
                    if (MGSVersion < modMGSVersion)
                    {
                        MessageBox.Show(String.Format("{0} requires MGSV version {1}, but your installation is version {2}. Please update MGSV and try again.", metaData.Name, modMGSVersion, MGSVersion), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ModFiles.RemoveAt(i);
                       continue;
                    }
                }
                */
            }
        }

        public static void FilterModConflicts(List<string> ModFiles)//checks if the mods in the list conflict with installed mods or with the game files
        {
            // asks user for input if a conflict is found.
            //return a list of mods that the user has OK'd
            int confCounter;
            int confIndex;
            SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
            var mods = manager.GetInstalledMods();
            List<string> conflictingMods;
            ModEntry metaData;
            formModConflict conflictForm = new formModConflict();

            // search installed mods for conflicts
            for (int i = ModFiles.Count() - 1; i >= 0; i--)
            {
                metaData = Tools.ReadMetaData(ModFiles[i]);
                confCounter = 0;
                confIndex = -1;
                conflictingMods = new List<string>();

                Debug.LogLine(String.Format("[Mod] Checking conflicts for {0}", metaData.Name));

                foreach (ModEntry mod in mods) // iterate through installed mods [Morbid: TODO iterate pftxs files as well]
                {
                    foreach (ModFileEntry fileEntry in metaData.ModFileEntries) // iterate external files from new mod
                    {
                        ModFileEntry conflicts = mod.ModFileEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FilePath, fileEntry.FilePath));
                        if (conflicts != null)
                        {
                            if (confIndex == -1) confIndex = mods.IndexOf(mod);
                            if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                            Debug.LogLine(String.Format("[{0}] Conflict in 00.dat: {1}", mod.Name, conflicts.FilePath));
                            confCounter++;
                        }
                    }

                    foreach (ModQarEntry qarEntry in metaData.ModQarEntries) // iterate qar files from new mod
                    {
                        if (qarEntry.FilePath.Contains(".fpk")) continue;
                        ModQarEntry conflicts = mod.ModQarEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FilePath, qarEntry.FilePath));
                        if (conflicts != null)
                        {
                            if (confIndex == -1) confIndex = mods.IndexOf(mod);
                            if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                            Debug.LogLine(String.Format("[{0}] Conflict in 00.dat: {1}", mod.Name, conflicts.FilePath));
                            confCounter++;
                        }
                    }

                    foreach (ModFpkEntry fpkEntry in metaData.ModFpkEntries) // iterate fpk files from new mod
                    {
                        ModFpkEntry conflicts = mod.ModFpkEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FpkFile, fpkEntry.FpkFile) &&
                                                                                               Tools.CompareHashes(entry.FilePath, fpkEntry.FilePath));
                        if (conflicts != null)
                        {
                            if (confIndex == -1) confIndex = mods.IndexOf(mod);
                            if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                            Debug.LogLine(String.Format("[{0}] Conflict in {2}: {1}", mod.Name, conflicts.FilePath, Path.GetFileName(conflicts.FpkFile)));
                            confCounter++;
                        }
                    }
                }

                // if the mod conflicts, prompt user for resolution

                if (conflictingMods.Count > 0)
                {
                    Debug.LogLine(String.Format("[Mod] Found {0} conflicts", confCounter));
                    string msgboxtext = String.Format("\"{0}\" conflicts with mods that are already installed:\n", Tools.ReadMetaData(ModFiles[i]).Name);
                    foreach (string Conflict in conflictingMods)
                    {
                        msgboxtext += String.Format("\n\"{0}\"", Conflict);
                    }
                    DialogResult userInput = conflictForm.ShowDialog(msgboxtext);
                    if (userInput == DialogResult.Cancel)
                    {
                        ModFiles.RemoveAt(i);
                        continue;
                    }
                }

                Debug.LogLine("[Mod] No conflicts found");

                bool sysConflict = false;
                // check for system file conflicts
                var gameData = manager.GetGameData();
                foreach (ModQarEntry gameQarFile in gameData.GameQarEntries.FindAll(entry => entry.SourceType == FileSource.System))
                {
                    if (metaData.ModQarEntries.Count(entry => Tools.ToQarPath(entry.FilePath) == Tools.ToQarPath(gameQarFile.FilePath)) > 0) sysConflict = true;
                }

                foreach (ModFpkEntry gameFpkFile in gameData.GameFpkEntries.FindAll(entry => entry.SourceType == FileSource.System))
                {
                    if (metaData.ModFpkEntries.Count(entry => entry.FilePath == gameFpkFile.FilePath && entry.FpkFile == gameFpkFile.FpkFile) > 0) sysConflict = true;
                }
                if (sysConflict)
                {
                    //tex TODO: figure out what it's actually checking and how this can be corrupted
                    string msgboxtext = String.Format("\"{0}\" conflicts with existing MGSV system files,\n", Tools.ReadMetaData(ModFiles[i]).Name);
                    msgboxtext += "or the snakebite.xml base entries has become corrupt.\n";
                    msgboxtext += "Please use the 'Restore Backup Game Files' option in Snakebite settings and re-run snakebite\n";
                    DialogResult userInput = conflictForm.ShowDialog(msgboxtext);
                    if (userInput == DialogResult.Cancel)
                    {
                        ModFiles.RemoveAt(i);
                        continue;
                    }
                }
            }
        }

        public static bool CheckConflicts(string ModFile)
        {
            ModEntry metaData = Tools.ReadMetaData(ModFile);
            if (metaData == null) return false;
            // check version conflicts
            var SBVersion = ModManager.GetSBVersion();
            var MGSVersion = ModManager.GetMGSVersion();

            SettingsManager manager = new SettingsManager(GamePaths.SnakeBiteSettings);
            Version modSBVersion = new Version();
            Version modMGSVersion = new Version();
            try
            {
                modSBVersion = metaData.SBVersion.AsVersion();
                modMGSVersion = metaData.MGSVersion.AsVersion();
            }
            catch
            {
                MessageBox.Show(String.Format("The selected version of {0} was created with an older version of SnakeBite and is no longer compatible, please download the latest version and try again.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            // Check if mod requires SB update
            if (modSBVersion > SBVersion)
            {
                MessageBox.Show(String.Format("{0} requires SnakeBite version {1} or newer. Please follow the link on the Settings page to get the latest version.", metaData.Name, metaData.SBVersion), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (modSBVersion < new Version(0, 8, 0, 0)) // 0.8.0.0
            {
                MessageBox.Show(String.Format("The selected version of {0} was created with an older version of SnakeBite and is no longer compatible, please download the latest version and try again.", metaData.Name), "Mod update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Check MGS version compatibility
            if (!manager.IsUpToDate(modMGSVersion))
            {
                if (MGSVersion > modMGSVersion)
                {
                    var contInstall = MessageBox.Show(String.Format("{0} appears to be for an older version of MGSV. It is recommended that you check for an updated version before installing.\n\nContinue installation?", metaData.Name), "Game version mismatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (contInstall == DialogResult.No) return false;
                }
                if (MGSVersion < modMGSVersion)
                {
                    MessageBox.Show(String.Format("{0} requires MGSV version {1}, but your installation is version {2}. Please update MGSV and try again.", metaData.Name, modMGSVersion, MGSVersion), "Update required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            //end of validity checks


            Debug.LogLine(String.Format("[Mod] Checking conflicts for {0}", metaData.Name));
            int confCounter = 0;
            // search installed mods for conflicts
            var mods = manager.GetInstalledMods();
            List<string> conflictingMods = new List<string>();
            int confIndex = -1;
            foreach (ModEntry mod in mods) // iterate through installed mods
            {
                foreach (ModFileEntry fileEntry in metaData.ModFileEntries) // iterate external files from new mod
                {
                    ModFileEntry conflicts = mod.ModFileEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FilePath, fileEntry.FilePath));
                    if (conflicts != null)
                    {
                        if (confIndex == -1) confIndex = mods.IndexOf(mod);
                        if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                        Debug.LogLine(String.Format("[{0}] Conflict in 00.dat: {1}", mod.Name, conflicts.FilePath));
                        confCounter++;
                    }
                }

                foreach (ModQarEntry qarEntry in metaData.ModQarEntries) // iterate qar files from new mod
                {
                    if (qarEntry.FilePath.Contains(".fpk")) continue;
                    ModQarEntry conflicts = mod.ModQarEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FilePath, qarEntry.FilePath));
                    if (conflicts != null)
                    {
                        if (confIndex == -1) confIndex = mods.IndexOf(mod);
                        if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                        Debug.LogLine(String.Format("[{0}] Conflict in 00.dat: {1}", mod.Name, conflicts.FilePath));
                        confCounter++;
                    }
                }

                foreach (ModFpkEntry fpkEntry in metaData.ModFpkEntries) // iterate fpk files from new mod
                {
                    ModFpkEntry conflicts = mod.ModFpkEntries.FirstOrDefault(entry => Tools.CompareHashes(entry.FpkFile, fpkEntry.FpkFile) &&
                                                                                           Tools.CompareHashes(entry.FilePath, fpkEntry.FilePath));
                    if (conflicts != null)
                    {
                        if (confIndex == -1) confIndex = mods.IndexOf(mod);
                        if (!conflictingMods.Contains(mod.Name)) conflictingMods.Add(mod.Name);
                        Debug.LogLine(String.Format("[{0}] Conflict in {2}: {1}", mod.Name, conflicts.FilePath, Path.GetFileName(conflicts.FpkFile)));
                        confCounter++;
                    }
                }
            }

            // if the mod conflicts, display message

            if (conflictingMods.Count > 0)
            {
                Debug.LogLine(String.Format("[Mod] Found {0} conflicts", confCounter));
                string msgboxtext = "The selected mod conflicts with these mods:\n";
                foreach (string Conflict in conflictingMods)
                {
                    msgboxtext += Conflict + "\n";
                }
                msgboxtext += "\nMore information regarding the conflicts has been output to the logfile.";
                MessageBox.Show(msgboxtext, "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Debug.LogLine("[Mod] No conflicts found");

            bool sysConflict = false;
            // check for system file conflicts
            var gameData = manager.GetGameData();
            foreach (ModQarEntry gameQarFile in gameData.GameQarEntries.FindAll(entry => entry.SourceType == FileSource.System))
            {
                if (metaData.ModQarEntries.Count(entry => Tools.ToQarPath(entry.FilePath) == Tools.ToQarPath(gameQarFile.FilePath)) > 0) sysConflict = true;
            }

            foreach (ModFpkEntry gameFpkFile in gameData.GameFpkEntries.FindAll(entry => entry.SourceType == FileSource.System))
            {
                if (metaData.ModFpkEntries.Count(entry => entry.FilePath == gameFpkFile.FilePath && entry.FpkFile == gameFpkFile.FpkFile) > 0) sysConflict = true;
            }
            if (sysConflict)
            {
                //tex TODO: figure out what it's actually checking and how this can be corrupted
                string msgboxtext = "The selected mod conflicts with existing MGSV system files,\n";
                msgboxtext += "or the snakebite.xml base entries has become corrupt.\n";
                msgboxtext += "Please use the 'Restore Backup Game Files' option in Snakebite settings and re-run snakebite\n";
                MessageBox.Show(msgboxtext, "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
    public class PreinstallEntry
    {
        public string filename { get; set; }

        public ModEntry modInfo { get; set; }

        public List<string> ModConflicts { get; set; }
        
    }
}



