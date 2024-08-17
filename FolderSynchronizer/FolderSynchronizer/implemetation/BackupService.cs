using FolderSynchronizer.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronizer.implemetation
{
    public class BackupService : IBackupService
    {
        private readonly ILogger<BackupService> _logger;

        public BackupService(ILogger<BackupService> logger)
        {
            _logger = logger;
        }

        public void BackupFolder(string folderPath)
        {
            string backupPath = folderPath + "_backup_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            DirectoryCopy(folderPath, backupPath, true);
            _logger.LogInformation("Backup created at: {BackupPath}", backupPath);
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
