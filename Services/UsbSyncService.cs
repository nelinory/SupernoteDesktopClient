using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Services.Contracts;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SupernoteDesktopClient.Services
{
    public class UsbSyncService : ISyncService
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;

        public bool IsBusy { get; private set; }

        public string SourceLocation { get { return _mediaDeviceService.SupernoteInfo.RootFolder; } }

        public string BackupLocation { get { return FileSystemManager.GetFolderByType(FileSystemManager.BACKUP_FOLDER, _mediaDeviceService.SupernoteInfo.SerialNumberHash); } }

        public string ArchiveLocation { get { return FileSystemManager.GetFolderByType(FileSystemManager.ARCHIVE_FOLDER, _mediaDeviceService.SupernoteInfo.SerialNumberHash); } }

        public UsbSyncService(IMediaDeviceService mediaDeviceService)
        {
            // services
            _mediaDeviceService = mediaDeviceService;

            IsBusy = false;
        }

        public async Task<bool> Sync()
        {
            // sync in progress
            if (IsBusy == true)
                return false;

            bool returnResult = false;

            IsBusy = true;

            if (_mediaDeviceService.IsDeviceConnected == true)
            {
                await Task.Run(() => ArchiveManager.Archive(BackupLocation, ArchiveLocation, SettingsManager.Instance.Settings.Sync.MaxDeviceArchives));

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
    }
}
