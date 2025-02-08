using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

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

            if (MessageBox.Show(
                "Приложение будет удалено.\n\nВы действительно хотите удалить приложение?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Закрываем все процессы приложения
                foreach (var process in Process.GetProcessesByName(AppName))
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch { }
                }

                // Создаем батник для удаления
                var batchPath = Path.Combine(Path.GetTempPath(), "uninstall.bat");
                var sb = new StringBuilder();
                sb.AppendLine("@echo off");
                sb.AppendLine("timeout /t 1 /nobreak > nul");
                
                // Удаляем ярлыки
                var startMenuPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs",
                    $"{AppName}.lnk");
                sb.AppendLine($"if exist \"{startMenuPath}\" del /f /q \"{startMenuPath}\"");

                var desktopPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"{AppName}.lnk");
                sb.AppendLine($"if exist \"{desktopPath}\" del /f /q \"{desktopPath}\"");

                // Удаляем лицензию
                sb.AppendLine($"if exist \"{LicenseFolderPath}\" rmdir /s /q \"{LicenseFolderPath}\"");

                // Удаляем основную папку установки
                sb.AppendLine($"if exist \"{InstallPath}\" rmdir /s /q \"{InstallPath}\"");

                // Удаляем сам батник
                sb.AppendLine("(goto) 2>nul & del \"%~f0\"");

                File.WriteAllText(batchPath, sb.ToString());

                // Запускаем батник для удаления
                var startInfo = new ProcessStartInfo
                {
                    FileName = batchPath,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(startInfo);

                MessageBox.Show(
                    "Приложение успешно удалено!",
                    "Удаление завершено",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Завершаем процесс деинсталлятора
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при удалении: {ex.Message}\n\nВозможно, некоторые компоненты не были удалены.",
                    "Ошибка удаления",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}