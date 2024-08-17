using FolderSynchronizer.implemetation;
using FolderSynchronizer.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FolderSynchronizer.IntegrationTests
{
    public class FolderSynchronizerIntegrationTests
    {
        private readonly IFolderSynchronizer _folderSynchronizer;
        private readonly string _sourceFolder;
        private readonly string _destinationFolder;

        public FolderSynchronizerIntegrationTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(configure => configure.AddConsole())
                .AddSingleton<IFolderSynchronizer, Synchronizer>()
                .AddSingleton<IBackupService, BackupService>()
                .AddSingleton<ICompressionService, CompressionService>()
                .AddSingleton<IEncryptionService, EncryptionService>()
                .BuildServiceProvider();

            _folderSynchronizer = serviceProvider.GetService<IFolderSynchronizer>();
            _sourceFolder = Path.Combine(Path.GetTempPath(), "SourceFolder");
            _destinationFolder = Path.Combine(Path.GetTempPath(), "DestinationFolder");

            Directory.CreateDirectory(_sourceFolder);
            Directory.CreateDirectory(_destinationFolder);
        }

        [Fact]
        public void SynchronizeFolders_ShouldCopyFiles()
        {
            // Arrange
            string filePath = Path.Combine(_sourceFolder, "test.txt");
            File.WriteAllText(filePath, "Test data");

            // Act
            _folderSynchronizer.SynchronizeFolders(_sourceFolder, _destinationFolder);

            // Assert
            Assert.True(File.Exists(Path.Combine(_destinationFolder, "test.txt")));

            // Cleanup
            Directory.Delete(_sourceFolder, true);
            Directory.Delete(_destinationFolder, true);
        }

        [Fact]
        public void SynchronizeFolders_ShouldDeleteFiles()
        {
            // Arrange
            string filePath = Path.Combine(_sourceFolder, "test.txt");
            File.WriteAllText(filePath, "Test data");
            _folderSynchronizer.SynchronizeFolders(_sourceFolder, _destinationFolder);
            File.Delete(filePath);

            // Act
            _folderSynchronizer.SynchronizeFolders(_sourceFolder, _destinationFolder);

            // Assert
            Assert.False(File.Exists(Path.Combine(_destinationFolder, "test.txt")));

            // Cleanup
            Directory.Delete(_sourceFolder, true);
            Directory.Delete(_destinationFolder, true);
        }
    }
}