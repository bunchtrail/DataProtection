namespace LicenseCore.Interfaces
{
    public interface ICryptoService
    {
        string GenerateSignature(string data);
        bool VerifySignature(string data, string signature);
        string Encrypt(string data);
        string Decrypt(string encryptedData);
    }
} 