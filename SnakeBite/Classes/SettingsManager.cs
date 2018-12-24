using SnakeBite.GzsTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using static SnakeBite.GamePaths;

namespace SnakeBite
{
    
    public class SettingsManager
    {
        internal static string vanillaDatHash = "41317C4D473D9A3DB6C1169E5ACDD35849FCF50601FD41F5A171E1055C642093"; //expected original hash for 1.0.15.0
        internal static Version IntendedGameVersion = new Version(1, 0, 15, 0); // GAMEVERSION
        
        public string xmlFilePath;

        public SettingsManager(string filePath)
        {
            xmlFilePath = filePath;
        }

        public List<string> GetModFpkFiles()
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            List<string> fpkList = new List<string>();
            foreach (ModEntry mod in settings.ModEntries)
            {
                foreach (ModFpkEntry fpkFile in mod.ModFpkEntries)
                {
                    fpkList.Add(Tools.ToQarPath(fpkFile.FilePath));
                }
            }
            return fpkList;
        }

        internal void updateQarFileNames() // snakebite supports automatically updating filenames before they're installed, but will need to update old game settings from the prior version. 1-time-check per SB update
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            HashingExtended.ReadDictionary();
            List<string> noUpdateQars = new List<string>();
            Dictionary<string, string> newNameDictionary = new Dictionary<string, string>();

