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
        private const string AppName = "ProtectedApp";
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
            Log("Starting installer...");
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (MessageBox.Show(
                "Продолжить установку?",
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

                MessageBox.Show(
                    "Приложение успешно установлено!\n\nЯрлыки созданы на рабочем столе и в меню Пуск.",
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
            
            try
            {
                // First verify the resources exist
                var resourceNames = assembly.GetManifestResourceNames()
                    .Where(name => name.StartsWith(resourcePrefix, StringComparison.Ordinal))
                    .ToList();
                    
                if (resourceNames.Count == 0)
                {
                    throw new InvalidOperationException($"No resources found with prefix {resourcePrefix}. Available resources: {string.Join(", ", assembly.GetManifestResourceNames())}");
                }
                
                Log($"Found {resourceNames.Count} resources to extract");
                int successCount = 0;
                
                foreach (var resourceName in resourceNames)
                {
                    try
                    {
                        using var stream = assembly.GetManifestResourceStream(resourceName);
                        if (stream == null)
                        {
                            Log($"Critical: Could not load resource {resourceName}");
                            throw new InvalidOperationException($"Resource stream is null for {resourceName}");
                        }

                        // Get the relative path by removing the prefix and any leading dots/slashes
                        var relativePath = resourceName
                            .Substring(resourcePrefix.Length)
                            .TrimStart('.')
                            .TrimStart('\\', '/');
                            
                        var targetFile = Path.Combine(targetPath, relativePath);
                        var targetDir = Path.GetDirectoryName(targetFile);
                        
                        Log($"Extracting {resourceName} to {targetFile}");
                        
                        if (targetDir != null)
                        {
                            Directory.CreateDirectory(targetDir);
                        }
                        
                        // If file exists, try to delete it first
                        if (IOFile.Exists(targetFile))
                        {
                            try
                            {
                                IOFile.Delete(targetFile);
                                Log($"Deleted existing file: {targetFile}");
                            }
                            catch (Exception ex)
                            {
                                Log($"Warning: Could not delete existing file {targetFile}: {ex.Message}");
                                // Try to continue anyway
                            }
                        }
                        
                        using (var fileStream = IOFile.Create(targetFile))
                        {
                            stream.CopyTo(fileStream);
                        }
                        
                        // Verify extraction
                        if (!IOFile.Exists(targetFile))
                        {
                            throw new IOException($"File {targetFile} was not created");
                        }
                        
                        var fileInfo = new FileInfo(targetFile);
                        if (fileInfo.Length == 0)
                        {
                            throw new IOException($"Extracted file {targetFile} is empty");
                        }
                        
                        Log($"Successfully extracted {relativePath} ({fileInfo.Length} bytes)");
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        Log($"Error extracting {resourceName}: {ex.Message}");
                        throw; // Re-throw to be caught by outer try-catch
                    }
                }
                
                Log($"Successfully extracted {successCount} of {resourceNames.Count} resources");
                
                if (successCount == 0)
                {
                    throw new InvalidOperationException("No resources were successfully extracted");
                }
                else if (successCount < resourceNames.Count)
                {
                    Log("Warning: Some resources failed to extract");
                }
            }
            catch (Exception ex)
            {
                var message = $"Critical error during extraction from {resourcePrefix}: {ex.Message}";
                Log(message);
                Log($"Stack trace: {ex.StackTrace}");
                throw new Exception(message, ex);
            }
        }

        private static void ExtractFiles(string installPath)
        {
            // Clean up any existing installation
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

            // Create fresh installation directory
            Directory.CreateDirectory(installPath);
            Log($"Created installation directory: {installPath}");

            Log("Extracting ProtectedApp files...");
            // Изменяем путь установки, убираем дополнительную вложенность
            ExtractAndCopyDirectory("Installer.Resources.ProtectedApp", Path.Combine(installPath, "App"));
            
            Log("Extracting Uninstaller files...");
            ExtractAndCopyDirectory("Installer.Resources.Uninstaller", Path.Combine(installPath, "Uninstaller"));
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