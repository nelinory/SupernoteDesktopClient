using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Uwp.Notifications;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Messages;
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
        private bool _isDeviceConnected;

        [ObservableProperty]
        private bool _isSyncRunning;

        [ObservableProperty]
        private string _sourceFolder;

        [ObservableProperty]
        private string _sourceAddress;

        [ObservableProperty]
        private string _backupFolder;

        [ObservableProperty]
        private string _lastBackupDateTime;

        [ObservableProperty]
        private ObservableCollection<Models.ArchiveFileAttributes> _archiveFiles;

        [ObservableProperty]
        private bool _archivesVisible;

        public void OnNavigatedTo()
        {
            DiagnosticLogger.Log($"{this}");

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
            WeakReferenceMessenger.Default.Register<MediaDeviceChangedMessage>(this, (r, m) => { UpdateSync(m.Value); });
        }

        [RelayCommand]
        private async Task ExecuteSync()
        {
            IsSyncRunning = true;

            await Task.Run(() => _syncService.Sync());

            IsSyncRunning = false;

            UpdateSync();
        }

        private void UpdateSync(DeviceInfo deviceInfo = null)
        {
            _mediaDeviceService.RefreshMediaDeviceInfo();

            SourceFolder = _mediaDeviceService.SupernoteInfo.RootFolder;
            SourceAddress = "http://XXX.XXX.XXX.XXX:8089";

            // Backup
            BackupFolder = _syncService.BackupFolder ?? "N/A";

            // Last backup DateTime
            DateTime? lastBackupDateTime = FileSystemManager.GetFolderCreateDateTime(BackupFolder);
            LastBackupDateTime = (lastBackupDateTime != null) ? lastBackupDateTime.GetValueOrDefault().ToString("F") : "N/A";

            // Archive
            ArchiveFiles = ArchiveManager.GetArchivesList(_syncService.ArchiveFolder);
            ArchivesVisible = ArchiveFiles.Count > 0;

            IsSyncRunning = _syncService.IsBusy;
            IsDeviceConnected = _mediaDeviceService.IsDeviceConnected;

            // auto sync on connect
            if (SettingsManager.Instance.Settings.Sync.AutomaticSyncOnConnect == true && deviceInfo?.IsConnected == true)
            {
                ExecuteSync().Await();

                new ToastContentBuilder()
                .AddText("Automatic sync completed")
                .Show();
            }
        }
    }
}
