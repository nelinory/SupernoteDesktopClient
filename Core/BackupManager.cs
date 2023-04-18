using Serilog;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;

namespace SupernoteDesktopClient.Core
{
    public static class BackupManager
    {
        public static void Backup(string sourceFolder, string backupFolder, int maxBackupsToKeep)
        {
            string currentDateTime = String.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);
            string backupFileName = $"{currentDateTime}_{Path.GetFileName(sourceFolder)}.zip";

            if (ExecuteBackup(sourceFolder, Path.Combine(backupFolder, backupFileName)) == true)
                PurgeOldBackups(backupFolder, Path.GetFileName(sourceFolder), maxBackupsToKeep);
        }

        public static ObservableCollection<Models.FileAttributes> GetPreviousBackupsList(string backupFolder)
        {
            ObservableCollection<Models.FileAttributes> backupFiles = new ObservableCollection<Models.FileAttributes>();

            try
            {
                if (String.IsNullOrWhiteSpace(backupFolder) == false && Directory.Exists(backupFolder) == true)
                {
                    foreach (string fileName in Directory.GetFiles(backupFolder))
                    {
                        var fileInfo = new FileInfo(fileName);
                        backupFiles.Add(new Models.FileAttributes(fileInfo.Name, fileInfo.DirectoryName, fileInfo.LastWriteTime, fileInfo.Length));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while getting list of all backups: {EX}", ex);
            }

            return backupFiles;
        }

        private static bool ExecuteBackup(string sourceFolder, string backupFileName)
        {
            bool success = false;

            try
            {
                if (Directory.Exists(sourceFolder) == true && Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories).Length > 0)
                {
                    FileSystemManager.EnsureFolderExists(backupFileName);

                    ZipFile.CreateFromDirectory(sourceFolder, backupFileName, CompressionLevel.Fastest, false);

                    success = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while executing backup: {EX}", ex);
            }

            return success;
        }

        private static void PurgeOldBackups(string backupLocation, string backupFilePattern, int maxBackupsToKeep)
        {
            try
            {
                if (Directory.Exists(backupLocation) == true)
                {
                    string[] backupFileNames = Directory.GetFiles(backupLocation, $"*{backupFilePattern}.zip");
                    if (backupFileNames.Length > maxBackupsToKeep)
                    {
                        Array.Sort(backupFileNames, StringComparer.InvariantCulture);

                        int filesToDelete = backupFileNames.Length - maxBackupsToKeep;
                        for (int i = 0; i < filesToDelete; i++)
                        {
                            if (File.Exists(backupFileNames[i]) == true)
                                File.Delete(backupFileNames[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while purging old backups: {EX}", ex);
            }
        }
    }
}
