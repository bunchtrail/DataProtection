using System; // Добавлено для STAThread и AppDomain
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
                // Используем базовую директорию приложения для лога
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFile);
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                File.AppendAllText(logPath, logMessage);
                Debug.WriteLine(logMessage);
            }
            catch
            {
                // Игнорируем ошибки логирования, чтобы не прерывать основной поток
            }
        }

        [STAThread] // Атрибут STAThread важен для MessageBox
        static void Main()
        {
            // Базовая настройка для GUI-приложения, даже если мы не показываем сложную форму
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Log("Запуск приложения");
                Log($"Рабочая директория: {AppDomain.CurrentDomain.BaseDirectory}");

                Log("Настройка сервисов");
                var services = new ServiceCollection();
                ConfigureServices(services);

                using var serviceProvider = services.BuildServiceProvider();
                var licenseService = serviceProvider.GetRequiredService<ILicenseService>();
                // IHardwareInfoProvider больше не нужен напрямую здесь, но нужен для LicenseService

                Log("Загрузка лицензии");
                var license = licenseService.LoadLicense();
                if (string.IsNullOrEmpty(license))
                {
                    Log("Ошибка: Лицензия отсутствует");
                    MessageBox.Show("Лицензия отсутствует. Приложение будет закрыто.",
                        "Ошибка лицензии", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Выход из приложения
                }

                Log($"Лицензия загружена, длина: {license.Length}");

                Log("Проверка лицензии");
                if (!licenseService.ValidateLicense(license))
                {
                    Log("Ошибка: Лицензия недействительна");
                    MessageBox.Show("Лицензия недействительна. Приложение будет закрыто.",
                        "Ошибка лицензии", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Выход из приложения
                }

                Log("Лицензия успешно проверена");

                // --- ИЗМЕНЕНИЕ: Вместо запуска MainForm показываем MessageBox ---
                Log("Приложение успешно запущено (показываем сообщение).");
                MessageBox.Show(
                    "Успешно запущено", // Текст сообщения
                    "ProtectedApp",      // Заголовок окна
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Log("Сообщение показано, приложение завершается.");
                // --- Конец изменения ---

            }
            catch (Exception ex)
            {
                var message = $"Критическая ошибка: {ex.Message}\nStack trace: {ex.StackTrace}";
                Log(message);
                // Показываем ошибку пользователю в любом случае
                MessageBox.Show(message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Приложение завершится автоматически после выхода из метода Main
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Оставляем регистрацию сервисов, так как LicenseService все еще нужен
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddSingleton<IHardwareInfoProvider, HardwareInfoProvider>(); // Нужен для LicenseService
            services.AddSingleton<ILicenseService, LicenseService>();
        }
    }
}