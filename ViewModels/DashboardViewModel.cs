﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Models;
using SupernoteDesktopClient.Services.Contracts;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;

        private const string _connectedStatusIcon_on = "PlugConnected24";
        private const string _connectedStatusIcon_off = "PlugDisconnected24";
        private const string _connectedStatusText_on = "Connected";
        private const string _connectedStatusText_off = "Disconnected";

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

        public void UpdateDashboard()
        {
            _mediaDeviceService.UpdateMediaDeviceInfo();

            ConnectedStatusIcon = (_mediaDeviceService.Device?.IsConnected == true) ? _connectedStatusIcon_on : _connectedStatusIcon_off;
            ConnectedStatusText = (_mediaDeviceService.Device?.IsConnected == true) ? _connectedStatusText_on : _connectedStatusText_off;
            ModelNumber = (_mediaDeviceService.Device?.IsConnected == true) ? _mediaDeviceService.Device?.Model : "N/A";
            SerialNumber = (_mediaDeviceService.Device?.IsConnected == true) ? _mediaDeviceService.Device?.SerialNumber : "N/A";
            SerialNumberMasked = SerialNumber.MaskSerialNumber();

            string batteryPower;
            if (_mediaDeviceService.Device?.PowerLevel < 100)
                batteryPower = _mediaDeviceService.Device?.PowerLevel.ToString().Substring(0, 1);
            else
                batteryPower = _mediaDeviceService.Device?.PowerLevel.ToString().Substring(0, 2);
            BatteryPowerIcon = (_mediaDeviceService.Device?.IsConnected == true) ? $"Battery{batteryPower}24" : "Battery124";
            BatteryPowerText = (_mediaDeviceService.Device?.IsConnected == true) ? _mediaDeviceService.Device?.PowerLevel + "%" : "N/A";

            // TODO: Cleanup bytes to GB conversion
            decimal freeSpace = (_mediaDeviceService.DriveInfo != null) ? (decimal)_mediaDeviceService.DriveInfo?.AvailableFreeSpace / (1024 * 1024 * 1024) : 0;
            decimal totalSpace = (_mediaDeviceService.DriveInfo != null) ? (decimal)_mediaDeviceService.DriveInfo?.TotalSize / (1024 * 1024 * 1024) : 0;
            decimal freeSpacePercent = (_mediaDeviceService.DriveInfo != null) ? (freeSpace / totalSpace) * 100 : 0;
            DeviceSpaceAvailable = (_mediaDeviceService.DriveInfo != null) ? $"{freeSpace.ToString("F2")}GB free of {totalSpace.ToString("F2")}GB ({freeSpacePercent.ToString("F2")}% free space)" : "N/A";
        }
    }
}
