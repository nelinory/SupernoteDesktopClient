using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SupernoteDesktopClient.Core
{
    public static class ArchiveManager
    {
        public static void Archive(string backupFolder, string archiveFolder, int maxArchivesToKeep)
        {
            string currentDateTime = String.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);
            string archiveFileName = $"{currentDateTime}_{Path.GetFileName(backupFolder)}.zip";

            if (ExecuteArchive(backupFolder, Path.Combine(archiveFolder, archiveFileName)) == true)
                PurgeOldArchives(archiveFolder, Path.GetFileName(backupFolder), maxArchivesToKeep);
        }

        public static ObservableCollection<Models.FileAttributes> GetArchivesList(string archiveFolder)
        {
            ObservableCollection<Models.FileAttributes> archiveFiles = new ObservableCollection<Models.FileAttributes>();

            if (String.IsNullOrWhiteSpace(archiveFolder) == false && Directory.Exists(archiveFolder) == true)
            {
                var directory = new DirectoryInfo(archiveFolder);
                foreach (FileInfo fileInfo in directory.GetFiles().OrderByDescending(p => p.CreationTime))
                {
                    archiveFiles.Add(new Models.FileAttributes(fileInfo.Name, fileInfo.DirectoryName, fileInfo.LastWriteTime, fileInfo.Length));
                }
            }

            return archiveFiles;
        }

        private static bool ExecuteArchive(string backupFolder, string archiveFileName)
        {
            bool success = false;

            if (Directory.Exists(backupFolder) == true && Directory.GetFiles(backupFolder, "*", SearchOption.AllDirectories).Length > 0)
            {
                FileSystemManager.EnsureFolderExists(archiveFileName);

                ZipFile.CreateFromDirectory(backupFolder, archiveFileName, CompressionLevel.Fastest, false);

                success = true;
            }

            return success;
        }

        private static void PurgeOldArchives(string archiveFolder, string archiveFilePattern, int maxArchivesToKeep)
        {
            if (Directory.Exists(archiveFolder) == true)
            {
                string[] archiveFileNames = Directory.GetFiles(archiveFolder, $"*{archiveFilePattern}.zip");
                if (archiveFileNames.Length > maxArchivesToKeep)
                {
                    Array.Sort(archiveFileNames, StringComparer.InvariantCulture);

                    int filesToDelete = archiveFileNames.Length - maxArchivesToKeep;
                    for (int i = 0; i < filesToDelete; i++)
                    {
                        if (File.Exists(archiveFileNames[i]) == true)
                            File.Delete(archiveFileNames[i]);
                    }
                }
            }
        }
    }
}
