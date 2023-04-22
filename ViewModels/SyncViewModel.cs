using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Models;
using SupernoteDesktopClient.Services.Contracts;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Wpf.Ui.Common.Interfaces;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class SyncViewModel : ObservableObject, INavigationAware
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;
        private readonly ISyncService _syncService;

        [ObservableProperty]
        private bool _isSyncEnabled;

        [ObservableProperty]
        private bool _isSyncRunning;

        [ObservableProperty]
        private string _sourceFolder;

        [ObservableProperty]
        private string _backupFolder;

        [ObservableProperty]
        private string _lastBackupDateTime;

        [ObservableProperty]
        private ObservableCollection<Models.FileAttributes> _archiveFiles;

        [ObservableProperty]
        private bool _archivesVisible;

        public void OnNavigatedTo()
        {
            UpdateSync();
        }

        public void OnNavigatedFrom()
        {
        }

        public SyncViewModel(IMediaDeviceService mediaDeviceService, ISyncService syncService)
        {
            // services
            _mediaDeviceService = mediaDeviceService;
            _syncService = syncService;

            // Register a message subscriber
            WeakReferenceMessenger.Default.Register<MediaDeviceChangedMessage>(this, (r, m) => { UpdateSync(); });
        }

        [RelayCommand]
        private async Task ExecuteSync()
        {
            IsSyncEnabled = false;
            IsSyncRunning = true;

            await Task.Run(() => _syncService.Sync(SourceFolder, BackupFolder, _mediaDeviceService.Device.SerialNumber.GetShortSHA1Hash())); 

            IsSyncEnabled = true;
            IsSyncRunning = false;

            UpdateSync();
        }

        private void UpdateSync()
        {
            _mediaDeviceService.RefreshMediaDeviceInfo();

            SourceFolder = (_mediaDeviceService.DriveInfo != null) ? _mediaDeviceService.DriveInfo.RootDirectory.FullName : "N/A";

            // Backup
            string backupFolder = FileSystemManager.GetApplicationFolder();
            if (String.IsNullOrWhiteSpace(backupFolder) == false && _mediaDeviceService.Device != null)
                backupFolder = Path.Combine(backupFolder, $@"Device\{_mediaDeviceService.Device.SerialNumber.GetShortSHA1Hash()}\Storage");
            else
                backupFolder = null;
            BackupFolder = backupFolder ?? "N/A";

            // Last backup DateTime
            DateTime? lastBackupDateTime = FileSystemManager.GetFolderCreateDateTime(BackupFolder);
            LastBackupDateTime = (lastBackupDateTime != null) ? lastBackupDateTime.GetValueOrDefault().ToString("F") : "N/A";

            // Archive
            string archiveFolder = FileSystemManager.GetApplicationFolder();
            if (String.IsNullOrWhiteSpace(archiveFolder) == false && _mediaDeviceService.Device != null)
                archiveFolder = Path.Combine(archiveFolder, $@"Device\{_mediaDeviceService.Device.SerialNumber.GetShortSHA1Hash()}\Backup");
            else
                archiveFolder = null;

            ArchiveFiles = ArchiveManager.GetArchivesList(archiveFolder);
            ArchivesVisible = ArchiveFiles.Count > 0;

            IsSyncEnabled = (_mediaDeviceService.Device != null);
            IsSyncRunning = _syncService.IsBusy;
        }
    }
}
