using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using LicenseCore.Interfaces;
using LicenseCore.Services;
using System.IO;
using System.Diagnostics;

namespace ProtectedApp
{
    static class Program
    {
        private const string LogFile = "app_log.txt";

        private static void Log(string message)
        {
            try
            {
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFile);
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                File.AppendAllText(logPath, logMessage);
                Debug.WriteLine(logMessage);
            }
            catch
            {
                // Игнорируем ошибки логирования
            }
        }

        [STAThread]
        static void Main()
        {
            try
            {
                Log("Запуск приложения");
                Log($"Рабочая директория: {AppDomain.CurrentDomain.BaseDirectory}");

                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Log("Настройка сервисов");
                var services = new ServiceCollection();
                ConfigureServices(services);

                using var serviceProvider = services.BuildServiceProvider();
                var licenseService = serviceProvider.GetRequiredService<ILicenseService>();
                var hardwareInfoProvider = serviceProvider.GetRequiredService<IHardwareInfoProvider>();

                Log("Загрузка лицензии");
                var license = licenseService.LoadLicense();
                if (string.IsNullOrEmpty(license))
                {
                    Log("Ошибка: Лицензия отсутствует");
                    MessageBox.Show("Лицензия отсутствует. Приложение будет закрыто.",
                        "Ошибка лицензии", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Log($"Лицензия загружена, длина: {license.Length}");
                
                Log("Проверка лицензии");
                if (!licenseService.ValidateLicense(license))
                {
                    Log("Ошибка: Лицензия недействительна");
                    MessageBox.Show("Лицензия недействительна. Приложение будет закрыто.",
                        "Ошибка лицензии", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Log("Лицензия успешно проверена");

                var hardwareInfo = hardwareInfoProvider.GetHardwareInfo();
                Log($"Информация об оборудовании: {hardwareInfo}");

                Log("Запуск главной формы");
                Application.Run(new MainForm(hardwareInfoProvider));
            }
            catch (Exception ex)
            {
                var message = $"Критическая ошибка: {ex.Message}\nStack trace: {ex.StackTrace}";
                Log(message);
                MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddSingleton<IHardwareInfoProvider, HardwareInfoProvider>();
            services.AddSingleton<ILicenseService, LicenseService>();
        }
    }
} 