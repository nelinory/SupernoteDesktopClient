using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MediaDevices;
using SupernoteDesktopClient.Models;
using SupernoteDesktopClient.Services.Contracts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Controls.Navigation;
using Wpf.Ui.Mvvm.Contracts;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private const string _supernoteDeviceId = "VID_2207&PID_0011";

        // services
        private readonly ISnackbarService _snackbarService;
        private readonly IUsbHubDetector _usbHubDetector;

        private MediaDevice _mediaDevice;
        private string _lastConnectedDeviceModel;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationFooter = new();

        public MainWindowViewModel(ISnackbarService snackbarService, IUsbHubDetector usbHubDetector)
        {
            // services
            _snackbarService = snackbarService;
            _usbHubDetector = usbHubDetector;

            // event handler
            _usbHubDetector.UsbHubStateChanged += UsbHubDetector_UsbHubStateChanged;

            BuildNavigationMenu();

            RefreshMediaDeviceInstance();
        }

        private void BuildNavigationMenu()
        {
            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Dashboard",
                    PageTag = "dashboard",
                    ToolTip = "Dashboard",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(Views.Pages.DashboardPage)
                },
                new NavigationSeparator(),
                new NavigationItem()
                {
                    Content = "Sync",
                    PageTag = "sync",
                    ToolTip = "Sync",
                    Icon = SymbolRegular.ArrowSyncCircle24,
                    PageType = typeof(Views.Pages.SyncPage)
                },
                //new NavigationItem()
                //{
                //    Content = "Note",
                //    PageTag = "note",
                //    Icon = SymbolRegular.Notebook24,
                //    IsEnabled = false
                //    //PageType = typeof(Views.Pages.DataPage)
                //},
                //new NavigationItem()
                //{
                //    Content = "Document",
                //    PageTag = "document",
                //    Icon = SymbolRegular.DocumentText24,
                //    IsEnabled = false
                //    //PageType = typeof(Views.Pages.DataPage)
                //},
                //new NavigationItem()
                //{
                //    Content = "Screenshot",
                //    PageTag = "screenshot",
                //    Icon = SymbolRegular.Image24,
                //    IsEnabled = false
                //    //PageType = typeof(Views.Pages.DataPage)
                //},
                //new NavigationItem()
                //{
                //    Content = "Export",
                //    PageTag = "export",
                //    Icon = SymbolRegular.FolderArrowLeft24,
                //    IsEnabled = false
                //    //PageType = typeof(Views.Pages.DataPage)
                //},
                //new NavigationItem()
                //{
                //    Content = "MyStyle",
                //    PageTag = "mystyle",
                //    IsEnabled = false,
                //    Icon = SymbolRegular.TextBulletListSquareEdit24,
                //    //PageType = typeof(Views.Pages.DataPage)
                //}
            };

            NavigationFooter = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Theme",
                    ToolTip = "Theme",
                    Icon = SymbolRegular.DarkTheme24,
                    Command = new RelayCommand(ToggleTheme)
                },
                new NavigationItem()
                {
                    Content = "Settings",
                    PageTag = "settings",
                    ToolTip = "Settings",
                    Icon = SymbolRegular.Settings24,
                    PageType = typeof(Views.Pages.SettingsPage)
                },
                new NavigationSeparator(),
                new NavigationItem()
                {
                    Content = "About",
                    PageTag = "about",
                    ToolTip = "About",
                    Icon = SymbolRegular.QuestionCircle24,
                    PageType = typeof(Views.Pages.AboutPage)
                }
            };
        }

        private void ToggleTheme()
        {
            Theme.Apply(Theme.GetAppTheme() == ThemeType.Light ? ThemeType.Dark : ThemeType.Light);
        }

        private void UsbHubDetector_UsbHubStateChanged(string deviceId, bool isConnected)
        {
            // events are invoked on a separate thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                RefreshMediaDeviceInstance();

                // notification on usb connect/disconnect
                // TODO: Add option in settings
                if (isConnected == true)
                    _snackbarService.Show("Information", $"Device: {(_lastConnectedDeviceModel != null ? _lastConnectedDeviceModel : "N/A")} connected.", SymbolRegular.Notebook24, ControlAppearance.Success);
                else
                    _snackbarService.Show("Information", $"Device: {(_lastConnectedDeviceModel != null ? _lastConnectedDeviceModel : "N/A")} disconnected.", SymbolRegular.Notebook24, ControlAppearance.Caution);
            });
        }

        private void RefreshMediaDeviceInstance()
        {
            if (_mediaDevice != null)
                _mediaDevice.Disconnect();

            List<MediaDevice> mediaDeviceList = MediaDevice.GetDevices().ToList();

            // if running under visual studio, do not select specific device
            if (Debugger.IsAttached == true)
                _mediaDevice = mediaDeviceList.FirstOrDefault();
            else
                _mediaDevice = mediaDeviceList.Where(p => p.DeviceId.Contains(_supernoteDeviceId, System.StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (_mediaDevice?.IsConnected == false)
                _mediaDevice.Connect();

            if (_mediaDevice != null)
                _lastConnectedDeviceModel = _mediaDevice.Model;

            // Notify all subscribers
            WeakReferenceMessenger.Default.Send(new MediaDeviceChangedMessage(_mediaDevice));
        }
    }
}
