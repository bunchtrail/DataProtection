using System;
using System.Security.Cryptography;
using System.Text;
using LicenseCore.Interfaces;

namespace LicenseCore.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public CryptoService()
        {
            // В реальном приложении эти значения должны быть защищены и храниться в безопасном месте
            _key = Encoding.UTF8.GetBytes("SecretKey1231234SecretKey1231234"); // 32 bytes for AES-256
            _iv = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes for AES
        }

        public string GenerateSignature(string data)
        {
            using var hmac = new HMACSHA256(_key);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }

        public bool VerifySignature(string data, string signature)
        {
            string computedSignature = GenerateSignature(data);
            return computedSignature.Equals(signature);
        }

        public string Encrypt(string data)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(data);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(cipherBytes);
        }

        public string Decrypt(string encryptedData)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] cipherBytes = Convert.FromBase64String(encryptedData);
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
} 