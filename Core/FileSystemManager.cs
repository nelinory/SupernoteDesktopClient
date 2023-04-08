using System;
using System.IO;
using System.Reflection;

namespace SupernoteDesktopClient.Core
{
    public static class FileSystemManager
    {
        public static void ForceDeleteDirectory(string path)
        {
            try
            {
                var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                directory.Delete(true);
            }
            catch (Exception)
            {
                // TODO: Error logging
            }
        }

        public static void EnsureFolderExists(string fileName)
        {
            string folderName = Path.GetDirectoryName(fileName);

            if (Directory.Exists(folderName) == false)
                Directory.CreateDirectory(folderName);
        }

        public static string GetApplicationFolder()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
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
