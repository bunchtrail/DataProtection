using LicenseCore.Models;

namespace LicenseCore.Interfaces
{
    public interface ILicenseService
    {
        string GenerateLicense(HardwareInfo hardwareInfo);
        bool ValidateLicense(string license);
        void SaveLicense(string license);
        string LoadLicense();
    }
} 