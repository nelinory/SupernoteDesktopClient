using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Services.Contracts;
using System;
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

        public bool Sync(string sourceFolder, string targetFolder, string deviceId)
        {
            bool returnResult = false;

            if (_mediaDeviceService.Device != null)
            {
                if (Directory.Exists(targetFolder) == true)
                {
                    // backup existing storage folder if exists
                    string backupFolder = FileSystemManager.GetApplicationFolder();
                    if (String.IsNullOrWhiteSpace(backupFolder) == false)
                        backupFolder = Path.Combine(backupFolder, $@"Device\{_mediaDeviceService.Device.SerialNumber.GetShortSHA1Hash()}\Backup");

                    // TODO: Load the number of backups to keep from settings
                    BackupManager.Backup(targetFolder, backupFolder, 7);

                    // delete existing storage folder if exists
                    FileSystemManager.ForceDeleteDirectory(targetFolder);
                }

                var supernoteFolder = _mediaDeviceService.Device.GetDirectoryInfo(@"\");
                var files = supernoteFolder.EnumerateFiles("*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    string destinationFileName = file.FullName.Replace(sourceFolder, targetFolder);
                    string destinationFolder = Path.GetDirectoryName(destinationFileName);

                    if (Directory.Exists(destinationFolder) == false)
                        Directory.CreateDirectory(destinationFolder);

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
    }
}
