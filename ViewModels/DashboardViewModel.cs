using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Messages;
using SupernoteDesktopClient.Services.Contracts;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;

        private const string CONNECTED_STATUS_ICON_ON = "PlugConnected24";
        private const string CONNECTED_STATUS_ICON_OFF = "PlugDisconnected24";
        private const string CONNECTED_STATUS_TEXT_ON = "Connected";
        private const string CONNECTED_STATUS_TEXT_OFF = "Disconnected";

        [ObservableProperty]
        private bool _isSerialNumberCopyEnabled;

        [ObservableProperty]
        private string _connectedStatusIcon = CONNECTED_STATUS_ICON_OFF;

        [ObservableProperty]
        private string _connectedStatusText;

        [ObservableProperty]
        private string _modelNumber;

        [ObservableProperty]
        private string _serialNumber;

        [ObservableProperty]
        private string _serialNumberMasked;

        [ObservableProperty]
        private string _batteryPowerIcon = "Battery124";

        [ObservableProperty]
        private string _batteryPowerText;

        [ObservableProperty]
        private string _deviceUsedSpace;

        [ObservableProperty]
        private decimal _deviceUsedSpacePercentage;

        public void OnNavigatedTo()
        {
            DiagnosticLogger.Log($"{this}");

            UpdateDashboard();
        }

        public void OnNavigatedFrom()
        {
        }

        public DashboardViewModel(IMediaDeviceService mediaDeviceService)
        {
            // services
            _mediaDeviceService = mediaDeviceService;

            // Register a message subscriber
            WeakReferenceMessenger.Default.Register<MediaDeviceChangedMessage>(this, (r, m) => { UpdateDashboard(); });
        }

        [RelayCommand]
        private void CopySerialNumberToClipboard(string serialNumber)
        {
            System.Windows.Clipboard.SetText(serialNumber);
        }

        private void UpdateDashboard()
        {
            _mediaDeviceService.RefreshMediaDeviceInfo();

            ConnectedStatusIcon = (_mediaDeviceService.IsDeviceConnected == true) ? CONNECTED_STATUS_ICON_ON : CONNECTED_STATUS_ICON_OFF;
            ConnectedStatusText = (_mediaDeviceService.IsDeviceConnected == true) ? CONNECTED_STATUS_TEXT_ON : CONNECTED_STATUS_TEXT_OFF;
            ModelNumber = (_mediaDeviceService.IsDeviceConnected == true) ? _mediaDeviceService.Device?.Model : "N/A";
            SerialNumber = (_mediaDeviceService.IsDeviceConnected == true) ? _mediaDeviceService.Device?.SerialNumber : "N/A";
            SerialNumberMasked = SerialNumber.MaskSerialNumber();

            string batteryPower;
            if (_mediaDeviceService.Device?.PowerLevel < 100)
                batteryPower = _mediaDeviceService.Device?.PowerLevel.ToString().Substring(0, 1);
            else
                batteryPower = _mediaDeviceService.Device?.PowerLevel.ToString().Substring(0, 2);
            BatteryPowerIcon = (_mediaDeviceService.IsDeviceConnected == true) ? $"Battery{batteryPower}24" : "Battery124";
            BatteryPowerText = (_mediaDeviceService.IsDeviceConnected == true) ? _mediaDeviceService.Device?.PowerLevel + "%" : "N/A";

            long freeSpace = (_mediaDeviceService.DriveInfo != null) ? (long)_mediaDeviceService.DriveInfo?.AvailableFreeSpace : 0;
            long totalSpace = (_mediaDeviceService.DriveInfo != null) ? (long)_mediaDeviceService.DriveInfo?.TotalSize : 0;
            DeviceUsedSpacePercentage = (_mediaDeviceService.DriveInfo != null) ? ((totalSpace - freeSpace) / (decimal)totalSpace) * 100 : 0;
            DeviceUsedSpace = (_mediaDeviceService.DriveInfo != null) ? $"{(totalSpace - freeSpace).GetDataSizeAsString()} / {totalSpace.GetDataSizeAsString()} ({DeviceUsedSpacePercentage.ToString("F2")}% used space)" : "N/A";

            IsSerialNumberCopyEnabled = _mediaDeviceService.IsDeviceConnected;
        }
    }
}
