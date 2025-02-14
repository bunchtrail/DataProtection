using System;
using System.IO;
using LicenseCore.Interfaces;
using LicenseCore.Models;

namespace LicenseCore.Services
{
    public class LicenseService : ILicenseService
    {
        private static readonly string LicenseFolderPath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectedApp");
        private static readonly string LicenseFilePath = Path.Combine(LicenseFolderPath, "license.dat");

        private readonly ICryptoService _cryptoService;
        private readonly IHardwareInfoProvider _hardwareInfoProvider;

        public LicenseService(ICryptoService cryptoService, IHardwareInfoProvider hardwareInfoProvider)
        {
            _cryptoService = cryptoService;
            _hardwareInfoProvider = hardwareInfoProvider;
        }

        public string GenerateLicense(HardwareInfo hardwareInfo)
        {
            var hardwareString = hardwareInfo.ToString();
            var signature = _cryptoService.GenerateSignature(hardwareString);
            var licenseData = $"{hardwareString}|{signature}";
            return _cryptoService.Encrypt(licenseData);
        }

        public bool ValidateLicense(string license)
        {
            try
            {
                // Сначала расшифруем лицензию
                var decryptedLicense = _cryptoService.Decrypt(license);
                var parts = decryptedLicense.Split('|');

                if (parts.Length < 7)
                {
                    AppendToLog("[ValidateLicense] Не хватает полей в лицензии: частей меньше 7");
                    return false;
                }

                // Последняя часть — это подпись, всё остальное — HardwareInfo
                var signature = parts[^1];
                var hardwareString = string.Join("|", parts[..^1]);

                // Логируем расшифрованные данные
                AppendToLog($"[ValidateLicense] Расшифрованная строка лицензии (без подписи): {hardwareString}");
                AppendToLog($"[ValidateLicense] Подпись из лицензии (HMAC): {signature}");

                // Проверяем корректность подписи
                if (!_cryptoService.VerifySignature(hardwareString, signature))
                {
                    AppendToLog("[ValidateLicense] Подпись не совпадает, лицензия недействительна.");
                    return false;
                }

                // Получаем реальные данные текущего ПК
                var currentHardware = _hardwareInfoProvider.GetHardwareInfo();
                var currentHardwareString = currentHardware.ToString();
                AppendToLog($"[ValidateLicense] Текущие данные оборудования: {currentHardwareString}");

                // Сравниваем
                bool isValid = hardwareString.Equals(currentHardwareString);
                if (!isValid)
                {
                    AppendToLog("[ValidateLicense] Строка лицензии НЕ совпадает с текущим оборудованием. Лицензия недействительна.");
                }
                else
                {
                    AppendToLog("[ValidateLicense] Строка лицензии совпадает с текущим оборудованием. Лицензия действительна.");
                }

                return isValid;
            }
            catch (Exception ex)
            {
                AppendToLog($"[ValidateLicense] Исключение: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }



        public void SaveLicense(string license)
        {
            try
            {
                Directory.CreateDirectory(LicenseFolderPath);
                File.WriteAllText(LicenseFilePath, license);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при сохранении лицензии: " + ex.Message, ex);
            }
        }

        public string LoadLicense()
        {
            try
            {
                if (File.Exists(LicenseFilePath))
                {
                    return File.ReadAllText(LicenseFilePath);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private void AppendToLog(string message)
        {
            try
            {
                // Путь к файлу лога. Допустим, используем тот же app_log.txt
                // Можете при желании вынести в константу или в настройки
                var logFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ProtectedApp",
                    "app_log.txt");

                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                File.AppendAllText(logFilePath, logMessage);
            }
            catch
            {
                // Если вдруг запись лога не удалась, ничего не делаем, чтобы не мешать работе приложения
            }
        }

    }
} 