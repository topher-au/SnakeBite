using System;
using System.Windows.Forms;

namespace SnakeBite
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Properties.Settings.Default.Upgrade();
            Debug.Clear();

            string InitLog = String.Format(
                "SnakeBite Version: {0}\n" +
                "-------------------------\n" +
                "MGS Install Folder: {1}\n" +
                "MGS Version: {2}\n" +
                "-------------------------\n",
                ModManager.GetSBVersion(),
                Properties.Settings.Default.InstallPath,
                ModManager.GetMGSVersion());

            Debug.LogLine(InitLog, Debug.LogLevel.Basic);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new formMain());
        }
    }
}