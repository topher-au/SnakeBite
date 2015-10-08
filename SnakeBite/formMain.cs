using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using SnakeBite.GzsTool;

namespace SnakeBite
{
    public partial class formMain : Form
    {
        private Settings mods = new Settings();

        public formMain()
        {
            InitializeComponent();
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            LoadInstalledMods();
            this.Show();

            // check if user has specified valid install path
            if (!ModManager.ValidInstallPath)
            {
                MessageBox.Show("Please locate your MGSV installation to continue.", "SnakeBite", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tabControl.SelectedIndex = 1;
                buttonFindMGSV_Click(null, null);
                if (!ModManager.ValidInstallPath)
                    Application.Exit();
            }

            textInstallPath.Text = Properties.Settings.Default.InstallPath;
        }

        private void LoadInstalledMods()
        {
            if (File.Exists("settings.xml"))
            {
                mods.LoadSettings();
            }
            else
            {
                mods.ModEntries = new List<ModEntry>();
            }

            listInstalledMods.Items.Clear();
            foreach (ModEntry mod in mods.ModEntries)
            {
                listInstalledMods.Items.Add(mod.Name);
            }
        }

        private void buttonUninstallMod_Click(object sender, EventArgs e)
        {
            ModEntry mod = mods.ModEntries[listInstalledMods.SelectedIndex];
            UninstallMod(mod);
        }

        private void buttonInstallMod_Click(object sender, EventArgs e)
        {
            OpenFileDialog openModFile = new OpenFileDialog();
            openModFile.Filter = "MGSV Mod Files|*.mgsv|All Files|*.*";
            DialogResult ofdResult = openModFile.ShowDialog();

            if (ofdResult != DialogResult.OK) return;

            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(openModFile.FileName, ".", "metadata.xml");

            StreamReader metaReader = new StreamReader("metadata.xml");
            XmlSerializer xS = new XmlSerializer(typeof(ModEntry));
            ModEntry modMeta = (ModEntry)xS.Deserialize(metaReader);
            metaReader.Close();
            File.Delete("metadata.xml");

            // search installed mods for conflicts
            List<string> conflictingMods = new List<string>();

            foreach (ModEntry modEntry in mods.ModEntries) // iterate through installed mods
            {
                foreach (ModQarEntry qarEntry in modMeta.ModQarEntries) // iterate qar files from new mod
                {
                    ModQarEntry conflicts = modEntry.ModQarEntries.FirstOrDefault(entry => entry.FilePath == qarEntry.FilePath);
                    if (conflicts != null)
                    {
                        conflictingMods.Add(modEntry.Name);
                        break;
                    }
                }

                foreach (ModFpkEntry fpkEntry in modMeta.ModFpkEntries) // iterate fpk files from new mod
                {
                    ModFpkEntry conflicts = modEntry.ModFpkEntries.FirstOrDefault(entry => entry.FpkFile == fpkEntry.FpkFile && entry.FilePath == fpkEntry.FilePath);
                    if (conflicts != null)
                    {
                        if (!conflictingMods.Contains(modEntry.Name))
                        {
                            conflictingMods.Add(modEntry.Name);
                            break;
                        }
                    }
                }
            }

            // if the mod conflicts, display message
            if (conflictingMods.Count > 0)
            {
                string msgboxtext = "The selected mod conflicts with these mods:\n";
                foreach (string Conflict in conflictingMods)
                {
                    msgboxtext += Conflict + "\n";
                }
                msgboxtext += "\nPlease uninstall the mods above and try again.";
                MessageBox.Show(msgboxtext, "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            InstallMod(openModFile.FileName);

            // Install mod
            mods.ModEntries.Add(modMeta);
            mods.SaveSettings();
            LoadInstalledMods();
            listInstalledMods.SelectedIndex = listInstalledMods.Items.Count - 1;
        }

        private void listInstalledMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listInstalledMods.SelectedIndex >= 0)
            {
                labelModName.Text = mods.ModEntries[listInstalledMods.SelectedIndex].Name;
            }
        }

        public void InstallMod(string ModFile)
        {
            string DatPath = Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat";
            string XmlPath = Properties.Settings.Default.InstallPath + "\\master\\0\\01.dat.xml";
            string FilesPath = Properties.Settings.Default.InstallPath + "\\master\\0\\01";

            if (!ModManager.ValidInstallPath) return;

            if (!File.Exists(DatPath))
            {
                return;
            }

            // extract existing DAT file
            GzsTool.GzsTool.Run(DatPath);

            // import existing DAT xml
            QarFile qarXml = new QarFile();
            qarXml.LoadFromFile(XmlPath);

            // check for unknown files in DAT
            foreach (QarEntry gzsQar in qarXml.QarEntries)
            {
                if (mods.ModEntries.FindAll(entry => entry.ModQarEntries.FindAll(f => f.FilePath == gzsQar.FilePath).Count > 0).Count > 0)
                {
                    // no unknown files found
                }
                else
                {
                    // unknown files found
                    DialogResult continueAnyway = MessageBox.Show("Your installation appears to have been modified outside of SnakeBite. Continuing may produce unpredictable results. It is recommended you make a backup (Settings -> Backup) before continuing.\n\nDo you wish to proceed?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (continueAnyway == DialogResult.No) return;
                    break;
                }
            }

            // extract mod files to temp folder
            if (Directory.Exists("_temp")) Directory.Delete("_temp", true);
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(ModFile, "_temp", "(.*?)");

            // load mod metadata
            StreamReader metaReader = new StreamReader("_temp\\metadata.xml");
            XmlSerializer xS = new XmlSerializer(typeof(ModEntry));
            ModEntry modMeta = (ModEntry)xS.Deserialize(metaReader);
            metaReader.Close();
            File.Delete("_temp\\metadata.xml");

            // check for fpk merges
            List<ModFpkEntry> installedFpkFiles = new List<ModFpkEntry>();
            foreach (QarEntry gzsQar in qarXml.QarEntries)
            {
                string qarExt = gzsQar.FilePath.Substring(gzsQar.FilePath.LastIndexOf(".") + 1).ToLower();
                if (qarExt == "fpk" || qarExt == "fpkd")
                {
                    // extract FPK and add files to list
                    string fpkFile = FilesPath + "\\" + gzsQar.FilePath;
                    string fpkDir = fpkFile.Replace(".", "_");
                    GzsTool.GzsTool.Run(fpkFile);

                    List<string> fpkFiles = Directory.GetFiles(fpkDir, "*.*", SearchOption.AllDirectories).ToList();
                    foreach (string file in fpkFiles)
                    {
                        string fpkFilePath = file.Substring(file.LastIndexOf("Assets\\"));
                        fpkFilePath = "/" + fpkFilePath.Replace("\\", "/");

                        string fpkPath = fpkFile.Substring(fpkFile.LastIndexOf("Assets\\"));
                        fpkPath = "/" + fpkPath.Replace("\\", "/");

                        installedFpkFiles.Add(new ModFpkEntry() { FilePath = fpkFilePath, FpkFile = fpkPath });
                    }
                }
            }

            // compare lists and build merge fpk list
            List<string> mergeFpks = new List<string>();
            foreach(ModFpkEntry installedFpk in installedFpkFiles)
            {
                foreach(ModEntry mod in mods.ModEntries)
                {
                    if(mod.ModFpkEntries.Find(entry => entry.FpkFile == installedFpk.FpkFile) != null)
                    {
                        if(!mergeFpks.Contains(installedFpk.FpkFile))
                        {
                            mergeFpks.Add(installedFpk.FpkFile);
                        }
                    }
                }
            }

            if(mergeFpks.Count > 0)
            {
                // merge fpks
                foreach (string fpkFile in mergeFpks)
                {
                    
                    string fpkPath = fpkFile.Substring(1).Replace("/", "\\");
                    string gameFpkPath = Properties.Settings.Default.InstallPath + "\\master\\0\\01\\" + fpkPath;
                    string gameFpkDir = Properties.Settings.Default.InstallPath + "\\master\\0\\01\\" + fpkPath.Replace(".","_");
                    string modFpkPath = "_temp\\" + fpkPath;
                    string modFpkDir = "_temp\\" + fpkPath.Replace(".", "_");

                    GzsTool.GzsTool.Run(gameFpkPath);
                    GzsTool.GzsTool.Run(modFpkPath);

                    // load existing xml data
                    FpkFile fpkXml = new FpkFile();
                    fpkXml.LoadFromFile(gameFpkPath + ".xml");

                    // generate list of files to move and add to xml
                    List<string> filesToMove = new List<string>();
                    foreach (ModFpkEntry file in modMeta.ModFpkEntries.FindAll(entry => entry.FpkFile == fpkFile))
                    {
                        filesToMove.Add(file.FilePath.Replace("/","\\"));
                        if(fpkXml.FpkEntries.Count(entry => entry.FilePath == file.FilePath) == 0)
                        {
                            // insert new fpk entries as required
                            fpkXml.FpkEntries.Add(new FpkEntry() { FilePath = fpkFile });
                        }
                    }

                    // create directories and move files
                    foreach (string file in filesToMove)
                    {
                        string fileDir = (gameFpkDir + file).Substring(0, (gameFpkDir + file).LastIndexOf("\\"));
                        if(!Directory.Exists(fileDir))
                        {
                            Directory.CreateDirectory(fileDir);
                        }
                        File.Copy(modFpkDir + file, gameFpkDir + file, true);
                    }

                    fpkXml.WriteToFile(gameFpkPath + ".xml");
                    GzsTool.GzsTool.Run(gameFpkPath + ".xml");
                }
            }

            // copy files for new DAT
            foreach (ModQarEntry modQarFile in modMeta.ModQarEntries )
            {
                string fileName = "/" + modQarFile.FilePath.Replace("\\","/");
                string fileDir = (Properties.Settings.Default.InstallPath + "\\master\\0\\01\\" + modQarFile.FilePath).Substring(0, (Properties.Settings.Default.InstallPath + "\\master\\0\\01\\" + modQarFile.FilePath).LastIndexOf("\\"));

                // if file is not already in QAR, add it
                if (qarXml.QarEntries.Count(entry => entry.FilePath == modQarFile.FilePath) == 0)
                {
                    qarXml.QarEntries.Add(new QarEntry() { FilePath = modQarFile.FilePath, Compressed = modQarFile.Compressed, Hash = modQarFile.Hash });
                }
                   
                // copy all files that weren't merged FPKS
                    if (!mergeFpks.Contains(fileName))
                    {
                        if (!Directory.Exists(fileDir))
                        {
                            Directory.CreateDirectory(fileDir);
                        }
                        File.Copy("_temp\\" + modQarFile.FilePath, Properties.Settings.Default.InstallPath + "\\master\\0\\01\\" + modQarFile.FilePath, true);

                    }
                
            }

            // build XML for new DAT
            qarXml.WriteToFile(XmlPath);

            // build new DAT
            GzsTool.GzsTool.Run(XmlPath);

            // remove temp files
            Directory.Delete("_temp", true);
        }

        public void UninstallMod(ModEntry mod)
        {
            mods.ModEntries.Remove(mod);
            mods.SaveSettings();
            LoadInstalledMods();
        }

        private void buttonFindMGSV_Click(object sender, EventArgs e)
        {
            OpenFileDialog findMGSV = new OpenFileDialog();
            findMGSV.Filter = "Metal Gear Solid V|MGSVTPP.exe";
            DialogResult findResult = findMGSV.ShowDialog();
            if (findResult != DialogResult.OK) return;

            string filePath = findMGSV.FileName.Substring(0, findMGSV.FileName.LastIndexOf("\\"));
            textInstallPath.Text = filePath;
            Properties.Settings.Default.InstallPath = filePath;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModManager.BuildGameData();
        }
    }
}