using System.Text;
using Moq;
using Microsoft.Extensions.Logging;
using FolderSynchronizer.implemetation;

namespace FolderSynchronizer.Tests
{
    public class CompressionServiceTests
    {
        private readonly CompressionService _compressionService;

        public CompressionServiceTests()
        {
            var loggerMock = new Mock<ILogger<CompressionService>>();
            _compressionService = new CompressionService(loggerMock.Object);
        }

        [Fact]
        public void Compress_ShouldCompressData()
        {
            // Arrange
            byte[] data = Encoding.UTF8.GetBytes("Test data");

            // Act
            byte[] compressedData = _compressionService.Compress(data);

            // Assert
            Assert.NotNull(compressedData);
            Assert.NotEqual(data.Length, compressedData.Length);
        }
    }
}
 