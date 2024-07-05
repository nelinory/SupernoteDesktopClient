using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Messages;
using SupernoteDesktopClient.Models;
using SupernoteDesktopClient.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

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
        private bool _isDeviceConnected;

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

        [ObservableProperty]
        private bool _isUpdateAvailable = false;

        [ObservableProperty]
        private string _updateMessage = String.Empty;

        [ObservableProperty]
        private string _updateDetails = String.Empty;

        [ObservableProperty]
        private bool _isDeviceProfileAvailable;

        public List<string> DeviceProfilesItemSource
        {
            get
            {
                var deviceProfiles = new List<string>();
                foreach (KeyValuePair<string, SupernoteInfo> kvp in SettingsManager.Instance.Settings.DeviceProfiles)
                {
                    deviceProfiles.Add($"{kvp.Key} ({kvp.Value.Model})");
                }

                return deviceProfiles;
            }
        }
        
        public int DeviceProfilesSelectedItem
        {
            get
            {
                int index = -1;

                SupernoteInfo activeProfileHash = SettingsManager.Instance.Settings.DeviceProfiles.Where(p => p.Value.Active == true).FirstOrDefault().Value;
                if (activeProfileHash!= null && string.IsNullOrWhiteSpace(activeProfileHash.SerialNumberHash) == false)
                    index = DeviceProfilesItemSource.FindIndex(p => p == $"{activeProfileHash.SerialNumberHash} ({activeProfileHash.Model})");

                return index == -1 ? 0 : index;
            }
            set
            {
                // clear active state
                foreach (KeyValuePair<string, SupernoteInfo> kvp in SettingsManager.Instance.Settings.DeviceProfiles)
                {
                    kvp.Value.Active = false;
                }

                SettingsManager.Instance.Settings.DeviceProfiles.ElementAt(value).Value.Active = true;
                
                UpdateDashboard();
            }
        }

        public void OnNavigatedTo()
        {
            DiagnosticLogger.Log($"{this}");

            UpdateDashboard();
            RefreshUpdateStatusAsync(false).Await();
        }

        public void OnNavigatedFrom()
        {
        }

        public DashboardViewModel(IMediaDeviceService mediaDeviceService)
        {
            // services
            _mediaDeviceService = mediaDeviceService;

            // register a message subscriber
            WeakReferenceMessenger.Default.Register<MediaDeviceChangedMessage>(this, (r, m) => { UpdateDashboard(); });

            // check for updates on startup
            if (SettingsManager.Instance.Settings.General.AutomaticUpdateCheckEnabled == true)
                RefreshUpdateStatusAsync(true).Await();
        }

        [RelayCommand]
        private static void CopySerialNumberToClipboard(string serialNumber)
        {
            System.Windows.Clipboard.SetText(serialNumber);
        }

        private void UpdateDashboard()
        {
            _mediaDeviceService.RefreshMediaDeviceInfo();

            ConnectedStatusIcon = (_mediaDeviceService.IsDeviceConnected == true) ? CONNECTED_STATUS_ICON_ON : CONNECTED_STATUS_ICON_OFF;
            ConnectedStatusText = (_mediaDeviceService.IsDeviceConnected == true) ? CONNECTED_STATUS_TEXT_ON : CONNECTED_STATUS_TEXT_OFF;
            ModelNumber = _mediaDeviceService.SupernoteInfo.Model;
            SerialNumber = _mediaDeviceService.SupernoteInfo.SerialNumber;
            SerialNumberMasked = SerialNumber.MaskSerialNumber();

            string batteryPower;
            if (_mediaDeviceService.SupernoteInfo.PowerLevel < 100)
                batteryPower = _mediaDeviceService.SupernoteInfo.PowerLevel.ToString().Substring(0, 1);
            else
                batteryPower = _mediaDeviceService.SupernoteInfo.PowerLevel.ToString().Substring(0, 2);
            BatteryPowerIcon = (_mediaDeviceService.IsDeviceConnected == true) ? $"Battery{batteryPower}24" : "Battery124";
            BatteryPowerText = (_mediaDeviceService.IsDeviceConnected == true) ? _mediaDeviceService.SupernoteInfo.PowerLevel + "%" : "N/A";

            long freeSpace = _mediaDeviceService.SupernoteInfo.AvailableFreeSpace;
            long totalSpace = _mediaDeviceService.SupernoteInfo.TotalSpace;
            DeviceUsedSpacePercentage = (_mediaDeviceService.IsDeviceConnected == true) ? ((totalSpace - freeSpace) / (decimal)totalSpace) * 100 : 0;
            DeviceUsedSpace = (_mediaDeviceService.IsDeviceConnected == true) ? $"{(totalSpace - freeSpace).GetDataSizeAsString()} / {totalSpace.GetDataSizeAsString()} ({DeviceUsedSpacePercentage.ToString("F2")}% used space)" : "N/A";

            IsDeviceConnected = _mediaDeviceService.IsDeviceConnected;
            IsDeviceProfileAvailable = _mediaDeviceService.SupernoteInfo.SerialNumberHash != "N/A";

            // refresh profiles dropdown control
            OnPropertyChanged(nameof(DeviceProfilesSelectedItem));
        }

        private async Task RefreshUpdateStatusAsync(bool updateRequested)
        {
            (bool updateAvailable, string updateMessage, string updateDetails) result;

            if (updateRequested == true)
                result = await UpdateManager.CheckForUpdateAsync();
            else
                result = await UpdateManager.GetUpdateDetails();

            IsUpdateAvailable = result.updateAvailable;
            UpdateMessage = result.updateMessage;
            UpdateDetails = result.updateDetails;
        }
    }
}
