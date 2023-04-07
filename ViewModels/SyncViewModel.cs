using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Models;
using SupernoteDesktopClient.Services.Contracts;
using System;
using System.IO;
using System.Reflection;
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
        private string _targetFolder;

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

            await Task.Run(() => _syncService.Sync(SourceFolder, TargetFolder, _mediaDeviceService.Device.SerialNumber.GetShortSHA1Hash())); 

            IsSyncEnabled = true;
            IsSyncRunning = false;
        }

        private void UpdateSync()
        {
            _mediaDeviceService.RefreshMediaDeviceInfo();

            SourceFolder = (_mediaDeviceService.DriveInfo != null) ? _mediaDeviceService.DriveInfo.RootDirectory.FullName : "N/A";

            string targetFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
            if (String.IsNullOrWhiteSpace(targetFolder) == false && _mediaDeviceService.Device != null)
                targetFolder = Path.Combine(targetFolder, $@"Device\{_mediaDeviceService.Device.SerialNumber.GetShortSHA1Hash()}\Storage");
            else
                targetFolder = null;
            TargetFolder = targetFolder ?? "N/A";

            IsSyncEnabled = (_mediaDeviceService.Device != null);
            IsSyncRunning = false;
        }
    }
}
