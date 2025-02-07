using System;
using System.IO;
using System.Security.Principal;
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

            var isAdmin = IsAdministrator();
            var message = isAdmin ?
                "Приложение будет полностью удалено из системы." :
                "Приложение будет удалено, но некоторые файлы могут остаться в системе из-за отсутствия прав администратора.";

            if (MessageBox.Show(
                message + "\n\nВы действительно хотите удалить приложение?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Закрываем все процессы приложения
                foreach (var process in System.Diagnostics.Process.GetProcessesByName(AppName))
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch { }
                }

                // Удаляем файлы приложения
                if (Directory.Exists(InstallPath))
                {
                    try
                    {
                        Directory.Delete(InstallPath, true);
                    }
                    catch (UnauthorizedAccessException) when (!isAdmin)
                    {
                        MessageBox.Show(
                            "Некоторые файлы не могут быть удалены без прав администратора.",
                            "Предупреждение",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }

                // Удаляем файл лицензии и папку в AppData
                if (Directory.Exists(LicenseFolderPath))
                {
                    Directory.Delete(LicenseFolderPath, true);
                }

                // Удаляем ярлыки
                DeleteShortcuts(isAdmin);

                var successMessage = isAdmin ?
                    "Приложение успешно удалено!" :
                    "Приложение удалено. Некоторые файлы могли остаться в системе.";

                MessageBox.Show(
                    successMessage,
                    "Удаление завершено",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
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

        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void DeleteShortcuts(bool isAdmin)
        {
            try
            {
                // Удаляем ярлык из меню Пуск
                var startMenuPath = Path.Combine(
                    Environment.GetFolderPath(isAdmin ? Environment.SpecialFolder.CommonStartMenu : Environment.SpecialFolder.StartMenu),
                    "Programs",
                    $"{AppName}.lnk");

                if (File.Exists(startMenuPath))
                {
                    File.Delete(startMenuPath);
                }

                // Удаляем ярлык с рабочего стола
                var desktopPath = Path.Combine(
                    Environment.GetFolderPath(isAdmin ? Environment.SpecialFolder.CommonDesktopDirectory : Environment.SpecialFolder.Desktop),
                    $"{AppName}.lnk");

                if (File.Exists(desktopPath))
                {
                    File.Delete(desktopPath);
                }
            }
            catch
            {
                // Игнорируем ошибки удаления ярлыков
            }
        }
    }
} 