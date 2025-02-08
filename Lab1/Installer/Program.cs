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
                // Создаем директорию для установки
                form.UpdateProgress(0, "Подготовка к установке...");
                Directory.CreateDirectory(installPath);
                Log($"Created installation directory: {installPath}");

                if (form.IsCancelled) return;

                // Extract files
                form.UpdateProgress(20, "Распаковка файлов приложения...");
                ExtractFiles(installPath);

                if (form.IsCancelled) return;

                // Update shortcuts
                form.UpdateProgress(60, "Создание ярлыков...");
                CreateShortcuts(Path.Combine(installPath, "App", "ProtectedApp.exe"));

                if (form.IsCancelled) return;

                // Generate and save license
                form.UpdateProgress(80, "Генерация лицензии...");
                GenerateAndSaveLicense();

                if (form.IsCancelled) return;

                form.UpdateProgress(100, "Установка завершена");
                form.OnInstallationComplete(true);
            }
            catch (Exception ex)
            {
                Log($"Installation failed: {ex.Message}");
                Log($"Stack trace: {ex.StackTrace}");
                form.OnInstallationComplete(false);
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

            // Create subdirectories
            var appPath = Path.Combine(installPath, "App");
            var uninstallerPath = Path.Combine(installPath, "Uninstaller");
            Directory.CreateDirectory(appPath);
            Directory.CreateDirectory(uninstallerPath);

            Log("Extracting ProtectedApp files...");
            ExtractAndCopyDirectory("Installer.Resources.ProtectedApp", appPath);
            
            Log("Extracting Uninstaller files...");
            ExtractAndCopyDirectory("Installer.Resources.Uninstaller", uninstallerPath);

            // Copy LicenseCore and its dependencies to both App and Uninstaller directories
            Log("Copying LicenseCore and dependencies...");
            var licenseCoreDll = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LicenseCore.dll");
            if (File.Exists(licenseCoreDll))
            {
                File.Copy(licenseCoreDll, Path.Combine(appPath, "LicenseCore.dll"), true);
                File.Copy(licenseCoreDll, Path.Combine(uninstallerPath, "LicenseCore.dll"), true);
                Log("Copied LicenseCore.dll");
            }

            // Copy required dependencies
            var dependencies = new[]
            {
                "Microsoft.Extensions.DependencyInjection.dll",
                "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
                "System.Management.dll"
            };

            foreach (var dependency in dependencies)
            {
                var sourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dependency);
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, Path.Combine(appPath, dependency), true);
                    File.Copy(sourcePath, Path.Combine(uninstallerPath, dependency), true);
                    Log($"Copied {dependency}");
                }
                else
                {
                    Log($"Warning: Dependency {dependency} not found");
                }
            }

            // Copy runtime folders if they exist
            var runtimesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "runtimes");
            if (Directory.Exists(runtimesPath))
            {
                CopyDirectory(runtimesPath, Path.Combine(appPath, "runtimes"));
                CopyDirectory(runtimesPath, Path.Combine(uninstallerPath, "runtimes"));
                Log("Copied runtime dependencies");
            }

            // Copy config files
            foreach (var configFile in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.config"))
            {
                var fileName = Path.GetFileName(configFile);
                File.Copy(configFile, Path.Combine(appPath, fileName), true);
                File.Copy(configFile, Path.Combine(uninstallerPath, fileName), true);
                Log($"Copied config file: {fileName}");
            }

            Log("All files copied successfully");
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Copy all files
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destinationDir, fileName);
                File.Copy(file, destFile, true);
            }

            // Copy all subdirectories
            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                string destDir = Path.Combine(destinationDir, dirName);
                CopyDirectory(dir, destDir);
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