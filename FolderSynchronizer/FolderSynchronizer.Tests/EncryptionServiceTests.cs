using FolderSynchronizer.implemetation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace FolderSynchronizer.Tests
{
    public class EncryptionServiceTests
    {
        private readonly EncryptionService _encryptionService;

        public EncryptionServiceTests()
        {
            var loggerMock = new Mock<ILogger<EncryptionService>>();
            _encryptionService = new EncryptionService(loggerMock.Object);
        }

        [Fact]
        public void Encrypt_ShouldEncryptData()
        {
            // Arrange
            byte[] data = Encoding.UTF8.GetBytes("Test data");
            string key = "encryption-key";

            // Act
            byte[] encryptedData = _encryptionService.Encrypt(data, key);

            // Assert
            Assert.NotNull(encryptedData);
            Assert.NotEqual(data, encryptedData);
        }
    }
}
