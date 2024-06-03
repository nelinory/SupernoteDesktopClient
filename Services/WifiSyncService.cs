using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Services.Contracts;
using System;
using System.Diagnostics;
using System.IO;

namespace SupernoteDesktopClient.Services
{
    public class WifiSyncService : ISyncService
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;

        public bool IsBusy { get; private set; }

        public string SourceLocation { get { return _mediaDeviceService.SupernoteInfo.RootFolder; } }

        public string BackupLocation { get { return GetFolderByType(FileSystemManager.BACKUP_FOLDER); } }

        public string ArchiveLocation { get { return GetFolderByType(FileSystemManager.ARCHIVE_FOLDER); } }

        public WifiSyncService(IMediaDeviceService mediaDeviceService)
        {
            // services
            _mediaDeviceService = mediaDeviceService;

            IsBusy = false;
        }

        public bool Sync()
        {
            // sync in progress
            if (IsBusy == true)
                return false;

            bool returnResult = false;

            IsBusy = true;

            if (_mediaDeviceService.IsDeviceConnected == true)
            {
                if (Directory.Exists(BackupLocation) == true)
                {
                    ArchiveManager.Archive(BackupLocation, ArchiveLocation, SettingsManager.Instance.Settings.Sync.MaxDeviceArchives);

                    // delete existing storage folder if exists
                    FileSystemManager.ForceDeleteDirectory(BackupLocation);
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                var supernoteFolder = _mediaDeviceService.SupernoteManager.GetDirectoryInfo(@"\");
                var files = supernoteFolder.EnumerateFiles("*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    Debug.WriteLine(file.FullName);
                    string destinationFileName = file.FullName.ReplaceFirstOccurrence(SourceLocation, BackupLocation);
                    string destinationFolder = Path.GetDirectoryName(destinationFileName);

                    if (Directory.Exists(destinationFolder) == false)
                        Directory.CreateDirectory(destinationFolder);

                    if (File.Exists(destinationFileName) == false)
                    {
                        using (FileStream fs = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write))
                        {
                            _mediaDeviceService.SupernoteManager.DownloadFile(file.FullName, fs);
                        }
                    }
                }

                sw.Stop();
                Debug.WriteLine($"Total Sync Time: {sw.Elapsed}");

                returnResult = true;
            }

            IsBusy = false;

            return returnResult;
        }

        private string GetFolderByType(string folderType)
        {
            string folder = FileSystemManager.GetApplicationFolder();
            if (String.IsNullOrWhiteSpace(folder) == false && _mediaDeviceService.SupernoteInfo.SerialNumberHash.Contains("N/A") == false)
                return Path.Combine(folder, $@"Device\{_mediaDeviceService.SupernoteInfo.SerialNumberHash}\{folderType}");
            else
                return null;
        }
    }
}
