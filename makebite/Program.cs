using System;
using System.Windows.Forms;

namespace makebite
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = 437;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new formMain());
        }
    }
}