            int foundUpdate = 0; 
            foreach (ModQarEntry QarEntry in settings.GameData.GameQarEntries)
            {
                if (QarEntry.FilePath.StartsWith("/Assets/")) { noUpdateQars.Add(QarEntry.FilePath);  continue; }
                string unhashedName = HashingExtended.UpdateName(QarEntry.FilePath);
                if (unhashedName != null)
                {
                    newNameDictionary.Add(QarEntry.FilePath, unhashedName);
                    foundUpdate++;

                    QarEntry.FilePath = unhashedName;
                }
                else
                {
                    noUpdateQars.Add(QarEntry.FilePath);
                }
            }
            if (foundUpdate > 0)
            {
                foreach (ModFpkEntry modFpkEntry in settings.GameData.GameFpkEntries.Where(entry => !noUpdateQars.Contains(entry.FpkFile)))
                {
                    string unHashedName;
                    if (newNameDictionary.TryGetValue(modFpkEntry.FpkFile, out unHashedName))
                        modFpkEntry.FpkFile = unHashedName;
                }
            }
            foreach (ModEntry mod in settings.ModEntries) {
                noUpdateQars.Clear();
                foreach (ModQarEntry modQar in mod.ModQarEntries)
                {
                    if (modQar.FilePath.StartsWith("/Assets/")) { noUpdateQars.Add(modQar.FilePath); continue; }
                    string unHashedName;
                    if (newNameDictionary.TryGetValue(modQar.FilePath, out unHashedName))
                        modQar.FilePath = unHashedName;
                    else
                    {
                        unHashedName = HashingExtended.UpdateName(modQar.FilePath);
                        if (unHashedName != null)
                        {
                            newNameDictionary.Add(modQar.FilePath, unHashedName);
                            modQar.FilePath = unHashedName;
                            foundUpdate++;
                        }
                        else
                        {
                            noUpdateQars.Add(modQar.FilePath);
                        }
                    }
                }
                if (foundUpdate > 0)
                {
                    foreach (ModFpkEntry modFpkEntry in mod.ModFpkEntries.Where(entry => !noUpdateQars.Contains(entry.FpkFile)))
                    {
                        string unHashedName;
                        if (newNameDictionary.TryGetValue(modFpkEntry.FpkFile, out unHashedName))
                            modFpkEntry.FpkFile = unHashedName;
                    }
                }
            }
            settings.SaveTo(xmlFilePath);
        }

        public List<string> GetModQarFiles(bool HideExtension = false)
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            List<string> qarList = new List<string>();
            foreach (ModEntry mod in settings.ModEntries)
            {
                foreach (ModQarEntry qarFile in mod.ModQarEntries)
                {
                    string fileName;
                    if (HideExtension)
                    {
                        fileName = Tools.ToQarPath(qarFile.FilePath.Substring(0, qarFile.FilePath.IndexOf(".")));
                    }
                    else
                    {
                        fileName = Tools.ToQarPath(qarFile.FilePath);
                    }
                    qarList.Add(fileName);
                }
            }
            return qarList;
        }

        public List<string> GetModExternalFiles() {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            List<string> fileList = new List<string>();
            foreach (ModEntry mod in settings.ModEntries) {
                foreach (ModFileEntry fpkFile in mod.ModFileEntries) {
                    fileList.Add(Tools.ToQarPath(fpkFile.FilePath));
                }
            }
            return fileList;
        }

        public bool SettingsExist()
        {
            return File.Exists(xmlFilePath);
        }

        public void DeleteSettings()
        {
            File.Delete(xmlFilePath);
        }

        public void AddMod(ModEntry Mod)
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);

            foreach (ModFpkEntry f in Mod.ModFpkEntries)
            {
                f.SourceType = FileSource.Mod;
                f.FpkFile = Tools.ToQarPath(f.FpkFile);
                f.FilePath = Tools.ToQarPath(f.FilePath);
            }

            foreach (ModQarEntry q in Mod.ModQarEntries)
            {
                q.SourceType = FileSource.Mod;
                q.FilePath = Tools.ToQarPath(q.FilePath);
            }

            settings.ModEntries.Add(Mod);
            settings.SaveTo(xmlFilePath);
        }

        public void RemoveMod(ModEntry Mod)
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            ModEntry remMod = settings.ModEntries.Find(entry => entry.Name == Mod.Name);
            settings.ModEntries.Remove(remMod);
            settings.SaveTo(xmlFilePath);
        }

        public List<ModEntry> GetInstalledMods()
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            return settings.ModEntries;
        }

        public void SetInstalledMods(List<ModEntry> NewModData)
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            settings.ModEntries = NewModData;
            settings.SaveTo(xmlFilePath);
        }

        public GameData GetGameData()
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            return settings.GameData;
        }

        public void SetGameData(GameData NewGameData)
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            settings.GameData = NewGameData;
            settings.SaveTo(xmlFilePath);
        }

        public Version GetSettingsSBVersion()
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            return settings.SbVersion.AsVersion();
        }

        public Version GetSettingsMGSVersion()
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            return settings.MGSVersion.AsVersion();
        }

        public void UpdateDatHash()
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
             
            // Hash 01.dat and update settings file
            string datHash = Tools.GetMd5Hash(ZeroPath) + Tools.GetMd5Hash(OnePath);
            settings.GameData.DatHash = datHash;
            Debug.LogLine(String.Format("[UpdateDatHash] Updated 00/01 dat hash to: {0}", datHash), Debug.LogLevel.All);
            settings.SaveTo(xmlFilePath);
        }

        public bool IsVanilla0001DatHash() //shouldn't be in settingsmanager
        {
            return vanillaDatHash.Equals(Tools.GetMd5Hash(ZeroPath) + Tools.GetMd5Hash(OnePath));
        }

        public bool IsVanilla0001Size() //shouldn't be in settingsmanager
        {
            var zeroSize = new System.IO.FileInfo(ZeroPath).Length;
            var oneSize = new System.IO.FileInfo(OnePath).Length;
            if (zeroSize < 495880000 && zeroSize > 495860000) // give or take 10 kb
            {
                if (oneSize < 264930000 && oneSize > 264910000)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsUpToDate(Version ModVersion) //shouldn't be in settingsmanager
        {
            bool isUpToDate = ModManager.GetMGSVersion() == ModVersion;
            bool isSpecialCase = ModVersion == new Version(0, 0, 0, 0) || ModVersion == new Version(1, 0, 14, 0); // 1.0.15.0 only affected the exe, so 1.0.14.0 mods are still up to date
            return isUpToDate || isSpecialCase;
        }

        public void ClearAllMods()
        {
            Settings settings = new Settings();
            settings.LoadFrom(xmlFilePath);
            settings.ModEntries = new List<ModEntry>();
            settings.SaveTo(xmlFilePath);
        }

        internal bool ValidateDatHash()
        {
            if (File.Exists(xmlFilePath)) {
                string datHash = Tools.GetMd5Hash(ZeroPath) + Tools.GetMd5Hash(OnePath);
                string hashOld = GetGameData().DatHash;
                if (datHash == hashOld)
                {
                    Debug.LogLine(String.Format("[ValidateDatHash] 00/01 dat hash match:\n{0} (Found Hash) == {1} (Expected Hash)", datHash, hashOld), Debug.LogLevel.All);
                    return true;
                }
                else
                {
                    Debug.LogLine(String.Format("[ValidateDatHash] 00/01 dat hash mismatch:\n{0} (Found Hash) != {1} (Expected Hash)", datHash, hashOld), Debug.LogLevel.All);
                    return false;
                }
            }
            Debug.LogLine(String.Format("[ValidateDatHash] could not find snakebite.xml"), Debug.LogLevel.All);
            return false;
        }

        // Checks the saved InstallPath variable for the existence of MGSVTPP.exe
        public bool ValidInstallPath
        {
            get
            {
                string installPath = Properties.Settings.Default.InstallPath;
                if (Directory.Exists(installPath))
                {
                    if (File.Exists(String.Format("{0}\\MGSVTPP.exe", installPath)))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    [XmlType("Settings")]
    public class Settings
    {
        [XmlElement("SbVersion")]
        public SerialVersion SbVersion { get; set; } = new SerialVersion();

        [XmlElement("MGSVersion")]
        public SerialVersion MGSVersion { get; set; } = new SerialVersion();

        [XmlElement("GameData")]
        public GameData GameData { get; set; } = new GameData();

        [XmlArray("Mods")]
        public List<ModEntry> ModEntries { get; set; } = new List<ModEntry>();

        public void SaveTo(string xmlFilePath)
        {
            // Write settings to XML
            Directory.CreateDirectory(Path.GetDirectoryName(xmlFilePath));
            using (FileStream s = new FileStream(xmlFilePath, FileMode.Create))
            {
                XmlSerializer x = new XmlSerializer(typeof(Settings), new[] { typeof(Settings) });
                foreach (ModEntry mod in ModEntries)
                {
                    mod.Description = mod.Description.Replace("\r\n", "\n");
                }
                SbVersion.Version = ModManager.GetSBVersion().ToString();
                MGSVersion.Version = ModManager.GetMGSVersion().ToString();
                x.Serialize(s, this);
            }
        }

        public void LoadFrom(string xmlFilePath)
        {
            // Load settings from XML

            if (!File.Exists(xmlFilePath))
            {
                return;
            }
            
            using (FileStream s = new FileStream(xmlFilePath, FileMode.Open))
            {
                XmlSerializer x = new XmlSerializer(typeof(Settings));
                Settings loaded = (Settings)x.Deserialize(s);
                GameData = loaded.GameData;
                ModEntries = loaded.ModEntries;
                SbVersion = loaded.SbVersion;
                foreach (ModEntry mod in ModEntries)
                {
                    mod.Description = mod.Description.Replace("\n", "\r\n");
                }
            }
            return;
        }
    }

    [XmlType("GameData")]
    public class GameData
    {
        public GameData()
        {
            GameQarEntries = new List<ModQarEntry>();
            GameFpkEntries = new List<ModFpkEntry>();
            GameFileEntries = new List<ModFileEntry>();
        }

        [XmlAttribute("DatHash")]
        public string DatHash { get; set; }

        //Entries of files in mod qar (ex 00,01.dat)
        [XmlArray("QarEntries")]
        public List<ModQarEntry> GameQarEntries { get; set; } = new List<ModQarEntry>();

        //Entries of files inside fpks
        [XmlArray("FpkEntries")]
        public List<ModFpkEntry> GameFpkEntries { get; set; } = new List<ModFpkEntry>();

        //Entries of files in GameDir (ex MGS_TPP)
        [XmlArray("FileEntries")]
        public List<ModFileEntry> GameFileEntries { get; set; } = new List<ModFileEntry>();
    }

    [XmlType("ModEntry")]
    public class ModEntry
    {
        public ModEntry()
        {
        }

        public ModEntry(string SourceFile)
        {
            ReadFromFile(SourceFile);
        }

        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlElement("MGSVersion")]
        public SerialVersion MGSVersion { get; set; } = new SerialVersion();

        [XmlElement("SBVersion")]
        public SerialVersion SBVersion { get; set; } = new SerialVersion();

        [XmlAttribute("Author")]
        public string Author { get; set; } = string.Empty;

        [XmlAttribute("Website")]
        public string Website { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;

        [XmlArray("QarEntries")]
        public List<ModQarEntry> ModQarEntries { get; set; } = new List<ModQarEntry>();

        [XmlArray("FpkEntries")]
        public List<ModFpkEntry> ModFpkEntries { get; set; } = new List<ModFpkEntry>();

        [XmlArray("FileEntries")]
        public List<ModFileEntry> ModFileEntries { get; set; } = new List<ModFileEntry>();

        public void ReadFromFile(string Filename)
        {
            // Read mod metadata from xml

            if (!File.Exists(Filename)) return;

            XmlSerializer x = new XmlSerializer(typeof(ModEntry));
            StreamReader s = new StreamReader(Filename);
            System.Xml.XmlReader xr = System.Xml.XmlReader.Create(s);

            ModEntry loaded = (ModEntry)x.Deserialize(xr);

            Name = loaded.Name;
            Version = loaded.Version;
            MGSVersion = loaded.MGSVersion;
            SBVersion = loaded.SBVersion;
            Author = loaded.Author;
            Website = loaded.Website;
            Description = loaded.Description;

            ModQarEntries = loaded.ModQarEntries;
            ModFpkEntries = loaded.ModFpkEntries;
            ModFileEntries = loaded.ModFileEntries;

            s.Close();
        }

        public void SaveToFile(string Filename)
        {
            // Write mod metadata to XML

            if (File.Exists(Filename)) File.Delete(Filename);

            XmlSerializer x = new XmlSerializer(typeof(ModEntry), new[] { typeof(ModEntry) });
            StreamWriter s = new StreamWriter(Filename);
            x.Serialize(s, this);
            s.Close();
        }
    }

    public enum FileSource
    {
        System,
        Merged,
        Mod
    }

    [XmlType("QarEntry")]
    public class ModQarEntry
    {
        [XmlAttribute("Hash")]
        public ulong Hash { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("Compressed")]
        public bool Compressed { get; set; }

        //Added by makebite, currently unused
        [XmlAttribute("ContentHash")]
        public string ContentHash { get; set; }

        [XmlAttribute("SourceType")]
        public FileSource SourceType { get; set; }

        [XmlAttribute("SourceName")]
        public string SourceName { get; set; }
    }

    [XmlType("FpkEntry")]
    public class ModFpkEntry
    {
        [XmlAttribute("FpkFile")]
        public string FpkFile { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        //Added by makebite, currently unused
        [XmlAttribute("ContentHash")]
        public string ContentHash { get; set; }

        //TODO: currently inaccurate
        [XmlAttribute("SourceType")]
        public FileSource SourceType { get; set; }

        [XmlAttribute("SourceName")]
        public string SourceName { get; set; }
    }

    [XmlType("FileEntry")]
    public class ModFileEntry {
        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("ContentHash")]
        public string ContentHash { get; set; }
    }
}