using FolderSynchronizer.interfaces;
using Microsoft.Extensions.Logging;

namespace FolderSynchronizer.implemetation
{
    public class FolderSynchronizerService : IFolderSynchronizerService
    {
        private readonly IBackupService _backupService;
        private readonly ICompressionService _compressionService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<FolderSynchronizerService> _logger;

        public FolderSynchronizerService(IBackupService backupService, ICompressionService compressionService, IEncryptionService encryptionService, ILogger<FolderSynchronizerService> logger)
        {
            _backupService = backupService;
            _compressionService = compressionService;
            _encryptionService = encryptionService;
            _logger = logger;
        }
        public void SynchronizeFolders(string source, string destination)
        {
            _logger.LogInformation("Starting synchronization from {Source} to {Destination}", source, destination);

            EnsureDirectoryExists(destination);

            var tasks = new List<Task>
        {
            Task.Run(() => CopyNewAndUpdatedFiles(source, destination)),
            Task.Run(() => SynchronizeSubdirectories(source, destination)),
            Task.Run(() => DeleteObsoleteFiles(source, destination)),
            Task.Run(() => DeleteEmptyDirectories(source, destination))
        };

            Task.WhenAll(tasks).Wait();

            _logger.LogInformation("Synchronization completed from {Source} to {Destination}", source, destination);
        }

        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void CopyNewAndUpdatedFiles(string source, string destination)
        {
            Parallel.ForEach(Directory.GetFiles(source), sourceFilePath =>
            {
                string fileName = Path.GetFileName(sourceFilePath);
                string destinationFilePath = Path.Combine(destination, fileName);

                FileInfo fileInfoDestination = new(destinationFilePath);
                FileInfo fileInfosourceFilePath = new(sourceFilePath);

                using (FileStream inputFileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                {
                    if (!fileInfoDestination.Exists || (fileInfosourceFilePath.Length > fileInfoDestination.Length))
                    {
                        byte[] compressedData = _compressionService.Compress(inputFileStream);
                        byte[] encryptedData = _encryptionService.Encrypt(compressedData, "encryption-key");

                        File.WriteAllBytes(destinationFilePath, encryptedData);
                      
                        _logger.LogInformation("Copied: {FileName}", fileName);
                    }
                }
            });
        }

        private void SynchronizeSubdirectories(string source, string destination)
        {
            Parallel.ForEach(Directory.GetDirectories(source), sourceSubDir =>
            {
                string subDirName = Path.GetFileName(sourceSubDir);
                string destinationSubDir = Path.Combine(destination, subDirName);

                SynchronizeFolders(sourceSubDir, destinationSubDir);
            });
        }

        private void DeleteObsoleteFiles(string source, string destination)
        {
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
        }

        private void DeleteEmptyDirectories(string source, string destination)
        {
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
        }
    }
}
