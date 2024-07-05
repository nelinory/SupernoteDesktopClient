using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Messages;
using SupernoteDesktopClient.Services.Contracts;
using SupernoteDesktopClient.Views.Pages;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        // services
        private readonly ISnackbarService _snackbarService;
        private readonly IUsbHubDetector _usbHubDetector;
        private readonly INavigationService _navigationService;
        private readonly IMediaDeviceService _mediaDeviceService;

        [ObservableProperty]
        private bool _isDeviceConnected;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = new();

        [ObservableProperty]
        private bool _minimizeToTrayEnabled = SettingsManager.Instance.Settings.General.MinimizeToTrayEnabled;

        public MainWindowViewModel(ISnackbarService snackbarService, IUsbHubDetector usbHubDetector, INavigationService navigationService, IMediaDeviceService mediaDeviceService)
        {
            // services
            _snackbarService = snackbarService;
            _usbHubDetector = usbHubDetector;
            _navigationService = navigationService;
            _mediaDeviceService = mediaDeviceService;

            // event handler
            _usbHubDetector.UsbHubStateChanged += UsbHubDetector_UsbHubStateChanged;

            BuildNavigationMenu();

            // register a message subscriber
            WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, (r, m) =>
            {
                if (m.Value == SettingsChangedMessage.MINIMIZE_TO_TRAY_ENABLED)
                    MinimizeToTrayEnabled = SettingsManager.Instance.Settings.General.MinimizeToTrayEnabled;
            });

            // offline mode indicator
            IsDeviceConnected = _mediaDeviceService.IsDeviceConnected;
        }

        private void BuildNavigationMenu()
        {
            NavigationItems = new ObservableCollection<object>
            {
                new NavigationViewItem()
                {
                    Content = "Dashboard",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(DashboardPage)
                },
                new NavigationViewItemSeparator(),
                new NavigationViewItem()
                {
                    Content = "Sync",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.ArrowSyncCircle24 },
                    TargetPageType = typeof(SyncPage)
                },
                new NavigationViewItem()
                {
                    Content = "Explorer",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.FolderOpen24 },
                    IsEnabled = true,
                    TargetPageType = typeof(ExplorerPage)
                }
            };

            NavigationFooter = new ObservableCollection<object>
            {
                new NavigationViewItem()
                {
                    Content = "Theme",
                    ToolTip = "Theme",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DarkTheme24 },
                    Command = new RelayCommand(ToggleTheme)
                },
                new NavigationViewItem()
                {
                    Content = "Settings",
                    ToolTip = "Settings",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                    TargetPageType = typeof(SettingsPage)
                },
                new NavigationViewItemSeparator(),
                new NavigationViewItem()
                {
                    Content = "About",
                    ToolTip = "About",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.QuestionCircle24 },
                    TargetPageType = typeof(AboutPage)
                }
            };
        }

        private void ToggleTheme()
        {
            ApplicationThemeManager.Apply(ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Light ? ApplicationTheme.Dark : ApplicationTheme.Light);

            SettingsManager.Instance.Settings.General.CurrentTheme = ApplicationThemeManager.GetAppTheme().ToString();
        }

        private void UsbHubDetector_UsbHubStateChanged(string deviceId, bool isConnected)
        {
            // events are invoked on a separate thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                // notification on usb connect/disconnect
                if (SettingsManager.Instance.Settings.Sync.ShowNotificationOnDeviceStateChange == true)
                {
                    if (isConnected == true)
                        _snackbarService.Show("Information", $"Device: {deviceId} connected.", ControlAppearance.Success, new SymbolIcon { Symbol = SymbolRegular.Notebook24 }, TimeSpan.FromSeconds(4));
                    else
                        _snackbarService.Show("Information", "Device disconnected.", ControlAppearance.Caution, new SymbolIcon { Symbol = SymbolRegular.Notebook24 }, TimeSpan.FromSeconds(4));
                }

                // auto sync on connect
                if (SettingsManager.Instance.Settings.Sync.AutomaticSyncOnConnect == true && isConnected == true)
                    _navigationService.Navigate(typeof(SyncPage));

                // Notify all subscribers
                WeakReferenceMessenger.Default.Send(new MediaDeviceChangedMessage(new DeviceInfo(deviceId, isConnected)));

                // offline mode indicator
                IsDeviceConnected = _mediaDeviceService.IsDeviceConnected;
            });
        }
    }
}
