using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Services.Contracts;
using System;
using System.Collections.ObjectModel;
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
        private bool _isSyncButtonEnabled;

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
            WeakReferenceMessenger.Default.Register<Models.MediaDeviceChangedMessage>(this, (r, m) => { UpdateSync(m.Value); });
        }

        [RelayCommand]
        private async Task ExecuteSync()
        {
            IsSyncButtonEnabled = false;
            IsSyncRunning = true;

            await Task.Run(() => _syncService.Sync());

            IsSyncButtonEnabled = true;
            IsSyncRunning = false;

            UpdateSync();
        }

        private void UpdateSync(Models.DeviceInfo deviceInfo = null)
        {
            _mediaDeviceService.RefreshMediaDeviceInfo();

            SourceFolder = (_mediaDeviceService.DriveInfo != null) ? _mediaDeviceService.DriveInfo.RootDirectory.FullName : "N/A";

            // Backup
            BackupFolder = _syncService.BackupFolder ?? "N/A";

            // Last backup DateTime
            DateTime? lastBackupDateTime = FileSystemManager.GetFolderCreateDateTime(BackupFolder);
            LastBackupDateTime = (lastBackupDateTime != null) ? lastBackupDateTime.GetValueOrDefault().ToString("F") : "N/A";

            // Archive
            ArchiveFiles = ArchiveManager.GetArchivesList(_syncService.ArchiveFolder);
            ArchivesVisible = ArchiveFiles.Count > 0;

            IsSyncButtonEnabled = (_mediaDeviceService.Device != null);
            IsSyncRunning = _syncService.IsBusy;

            // auto sync on connect
            if (SettingsManager.Instance.Settings.Sync.AutomaticSyncOnConnect == true && deviceInfo?.IsConnected == true)
                ExecuteSync().Await();
        }
    }
}
