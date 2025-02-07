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
                var decryptedLicense = _cryptoService.Decrypt(license);
                var parts = decryptedLicense.Split('|');
                
                if (parts.Length < 7) // 6 hardware components + signature
                    return false;

                var signature = parts[parts.Length - 1];
                var hardwareString = string.Join("|", parts[..^1]);

                // Проверяем подпись
                if (!_cryptoService.VerifySignature(hardwareString, signature))
                    return false;

                // Проверяем текущее оборудование
                var currentHardware = _hardwareInfoProvider.GetHardwareInfo();
                return hardwareString.Equals(currentHardware.ToString());
            }
            catch
            {
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
    }
} 