using System;
using System.IO;
using IOFile = System.IO.File; // added alias to resolve ambiguous "File"
using System.Security.Principal;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using LicenseCore.Interfaces;
using LicenseCore.Services;
using IWshRuntimeLibrary; // Added to create proper shortcuts

namespace Installer
{
    static class Program
    {
        private const string AppName = "ProtectedApp";
        private static readonly string DefaultInstallPath = Path.Combine(
            IsAdministrator() ? 
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) : 
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppName);

        private static readonly string LicenseFolderPath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectedApp");

        private const string AppResourceName = "Installer.Resources.ProtectedApp.exe";
        private const string UninstallerResourceName = "Installer.Resources.Uninstaller.exe";
        private const string LogFile = "install_log.txt";

        private static void Log(string message)
        {
            try
            {
                // Используем временную директорию для логов до создания директории установки
                var logDir = Directory.Exists(DefaultInstallPath) ? DefaultInstallPath : Path.GetTempPath();
                var logPath = Path.Combine(logDir, LogFile);
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                
                // Создаём директорию для лога, если её нет
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                
                IOFile.AppendAllText(logPath, logMessage);
                Debug.WriteLine(logMessage);

                // Если директория установки создана, но лог во временной папке - переместим его
                if (Directory.Exists(DefaultInstallPath) && logDir != DefaultInstallPath)
                {
                    var targetLogPath = Path.Combine(DefaultInstallPath, LogFile);
                    if (IOFile.Exists(logPath))
                    {
                        IOFile.Copy(logPath, targetLogPath, true);
                        IOFile.Delete(logPath);
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки логирования
                Debug.WriteLine($"Ошибка при логировании: {message}");
            }
        }

        [STAThread]
        static void Main()
        {
            Log("Starting installer...");
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var isAdmin = IsAdministrator();
            var message = isAdmin ? 
                "Установка будет выполнена с правами администратора.\nПриложение получит полный доступ к информации об оборудовании." :
                "Установка будет выполнена без прав администратора.\nНекоторые функции приложения будут ограничены.";

            if (MessageBox.Show(
                message + "\n\nПродолжить установку?",
                "Подтверждение установки",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information) != DialogResult.Yes)
            {
                return;
            }

            var installPath = DefaultInstallPath;
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Выберите папку для установки приложения";
                folderDialog.UseDescriptionForTitle = true;
                folderDialog.SelectedPath = Path.GetDirectoryName(installPath);

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    installPath = Path.Combine(folderDialog.SelectedPath, AppName);
                }
                else
                {
                    return;
                }
            }

            try
            {
                // Создаем директорию для установки
                Directory.CreateDirectory(installPath);
                Log($"Created installation directory: {installPath}");

                // Extract all files
                ExtractFiles(installPath);

                // Update shortcuts to point to the new paths
                CreateShortcuts(Path.Combine(installPath, "App", "ProtectedApp.exe"));

                // Генерируем и сохраняем лицензию
                GenerateAndSaveLicense();

                var successMessage = isAdmin ?
                    "Приложение успешно установлено с полными правами!\n\nЯрлыки созданы на рабочем столе и в меню Пуск." :
                    "Приложение успешно установлено в пользовательском режиме!\n\nЯрлыки созданы на рабочем столе и в меню Пуск.\n\nПримечание: некоторые функции будут ограничены.";

                MessageBox.Show(
                    successMessage,
                    "Установка завершена",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при установке: {ex.Message}",
                    "Ошибка установки",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void ExtractAndCopyFile(string resourceName, string targetPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Log($"Looking for resource: {resourceName}");
            
            var availableResources = assembly.GetManifestResourceNames();
            Log($"Available resources: {string.Join(", ", availableResources)}");
            
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception($"Не удалось найти встроенный ресурс: {resourceName}");
                }

                var fileName = resourceName.Split('.').Last();
                var targetFile = Path.Combine(targetPath, fileName);
                Log($"Extracting {resourceName} to {targetFile}");
                
                using (var fileStream = IOFile.Create(targetFile))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        private static void ExtractAndCopyDirectory(string resourcePrefix, string targetPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Log($"Looking for resources with prefix: {resourcePrefix}");
            
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(name => name.StartsWith(resourcePrefix))
                .ToList();
                
            Log($"Found {resourceNames.Count} resources");
            
            foreach (var resourceName in resourceNames)
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    Log($"Warning: Could not load resource {resourceName}");
                    continue;
                }

                // Convert resource name to relative path by removing the prefix
                var relativePath = resourceName.Substring(resourcePrefix.Length).TrimStart('.');
                var targetFile = Path.Combine(targetPath, relativePath);
                
                Log($"Extracting {resourceName} to {targetFile}");
                
                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                
                using (var fileStream = IOFile.Create(targetFile))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        private static void ExtractFiles(string installPath)
        {
            Log("Extracting ProtectedApp files...");
            ExtractAndCopyDirectory("Installer.Resources.ProtectedApp", Path.Combine(installPath, "App"));
            
            Log("Extracting Uninstaller files...");
            ExtractAndCopyDirectory("Installer.Resources.Uninstaller", Path.Combine(installPath, "Uninstaller"));
        }

        private static void CreateShortcuts(string appPath)
        {
            var isAdmin = IsAdministrator();

            // Создаем ярлык в меню Пуск
            var startMenuPath = Path.Combine(
                Environment.GetFolderPath(isAdmin ? Environment.SpecialFolder.CommonStartMenu : Environment.SpecialFolder.StartMenu),
                "Programs",
                $"{AppName}.lnk");

            CreateShortcut(appPath, startMenuPath);

            // Создаем ярлык на рабочем столе
            var desktopPath = Path.Combine(
                Environment.GetFolderPath(isAdmin ? Environment.SpecialFolder.CommonDesktopDirectory : Environment.SpecialFolder.Desktop),
                $"{AppName}.lnk");

            CreateShortcut(appPath, desktopPath);
        }

        private static void CreateShortcut(string targetPath, string shortcutPath)
        {
            try
            {
                // Create a shortcut using Windows Script Host COM API.
                var shell = new WshShell();
                var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Description = "Shortcut for ProtectedApp";
                shortcut.IconLocation = targetPath;
                shortcut.Save();
            }
            catch
            {
                // Игнорируем ошибки создания ярлыков
            }
        }

        private static void GenerateAndSaveLicense()
        {
            var services = new ServiceCollection()
                .AddTransient<ICryptoService, CryptoService>()
                .AddTransient<IHardwareInfoProvider, HardwareInfoProvider>()
                .AddTransient<ILicenseService, LicenseService>()
                .BuildServiceProvider();

            var hardwareInfoProvider = services.GetRequiredService<IHardwareInfoProvider>();
            var licenseService = services.GetRequiredService<ILicenseService>();

            var hardwareInfo = hardwareInfoProvider.GetHardwareInfo();
            var license = licenseService.GenerateLicense(hardwareInfo);
            licenseService.SaveLicense(license);
        }

        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}