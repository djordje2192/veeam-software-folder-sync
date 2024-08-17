using FolderSynchronizer.interfaces;
using Microsoft.Extensions.Logging;

namespace FolderSynchronizer.implemetation
{
    public class Synchronizer : IFolderSynchronizer
    {
        private readonly IBackupService _backupService;
        private readonly ICompressionService _compressionService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<Synchronizer> _logger;

        public Synchronizer(IBackupService backupService, ICompressionService compressionService, IEncryptionService encryptionService, ILogger<Synchronizer> logger)
        {
            _backupService = backupService;
            _compressionService = compressionService;
            _encryptionService = encryptionService;
            _logger = logger;
        }
        public void SynchronizeFolders(string source, string destination)
        {
            _logger.LogInformation("Starting synchronization from {Source} to {Destination}", source, destination);

            // Ensure destination folder exists
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            // Copy new and updated files
            Parallel.ForEach(Directory.GetFiles(source), sourceFilePath =>
            {
                string fileName = Path.GetFileName(sourceFilePath);
                string destinationFilePath = Path.Combine(destination, fileName);

                if (!File.Exists(destinationFilePath) || File.GetLastWriteTime(sourceFilePath) > File.GetLastWriteTime(destinationFilePath))
                {
                    // Compress and encrypt file before copying
                    byte[] fileData = File.ReadAllBytes(sourceFilePath);
                    byte[] compressedData = _compressionService.Compress(fileData);
                    byte[] encryptedData = _encryptionService.Encrypt(compressedData, "your-encryption-key");

                    File.WriteAllBytes(destinationFilePath, encryptedData);
                    _logger.LogInformation("Copied: {FileName}", fileName);
                }
            });

            // Recursively synchronize subdirectories
            Parallel.ForEach(Directory.GetDirectories(source), sourceSubDir =>
            {
                string subDirName = Path.GetFileName(sourceSubDir);
                string destinationSubDir = Path.Combine(destination, subDirName);

                SynchronizeFolders(sourceSubDir, destinationSubDir);
            });

            // Delete files in destination that no longer exist in source
            Parallel.ForEach(Directory.GetFiles(destination), destinationFilePath =>
            {
                string fileName = Path.GetFileName(destinationFilePath);
                string sourceFilePath = Path.Combine(source, fileName);

                if (!File.Exists(sourceFilePath))
                {
                    File.Delete(destinationFilePath);
                    _logger.LogInformation("Deleted: {FileName}", fileName);
                }
            });

            // Delete empty directories in destination that no longer exist in source
            Parallel.ForEach(Directory.GetDirectories(destination), destinationSubDir =>
            {
                string subDirName = Path.GetFileName(destinationSubDir);
                string sourceSubDir = Path.Combine(source, subDirName);

                if (!Directory.Exists(sourceSubDir))
                {
                    Directory.Delete(destinationSubDir, true);
                    _logger.LogInformation("Deleted directory: {SubDirName}", subDirName);
                }
            });

            _logger.LogInformation("Synchronization completed from {Source} to {Destination}", source, destination);
        }
    }
}
