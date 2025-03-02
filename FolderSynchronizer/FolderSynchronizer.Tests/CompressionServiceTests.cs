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
            string filePath = "testdata.txt"; // Path to save the data to a file
            string dataToWrite = "Test data";
            File.WriteAllText(filePath, dataToWrite); // Save the text to the file

            // Read the data from the file into a FileStream
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Act
                byte[] compressedData = _compressionService.Compress(fileStream);

                // Assert
                Assert.NotNull(compressedData);
                Assert.NotEqual(dataToWrite.Length, compressedData.Length);
            }

            // Clean up: Delete the test file
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
 