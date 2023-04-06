using SupernoteDesktopClient.Services.Contracts;
using System.IO;

namespace SupernoteDesktopClient.Services
{
    public class SyncService : ISyncService
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;

        public SyncService(IMediaDeviceService mediaDeviceService)
        {
            // services
            _mediaDeviceService = mediaDeviceService;
        }

        public bool Sync(string sourceFolder, string targetFolder)
        {
            bool returnResult = false;

            if (_mediaDeviceService.Device != null)
            {
                // delete existing sync if exists
                if (Directory.Exists(targetFolder) == true)
                {
                    // TODO: Backup destination folder if exists
                    ForceDeleteDirectory(targetFolder);
                }

                var supernoteFolder = _mediaDeviceService.Device.GetDirectoryInfo(@"\");
                var files = supernoteFolder.EnumerateFiles("*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    string destinationFileName = file.FullName.Replace(sourceFolder, targetFolder);
                    string destinationFolder = Path.GetDirectoryName(destinationFileName);

                    if (Directory.Exists(destinationFolder) == false)
                        Directory.CreateDirectory(destinationFolder);

                    // TODO: WaitCurson progress
                    if (File.Exists(destinationFileName) == false)
                    {
                        using (FileStream fs = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write))
                        {
                            _mediaDeviceService.Device.DownloadFile(file.FullName, fs);
                        }
                    }
                }

                returnResult = true;
            }

            return returnResult;
        }

        // TODO: Refactor this one into FileSystemManager
        public static void ForceDeleteDirectory(string path)
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }
    }
}
