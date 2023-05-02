using System;
using System.IO;

namespace SupernoteDesktopClient.Core
{
    public static class FileSystemManager
    {
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

        public static DateTime? GetFolderCreateDateTime(string folder)
        {
            DateTime? returnResult = null;

            if (String.IsNullOrWhiteSpace(folder) == false && Directory.Exists(folder) == true)
                returnResult = Directory.GetCreationTime(folder);

            return returnResult;
        }
    }
}
