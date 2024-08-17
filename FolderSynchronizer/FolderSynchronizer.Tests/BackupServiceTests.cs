using FolderSynchronizer.implemetation;
using Microsoft.Extensions.Logging;
using Moq;

namespace FolderSynchronizer.Tests
{
    public class BackupServiceTests
    {
        private readonly BackupService _backupService;

        public BackupServiceTests()
        {
            var loggerMock = new Mock<ILogger<BackupService>>();
            _backupService = new BackupService(loggerMock.Object);
        }

        [Fact]
        public void BackupFolder_ShouldCreateBackup()
        {
            // Arrange
            string sourceFolder = Path.Combine(Path.GetTempPath(), "SourceFolder");
            Directory.CreateDirectory(sourceFolder);
            string filePath = Path.Combine(sourceFolder, "test.txt");
            File.WriteAllText(filePath, "Test data");

            // Act
            _backupService.BackupFolder(sourceFolder);

            // Assert
            string backupFolder = Directory.GetDirectories(Path.GetTempPath(), "SourceFolder_backup_*")[0];
            Assert.True(Directory.Exists(backupFolder));
            Assert.True(File.Exists(Path.Combine(backupFolder, "test.txt")));

            // Cleanup
            Directory.Delete(sourceFolder, true);
            Directory.Delete(backupFolder, true);
        }
    }
}
