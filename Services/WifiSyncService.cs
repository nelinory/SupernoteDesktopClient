using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SupernoteDesktopClient.Services
{
    public class WifiSyncService : ISyncService
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;

        public bool IsBusy { get; private set; }

        public string SourceLocation { get { return SettingsManager.Instance.Settings.Sync.SourceLocation; } }

        public string BackupLocation { get { return FileSystemManager.GetFolderByType(FileSystemManager.BACKUP_FOLDER, _mediaDeviceService.SupernoteInfo.SerialNumberHash); } }

        public string ArchiveLocation { get { return FileSystemManager.GetFolderByType(FileSystemManager.ARCHIVE_FOLDER, _mediaDeviceService.SupernoteInfo.SerialNumberHash); } }

        public WifiSyncService(IMediaDeviceService mediaDeviceService)
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

            if (SourceLocation != "N/A")
            {
                await Task.Run(() => ArchiveManager.Archive(BackupLocation, ArchiveLocation, SettingsManager.Instance.Settings.Sync.MaxDeviceArchives));

                Stopwatch sw = new Stopwatch();
                sw.Start();

                List<string> webFileItems = new List<string>();

                // build file list
                using HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(SourceLocation) };
                await HttpManager.GetWebFolderAsync(httpClient, webFileItems, "/");

                // download all files
                ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 3 };
                await Parallel.ForEachAsync(webFileItems, parallelOptions, async (filePath, ct) =>
                {
                    await HttpManager.DownloadFileAsync(httpClient, BackupLocation, filePath);
                });

                sw.Stop();
                Debug.WriteLine($"Total Sync Time: {sw.Elapsed}");

                returnResult = true;
            }

            IsBusy = false;

            return returnResult;
        }
    }
}
