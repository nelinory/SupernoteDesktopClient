﻿using System;
using System.IO;

namespace SupernoteDesktopClient.Core
{
    public static class FileSystemManager
    {
        public const string BACKUP_FOLDER = "Backup";
        public const string ARCHIVE_FOLDER = "Archive";

        public static void ForceDeleteDirectory(string path)
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }

        public static void EnsureFolderExists(string fileName)
        {
            string folderName = Path.GetDirectoryName(fileName);

            if (Directory.Exists(folderName) == false)
                Directory.CreateDirectory(folderName);
        }

        public static string GetApplicationFolder()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetApplicationDeviceFolder()
        {
            return Path.Combine(GetApplicationFolder(), @"Device");
        }

        public static DateTime? GetFolderCreateDateTime(string folder)
        {
            DateTime? returnResult = null;

            if (String.IsNullOrWhiteSpace(folder) == false && Directory.Exists(folder) == true)
                returnResult = Directory.GetCreationTime(folder);

            return returnResult;
        }

        public static void CleanupTempConversionFiles()
        {
            try
            {
                string[] tempFileNames = Directory.GetFiles(Path.GetTempPath(), $"*_sdc.pdf");
                for (int i = 0; i < tempFileNames.Length; i++)
                {
                    if (File.Exists(tempFileNames[i]) == true)
                        File.Delete(tempFileNames[i]);
                }
            }
            catch (Exception)
            {
                // errors while deleting temporary files
            }
        }

        public static string GetFolderByType(string folderType, string serialNumberHash)
        {
            string folder = GetApplicationFolder();
            if (String.IsNullOrWhiteSpace(folder) == false && serialNumberHash.Contains("N/A") == false)
                return Path.Combine(folder, $@"Device\{serialNumberHash}\{folderType}");
            else
                return null;
        }
    }
}
