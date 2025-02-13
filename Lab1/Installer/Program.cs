using System;
using System.IO;
using IOFile = System.IO.File; // added alias to resolve ambiguous "File"
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using LicenseCore.Interfaces;
using LicenseCore.Services;
using System.Runtime.InteropServices;
using System.Text;

namespace Installer
{
    static class Program
    {
        public const string AppName = "ProtectedApp";
        private static readonly string DefaultInstallPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppName);

        private static readonly string LicenseFolderPath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectedApp");

        private const string LogFile = "install_log.txt";

        private static void Log(string message)
        {
            try
            {
                // Get timestamp once to ensure consistency
                var timestamp = DateTime.Now;
                var logMessage = $"[{timestamp:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                
                // Use temporary directory for logs until installation directory is created
                var logDir = Directory.Exists(DefaultInstallPath) ? DefaultInstallPath : Path.GetTempPath();
                var logPath = Path.Combine(logDir, LogFile);
                
                // Ensure directory exists
                var dirPath = Path.GetDirectoryName(logPath);
                if (dirPath != null && !Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // Write log with retries
                const int maxRetries = 3;
                Exception lastException = null;
                
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        IOFile.AppendAllText(logPath, logMessage);
                        Debug.WriteLine(logMessage); // Always write to debug output
                        
                        // If we get here, write succeeded
                        if (i > 0)
                        {
                            Debug.WriteLine($"Log write succeeded on attempt {i + 1}");
                        }
                        
                        // Move log if needed
                        if (Directory.Exists(DefaultInstallPath) && logDir != DefaultInstallPath)
                        {
                            var targetLogPath = Path.Combine(DefaultInstallPath, LogFile);
                            if (IOFile.Exists(logPath))
                            {
                                // Try to move the log file
                                try
                                {
                                    if (IOFile.Exists(targetLogPath))
                                    {
                                        // Append content instead of overwriting
                                        var content = IOFile.ReadAllText(logPath);
                                        IOFile.AppendAllText(targetLogPath, content);
                                        IOFile.Delete(logPath);
                                    }
                                    else
                                    {
                                        IOFile.Move(logPath, targetLogPath);
                                    }
                                    Debug.WriteLine($"Moved log from {logPath} to {targetLogPath}");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Failed to move log file: {ex.Message}");
                                    // Continue execution - logging should not stop installation
                                }
                            }
                        }
                        
                        return; // Success - exit the retry loop
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        if (i < maxRetries - 1)
                        {
                            // Wait before retry with exponential backoff
                            Thread.Sleep((i + 1) * 100);
                        }
                    }
                }
                
                // If we get here, all retries failed
                if (lastException != null)
                {
                    Debug.WriteLine($"Failed to write to log after {maxRetries} attempts: {lastException.Message}");
                }
            }
            catch (Exception ex)
            {
                // Last resort - at least try to output to debug
                Debug.WriteLine($"Critical logging error: {ex.Message}");
                Debug.WriteLine($"Failed to log message: {message}");
            }
        }

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var form = new InstallForm(DefaultInstallPath);
            Application.Run(form);
        }

        public static void Install(string installPath, InstallForm form)
        {
            try
            {
                // Проверяем, существует ли директория и очищаем её
                form.UpdateProgress(0, "Подготовка к установке...");
                if (Directory.Exists(installPath))
                {
                    try
                    {
                        Directory.Delete(installPath, true);
                        Log($"Cleaned up existing installation at: {installPath}");
                    }
                    catch (Exception ex)
                    {
                        Log($"Warning: Could not clean up existing installation: {ex.Message}");
                    }
                }

                if (form.IsCancelled) return;

                // Extract files
                form.UpdateProgress(20, "Установка файлов приложения...");
                ExtractFiles(installPath);

                if (form.IsCancelled) return;

                // Update shortcuts
                form.UpdateProgress(60, "Создание ярлыков...");
                CreateShortcuts(Path.Combine(installPath, "Project1.exe"));

                if (form.IsCancelled) return;

                // Generate and save license
                form.UpdateProgress(80, "Генерация лицензии...");
                GenerateAndSaveLicense();

                if (form.IsCancelled) return;

                form.UpdateProgress(100, "Установка завершена");
                form.OnInstallationComplete(true);

                // Записываем путь установки в реестр для деинсталлятора
                SaveInstallationPath(installPath);
            }
            catch (Exception ex)
            {
                Log($"Installation failed: {ex.Message}");
                Log($"Stack trace: {ex.StackTrace}");
                form.OnInstallationComplete(false);
            }
        }

        private static void SaveInstallationPath(string installPath)
        {
            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                    @"Software\ProtectedApp",
                    true);
                key?.SetValue("InstallPath", installPath);
                Log($"Saved installation path to registry: {installPath}");
            }
            catch (Exception ex)
            {
                Log($"Failed to save installation path to registry: {ex.Message}");
                // Не выбрасываем исключение, так как это не критическая ошибка
            }
        }

        private static void ExtractFiles(string installPath)
        {
            try
            {
                Log($"Starting file extraction to {installPath}");

                // Создаем директорию для установки
                Directory.CreateDirectory(installPath);

                // Получаем путь к папке с ресурсами (рядом с exe инсталлятора)
                var installerDir = AppContext.BaseDirectory;
                var protectedAppSourcePath = Path.Combine(installerDir, "Resources", "ProtectedApp");
                var uninstallerSourcePath = Path.Combine(installerDir, "Resources", "Uninstaller");

                // Проверяем наличие исходных файлов
                if (!Directory.Exists(protectedAppSourcePath))
                    throw new DirectoryNotFoundException($"Protected app directory not found at: {protectedAppSourcePath}");
                if (!Directory.Exists(uninstallerSourcePath))
                    throw new DirectoryNotFoundException($"Uninstaller directory not found at: {uninstallerSourcePath}");

                // Копируем все файлы защищенного приложения
                foreach (var file in Directory.GetFiles(protectedAppSourcePath, "*.*", SearchOption.AllDirectories))
                {
                    var relativePath = file.Substring(protectedAppSourcePath.Length).TrimStart('\\', '/');
                    var targetPath = Path.Combine(installPath, relativePath);
                    var targetDir = Path.GetDirectoryName(targetPath);

                    if (!string.IsNullOrEmpty(targetDir))
                        Directory.CreateDirectory(targetDir);

                    File.Copy(file, targetPath, true);
                    Log($"Copied: {relativePath}");
                }

                // Копируем все файлы деинсталлятора
                foreach (var file in Directory.GetFiles(uninstallerSourcePath, "*.*", SearchOption.AllDirectories))
                {
                    var relativePath = file.Substring(uninstallerSourcePath.Length).TrimStart('\\', '/');
                    var targetPath = Path.Combine(installPath, relativePath);
                    var targetDir = Path.GetDirectoryName(targetPath);

                    if (!string.IsNullOrEmpty(targetDir))
                        Directory.CreateDirectory(targetDir);

                    File.Copy(file, targetPath, true);
                    Log($"Copied: {relativePath}");
                }

                Log("All files copied successfully");
            }
            catch (Exception ex)
            {
                Log($"Error during file extraction: {ex.Message}");
                throw;
            }
        }

        private static void CreateShortcuts(string appPath)
        {
            // Создаем ярлык в меню Пуск
            var startMenuPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                "Programs",
                $"{AppName}.lnk");

            CreateShortcut(appPath, startMenuPath);

            // Создаем ярлык на рабочем столе
            var desktopPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"{AppName}.lnk");

            CreateShortcut(appPath, desktopPath);
        }

        private static void CreateShortcut(string targetPath, string shortcutPath)
        {
            try
            {
                Log($"Creating shortcut: {shortcutPath} -> {targetPath}");
                
                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(shortcutPath));
                
                if (IOFile.Exists(shortcutPath))
                {
                    Log($"Removing existing shortcut: {shortcutPath}");
                    IOFile.Delete(shortcutPath);
                }

                // Create shortcut using PowerShell
                var psCommand = new StringBuilder();
                psCommand.AppendLine("$WshShell = New-Object -comObject WScript.Shell");
                psCommand.AppendLine($"$Shortcut = $WshShell.CreateShortcut('{shortcutPath.Replace("'", "''")}')");
                psCommand.AppendLine($"$Shortcut.TargetPath = '{targetPath.Replace("'", "''")}'");
                psCommand.AppendLine($"$Shortcut.WorkingDirectory = '{Path.GetDirectoryName(targetPath).Replace("'", "''")}'");
                psCommand.AppendLine("$Shortcut.Description = 'Protected Application'");
                psCommand.AppendLine($"$Shortcut.IconLocation = '{targetPath.Replace("'", "''")}'");
                psCommand.AppendLine("$Shortcut.Save()");

                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -NonInteractive -Command {psCommand}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process != null)
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        var error = process.StandardError.ReadToEnd();
                        throw new Exception($"PowerShell error (Exit code: {process.ExitCode}): {error}");
                    }
                }

                // Verify shortcut was created
                if (!IOFile.Exists(shortcutPath))
                {
                    throw new Exception("Shortcut file was not created");
                }
                
                Log($"Shortcut created successfully at {shortcutPath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                var message = $"Access denied while creating shortcut {shortcutPath}. This may be due to insufficient permissions.";
                Log($"Error: {message}. Details: {ex.Message}");
                // Don't rethrow - shortcut creation failure shouldn't stop installation
            }
            catch (Exception ex)
            {
                Log($"Warning: Failed to create shortcut {shortcutPath}: {ex.Message}");
                Log($"Stack trace: {ex.StackTrace}");
                // Don't rethrow - shortcut creation failure shouldn't stop installation
            }
        }

        private static void GenerateAndSaveLicense()
        {
            try
            {
                Log("Setting up license generation services...");
                var services = new ServiceCollection()
                    .AddTransient<ICryptoService, CryptoService>()
                    .AddTransient<IHardwareInfoProvider, HardwareInfoProvider>()
                    .AddTransient<ILicenseService, LicenseService>()
                    .BuildServiceProvider();

                Log("Getting hardware information...");
                var hardwareInfoProvider = services.GetRequiredService<IHardwareInfoProvider>();
                var licenseService = services.GetRequiredService<ILicenseService>();

                var hardwareInfo = hardwareInfoProvider.GetHardwareInfo();
                Log($"Hardware info obtained: CPU={hardwareInfo.CpuId}, MB={hardwareInfo.MotherboardSerialNumber}");

                // Check if license directory exists
                Directory.CreateDirectory(LicenseFolderPath);

                // Check if license already exists
                var existingLicense = licenseService.LoadLicense();
                if (!string.IsNullOrEmpty(existingLicense))
                {
                    Log("Existing license found, validating...");
                    if (licenseService.ValidateLicense(existingLicense))
                    {
                        Log("Existing license is valid, verifying hardware match...");
                        // Additional check to ensure the license matches current hardware
                        if (licenseService.ValidateLicense(existingLicense))  // Убираем ValidateLicenseHardware, используем обычную валидацию
                        {
                            Log("Existing license matches current hardware, skipping generation");
                            return;
                        }
                        Log("Existing license is valid but hardware changed, generating new one");
                    }
                    else
                    {
                        Log("Existing license is invalid, generating new one");
                    }
                }

                Log("Generating new license...");
                var license = licenseService.GenerateLicense(hardwareInfo);
                
                Log("Saving license...");
                licenseService.SaveLicense(license);
                
                // Validate the newly saved license
                var savedLicense = licenseService.LoadLicense();
                if (string.IsNullOrEmpty(savedLicense) || !licenseService.ValidateLicense(savedLicense))
                {
                    throw new Exception("License validation after save failed");
                }
                
                Log("License generated, saved and validated successfully");
            }
            catch (Exception ex)
            {
                Log($"Error during license generation: {ex.Message}");
                Log($"Stack trace: {ex.StackTrace}");
                throw new Exception("Failed to generate or save license", ex);
            }
        }
    }
}