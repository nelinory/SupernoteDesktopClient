using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly ISyncService _usbSyncService;
        private readonly ISyncService _wifiSyncService;

        [ObservableProperty]
        private bool _isUsbDeviceConnected;

        [ObservableProperty]
        private bool _isSyncRunning;

        [ObservableProperty]
        private string _sourceLocation;

        [ObservableProperty]
        private string _sourceLocationText;

        [ObservableProperty]
        private string _sourceLocationIcon;

        [ObservableProperty]
        private string _backupLocation;

        [ObservableProperty]
        private string _lastBackupDateTime;

        [ObservableProperty]
        private ObservableCollection<Models.ArchiveFileAttributes> _archiveFiles;

        [ObservableProperty]
        private bool _archivesVisible;

        [ObservableProperty]
        private string _syncButtonCaption;

        [ObservableProperty]
        private string _syncButtonParameter;

        public void OnNavigatedTo()
        {
            DiagnosticLogger.Log($"{this}");

            UpdateSync();
        }

        public void OnNavigatedFrom()
        {
        }

        public SyncViewModel(IMediaDeviceService mediaDeviceService, IServiceProvider serviceProvider)
        {
            // services
            _mediaDeviceService = mediaDeviceService;
            _usbSyncService = serviceProvider.GetKeyedService<ISyncService>(SyncMode.UsbSync);
            _wifiSyncService = serviceProvider.GetKeyedService<ISyncService>(SyncMode.WifiSync);

            // register a message subscriber
            WeakReferenceMessenger.Default.Register<MediaDeviceChangedMessage>(this, (r, m) => { UpdateSync(m.Value); });
        }

        [RelayCommand]
        private async Task ExecuteSync(string syncMode)
        {
            IsSyncRunning = true;

            await Task.Run(() => _usbSyncService.Sync());

            IsSyncRunning = false;

            UpdateSync();
        }

        private void UpdateSync(DeviceInfo deviceInfo = null)
        {
            IsSyncRunning = _usbSyncService.IsBusy || _wifiSyncService.IsBusy;
            IsUsbDeviceConnected = _mediaDeviceService.IsDeviceConnected;

            // TODO: sync in progress, get out
            if (IsSyncRunning == true)
                return;

            _mediaDeviceService.RefreshMediaDeviceInfo();

            if (IsUsbDeviceConnected == true)
            {
                SourceLocation = _mediaDeviceService.SupernoteInfo.RootFolder;
                SourceLocationText = "Device source location";
                SourceLocationIcon = "Notebook24";
                BackupLocation = _usbSyncService.BackupLocation ?? "N/A";
                SyncButtonCaption = "Usb Synchronize";
                SyncButtonParameter = SyncMode.UsbSync.ToString();
            }
            else
            {
                SourceLocation = "";
                SourceLocationText = "Browse & Access address";
                SourceLocationIcon = "Wifi124";
                BackupLocation = _wifiSyncService.BackupLocation ?? "N/A";
                SyncButtonCaption = "Wifi Synchronize";
                SyncButtonParameter = SyncMode.WifiSync.ToString();
            }

            // last backup DateTime
            DateTime? lastBackupDateTime = FileSystemManager.GetFolderCreateDateTime(BackupLocation);
            LastBackupDateTime = (lastBackupDateTime != null) ? lastBackupDateTime.GetValueOrDefault().ToString("F") : "N/A";

            // archive
            ArchiveFiles = ArchiveManager.GetArchivesList(_usbSyncService.ArchiveLocation);
            ArchivesVisible = ArchiveFiles.Count > 0;

            // auto sync on connect
            if (SettingsManager.Instance.Settings.Sync.AutomaticSyncOnConnect == true && deviceInfo?.IsConnected == true)
            {
                ExecuteSync(SyncMode.UsbSync.ToString()).Await();

                new ToastContentBuilder()
                .AddText("Automatic sync completed")
                .Show();
            }
        }
    }
}
