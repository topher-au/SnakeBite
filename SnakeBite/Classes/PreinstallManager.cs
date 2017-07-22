using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBite
{ //for use with formInstallOrder to preview mod info and check for conflicts prior to installation
    internal static class PreinstallManager
    {
        public static void RefreshXml(List<string> ModFiles)
        {
            FastZip unzipper = new FastZip();
            SettingsManager infoXml = new SettingsManager("_extr", "buildInfo.xml");

            infoXml.ClearAllMods();
            foreach (string ModFile in ModFiles) //check current list of file names, add and remove the items in the xml
            {
                unzipper.ExtractZip(ModFile, "_extr", "metadata.xml");
                ModEntry metaData = new ModEntry("_extr\\metadata.xml");
                infoXml.AddMod(metaData);
            }
        }

        public static List<ModEntry> getModEntries()
        {
            return new SettingsManager("_extr", "buildInfo.xml").GetInstalledMods();
        }

        public static List<List<string>> getConflictList(List<ModEntry> ModFiles)
        { //returns a list of lists. each contained list has modnames that conflict with the mod at i.
            List<List<string>> conflictingModLists= new List<List<string>>();

            for (int i = 0; i < ModFiles.Count; i++)
            {
                conflictingModLists.Add(new List<string>());
                for (int j = 0; j < ModFiles.Count; j++)
                {
                    if (hasConflict(ModFiles[i], ModFiles[j]))
                    {
                        if ((i != j) && (!conflictingModLists[i].Contains(ModFiles[j].Name)))
                            conflictingModLists[i].Add(ModFiles[j].Name);
                    } 
                }
            }
            /*string itemlist = "";
            foreach (List<string> conflictlist in conflictingModLists)
            {
                foreach (string modname in conflictlist)
                {
                    itemlist += modname + " ";
                }
                MessageBox.Show(itemlist);
                itemlist = "";
            }*/
            
            return conflictingModLists;
        }
        public static bool hasConflict(ModEntry mod1, ModEntry mod2)
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

    }
}
