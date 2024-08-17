using FolderSynchronizer.interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;


namespace FolderSynchronizer.implemetation
{
    public class EncryptionService : IEncryptionService
    {
        private readonly ILogger<EncryptionService> _logger;

        public EncryptionService(ILogger<EncryptionService> logger)
        {
            _logger = logger;
        }

        public byte[] Encrypt(byte[] data, string key)
        {
            _logger.LogInformation("Encrypting data");
            using var aes = Aes.Create();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            Array.Resize(ref keyBytes, aes.Key.Length);
            aes.Key = keyBytes;

            using var encryptor = aes.CreateEncryptor();
            return PerformCryptography(data, encryptor);
        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
            }
            return memoryStream.ToArray();
        }
    }
}
