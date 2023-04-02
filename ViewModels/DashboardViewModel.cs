using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaDevices;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Services;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        // services
        private readonly ISnackbarService _snackbarService;
        private readonly IUsbHubDetector _usbHubDetector;

        private const string _supernoteDeviceId = "VID_2207&PID_0011";
        private const string _connectedStatusIcon_on = "PlugConnected24";
        private const string _connectedStatusIcon_off = "PlugDisconnected24";
        private const string _connectedStatusText_on = "Connected";
        private const string _connectedStatusText_off = "Disconnected";

        private MediaDevice _mediaDevice;
        private MediaDriveInfo _mediaDriveInfo;

        [ObservableProperty]
        private string _connectedStatusIcon;

        [ObservableProperty]
        private string _connectedStatusText;

        [ObservableProperty]
        private string _modelNumber;

        [ObservableProperty]
        private string _serialNumber;

        [ObservableProperty]
        private string _serialNumberMasked;

        [ObservableProperty]
        private string _batteryPowerIcon;

        [ObservableProperty]
        private string _batteryPowerText;

        [ObservableProperty]
        private string _deviceSpaceAvailable;

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        public DashboardViewModel(ISnackbarService snackbarService, IUsbHubDetector usbHubDetector)
        {
            // services
            _snackbarService = snackbarService;
            _usbHubDetector = usbHubDetector;

            // event handler
            _usbHubDetector.UsbHubStateChanged += UsbHubDetector_UsbHubStateChanged;

            UpdateDashboard(false);
        }

        [RelayCommand]
        private void CopySerialNumberToClipboard(string serialNumber)
        {
            System.Windows.Clipboard.SetText(serialNumber);
        }

        private void UsbHubDetector_UsbHubStateChanged(string deviceId, bool isConnected)
        {
            // events are invoked on a separate thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateDashboard(true);
            });
        }

        private void UpdateDashboard(bool usbHubStateChanged)
        {
            // if running under visual studio, do not select specific device
            if (Debugger.IsAttached == true)
                _mediaDevice = MediaDevice.GetDevices().FirstOrDefault();
            else
                _mediaDevice = MediaDevice.GetDevices().Where(p => p.DeviceId.Contains(_supernoteDeviceId, System.StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            _mediaDriveInfo = null;
            if (_mediaDevice?.IsConnected == false)
            {
                _mediaDevice.Connect();
                _mediaDriveInfo = _mediaDevice?.GetDrives().FirstOrDefault();
            }

            ConnectedStatusIcon = (_mediaDevice?.IsConnected == true) ? _connectedStatusIcon_on : _connectedStatusIcon_off;
            ConnectedStatusText = (_mediaDevice?.IsConnected == true) ? _connectedStatusText_on : _connectedStatusText_off;
            ModelNumber = (_mediaDevice?.IsConnected == true) ? _mediaDevice?.Model  : "N/A";
            SerialNumber = (_mediaDevice?.IsConnected == true) ? _mediaDevice?.SerialNumber : "N/A";
            SerialNumberMasked = SerialNumber.MaskSerialNumber();

            string batteryPower;
            if (_mediaDevice?.PowerLevel < 100)
                batteryPower = _mediaDevice?.PowerLevel.ToString().Substring(0, 1);
            else
                batteryPower = _mediaDevice?.PowerLevel.ToString().Substring(0, 2);
            BatteryPowerIcon = (_mediaDevice?.IsConnected == true) ? $"Battery{batteryPower}24" : "Battery124";
            BatteryPowerText = (_mediaDevice?.IsConnected == true) ? _mediaDevice?.PowerLevel + "%" : "N/A";

            // TODO: Cleanup bytes to GB conversion
            decimal freeSpace = (_mediaDriveInfo != null) ? (decimal)_mediaDriveInfo?.AvailableFreeSpace / (1024 * 1024 * 1024) : 0;
            decimal totalSpace = (_mediaDriveInfo != null) ? (decimal)_mediaDriveInfo?.TotalSize / (1024 * 1024 * 1024) : 0;
            decimal freeSpacePercent = (_mediaDriveInfo != null) ? (freeSpace / totalSpace) * 100 : 0;
            DeviceSpaceAvailable = (_mediaDriveInfo != null) ? $"{freeSpace.ToString("F2")}GB free of {totalSpace.ToString("F2")}GB ({freeSpacePercent.ToString("F2")}% free space)" : "N/A";

            // notification on usb connect/disconnect
            // TODO: Add option in settings
            if (usbHubStateChanged == true)
            {
                if (_mediaDevice?.IsConnected == true)
                    _snackbarService.Show("Information", $"Device: {ModelNumber} connected.", SymbolRegular.Notebook24, ControlAppearance.Success);
                else
                    _snackbarService.Show("Information", $"Device: disconnected.", SymbolRegular.Notebook24, ControlAppearance.Caution);
            }
        }
    }
}
