using Serilog;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;

namespace SupernoteDesktopClient.Core
{
    public static class ArchiveManager
    {
        public static void Archive(string sourceFolder, string archiveFolder, int maxArchivesToKeep)
        {
            string currentDateTime = String.Format("{0:yyyyMMdd_HHmmss}", DateTime.Now);
            string archiveFileName = $"{currentDateTime}_{Path.GetFileName(sourceFolder)}.zip";

            if (ExecuteArchive(sourceFolder, Path.Combine(archiveFolder, archiveFileName)) == true)
                PurgeOldArchives(archiveFolder, Path.GetFileName(sourceFolder), maxArchivesToKeep);
        }

        public static ObservableCollection<Models.FileAttributes> GetArchivesList(string archiveFolder)
        {
            ObservableCollection<Models.FileAttributes> archiveFiles = new ObservableCollection<Models.FileAttributes>();

            try
            {
                if (String.IsNullOrWhiteSpace(archiveFolder) == false && Directory.Exists(archiveFolder) == true)
                {
                    foreach (string fileName in Directory.GetFiles(archiveFolder))
                    {
                        var fileInfo = new FileInfo(fileName);
                        archiveFiles.Add(new Models.FileAttributes(fileInfo.Name, fileInfo.DirectoryName, fileInfo.LastWriteTime, fileInfo.Length));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while getting list of all archives: {EX}", ex);
            }

            return archiveFiles;
        }

        private static bool ExecuteArchive(string sourceFolder, string archiveFileName)
        {
            bool success = false;

            try
            {
                if (Directory.Exists(sourceFolder) == true && Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories).Length > 0)
                {
                    FileSystemManager.EnsureFolderExists(archiveFileName);

                    ZipFile.CreateFromDirectory(sourceFolder, archiveFileName, CompressionLevel.Fastest, false);

                    success = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while executing archive: {EX}", ex);
            }

            return success;
        }

        private static void PurgeOldArchives(string archiveFolder, string archiveFilePattern, int maxArchivesToKeep)
        {
            try
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
            catch (Exception ex)
            {
                Log.Error("Error while purging old archives: {EX}", ex);
            }
        }
    }
}
