using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using Microsoft.Win32;

namespace Uninstaller
{
    static class Program
    {
        private const string AppName = "ProtectedApp";
        private static readonly string InstallPath = GetInstallPath();
        private static readonly string LicenseFolderPath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectedApp");

        private static string GetInstallPath()
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\ProtectedApp");
                var path = key?.GetValue("InstallPath") as string;
                return path ?? Path.GetDirectoryName(AppContext.BaseDirectory);
            }
            catch
            {
                return Path.GetDirectoryName(AppContext.BaseDirectory);
            }
        }

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
                return;
            }

            try
            {
                // ��������� ��� �������� ����������
                foreach (var process in Process.GetProcessesByName("Project1"))
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch { }
                }

                // ������� ������ ��� ��������
                var batchPath = Path.Combine(Path.GetTempPath(), "uninstall.bat");
                var sb = new StringBuilder();
                sb.AppendLine("@echo off");
                sb.AppendLine("timeout /t 1 /nobreak > nul");
                
                // ������� ������
                var startMenuPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs",
                    $"{AppName}.lnk");
                sb.AppendLine($"if exist \"{startMenuPath}\" del /f /q \"{startMenuPath}\"");

                var desktopPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"{AppName}.lnk");
                sb.AppendLine($"if exist \"{desktopPath}\" del /f /q \"{desktopPath}\"");

                // ������� ��������
                sb.AppendLine($"if exist \"{LicenseFolderPath}\" rmdir /s /q \"{LicenseFolderPath}\"");

                // ������� �������� ����� ���������
                sb.AppendLine($"if exist \"{InstallPath}\" rmdir /s /q \"{InstallPath}\"");

                // ������� ������ �� �������
                sb.AppendLine("reg delete \"HKCU\\Software\\ProtectedApp\" /f");

                // ������� ��� ������
                sb.AppendLine("(goto) 2>nul & del \"%~f0\"");

                File.WriteAllText(batchPath, sb.ToString());

                // ��������� ������ ��� ��������
                var startInfo = new ProcessStartInfo
                {
                    FileName = batchPath,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(startInfo);

                MessageBox.Show(
                    "���������� ������� �������!",
                    "�������� ���������",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // ��������� ������� ��������������
                Environment.Exit(0);
            } catch
            {
                
            }
        }
    }
}