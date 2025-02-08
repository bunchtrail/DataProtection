using System;
using System.IO;
using System.Windows.Forms;

namespace Uninstaller
{
    static class Program
    {
        private const string AppName = "ProtectedApp";
        private static readonly string InstallPath = Path.GetDirectoryName(
            AppContext.BaseDirectory);
        private static readonly string LicenseFolderPath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectedApp");

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var form = new UninstallForm(InstallPath, LicenseFolderPath);
            Application.Run(form);
            if (form.DialogResult == DialogResult.OK)
            {
                Environment.Exit(0);
            }
        }
    }
}