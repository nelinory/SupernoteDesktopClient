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
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class SyncViewModel : ObservableObject, INavigationAware
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;
        private readonly ISyncService _usbSyncService;
        private readonly ISyncService _wifiSyncService;
        private readonly IDialogControl _dialogControlService;

        [ObservableProperty]
        private bool _isUsbDeviceConnected;

        [ObservableProperty]
        private bool _isSyncRunning;

        [ObservableProperty]
        private string _sourceLocation;

        [ObservableProperty]
        private string _sourceLocationCaption;

        [ObservableProperty]
        private string _sourceLocationDescription;

        [ObservableProperty]
        private string _sourceLocationIcon;

        [ObservableProperty]
        private string _sourceLocationPlaceholderText;

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
        private object _syncButtonParameter;

        [ObservableProperty]
        private bool _isDeviceProfileAvailable;

        public void OnNavigatedTo()
        {
            DiagnosticLogger.Log($"{this}");

            UpdateSync();
        }

        public void OnNavigatedFrom()
        {
        }

        public SyncViewModel(IMediaDeviceService mediaDeviceService, IServiceProvider serviceProvider, IDialogService dialogService)
        {
            // services
            _mediaDeviceService = mediaDeviceService;
            _usbSyncService = serviceProvider.GetKeyedService<ISyncService>(SyncMode.UsbSync);
            _wifiSyncService = serviceProvider.GetKeyedService<ISyncService>(SyncMode.WifiSync);
            _dialogControlService = dialogService.GetDialogControl();

            // register a message subscriber
            WeakReferenceMessenger.Default.Register<MediaDeviceChangedMessage>(this, (r, m) => { UpdateSync(m.Value); });
        }

        [RelayCommand]
        private async Task ExecuteSync(object syncMode)
        {
            IsSyncRunning = true;

            if ((SyncMode)syncMode == SyncMode.UsbSync)
                await _usbSyncService.Sync();
            else
            {
                (bool isValid, string message) result = await HttpManager.IsSourceLocationValidAsync(SourceLocation);

                if (result.isValid == true)
                    await _wifiSyncService.Sync();
                else
                {
                    _dialogControlService.ButtonRightName = "OK";

                    await _dialogControlService.ShowAndWaitAsync("Validation Error", result.message);

                    _dialogControlService.Hide();
                }
            }

            IsSyncRunning = false;

            UpdateSync();
        }

        private void UpdateSync(DeviceInfo deviceInfo = null)
        {
            IsSyncRunning = _usbSyncService.IsBusy || _wifiSyncService.IsBusy;
            IsUsbDeviceConnected = _mediaDeviceService.IsDeviceConnected;
            IsDeviceProfileAvailable = _mediaDeviceService.SupernoteInfo.SerialNumberHash != "N/A";

            // sync in progress, get out
            if (IsSyncRunning == true)
                return;

            _mediaDeviceService.RefreshMediaDeviceInfo();

            if (IsUsbDeviceConnected == true)
            {
                SourceLocation = _mediaDeviceService.SupernoteInfo.RootFolder;
                SourceLocationCaption = "Source location";
                SourceLocationDescription = "Device source location";
                SourceLocationIcon = "Notebook24";
                SourceLocationPlaceholderText = "N/A";
                BackupLocation = _usbSyncService.BackupLocation ?? "N/A";
                SyncButtonCaption = "Usb Synchronize";
                SyncButtonParameter = SyncMode.UsbSync;
            }
            else
            {
                SourceLocation = SettingsManager.Instance.Settings.Sync.SourceLocation;
                SourceLocationCaption = "Source web address";
                SourceLocationDescription = "Browse & Access address";
                SourceLocationIcon = "Wifi124";
                SourceLocationPlaceholderText = "http://XXX.XXX.XXX.XXX:8089";
                BackupLocation = _wifiSyncService.BackupLocation ?? "N/A";
                SyncButtonCaption = "Wifi Synchronize";
                SyncButtonParameter = SyncMode.WifiSync;
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
                ExecuteSync(SyncMode.UsbSync).Await();

                new ToastContentBuilder()
                .AddText("Automatic sync completed")
                .Show();
            }
        }

        partial void OnSourceLocationChanged(string value)
        {
            // update source location for browse & sync only
            if (_mediaDeviceService.IsDeviceConnected == false)
                SettingsManager.Instance.Settings.Sync.SourceLocation = value;
        }
    }
}
