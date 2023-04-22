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

        public bool IsBusy { get; private set; }

        public SyncService(IMediaDeviceService mediaDeviceService)
        {
            // services
            _mediaDeviceService = mediaDeviceService;

            IsBusy = false;
        }

        public bool Sync(string sourceFolder, string targetFolder, string deviceId)
        {
            // sync in progress
            if (IsBusy == true)
                return false;

            bool returnResult = false;

            IsBusy = true;

            if (_mediaDeviceService.Device != null)
            {
                if (Directory.Exists(targetFolder) == true)
                {
                    // archive existing storage folder if exists
                    string archiveFolder = FileSystemManager.GetApplicationFolder();
                    if (String.IsNullOrWhiteSpace(archiveFolder) == false)
                        archiveFolder = Path.Combine(archiveFolder, $@"Device\{_mediaDeviceService.Device.SerialNumber.GetShortSHA1Hash()}\Backup");

                    ArchiveManager.Archive(targetFolder, archiveFolder, SettingsManager.Instance.Settings.Sync.MaxDeviceArchives);

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

            IsBusy = false;

            return returnResult;
        }
    }
}
