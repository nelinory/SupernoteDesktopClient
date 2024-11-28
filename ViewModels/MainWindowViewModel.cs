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
        private WindowState _mainWindowState;

        [ObservableProperty]
        private Visibility _mainWindowVisibility;

        [ObservableProperty]
        private bool _mainWindowShowInTaskbar;

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

            // offline mode indicator
            IsDeviceConnected = _mediaDeviceService.IsDeviceConnected;
        }

        [RelayCommand]
        private void Minimize()
        {
            if (SettingsManager.Instance.Settings.General.MinimizeToTrayEnabled == true)
            {
                MainWindowShowInTaskbar = true; // preventing the weird glitch of just window title showing on minimize
                MainWindowState = WindowState.Minimized;
                MainWindowVisibility = Visibility.Hidden;
                MainWindowShowInTaskbar = false;
            }
        }

        [RelayCommand]
        private void TrayIconContextMenu(string tag)
        {
            switch (tag)
            {
                case "home":
                    _navigationService.Navigate(typeof(DashboardPage));
                    ShowApplicationWindow();
                    break;
                case "sync":
                    _navigationService.Navigate(typeof(SyncPage));
                    ShowApplicationWindow();
                    break;
                case "explorer":
                    _navigationService.Navigate(typeof(ExplorerPage));
                    ShowApplicationWindow();
                    break;
                case "settings":
                    _navigationService.Navigate(typeof(SettingsPage));
                    ShowApplicationWindow();
                    break;
                default: // exit
                    Application.Current.Shutdown();
                    break;
            }
        }

        [RelayCommand]
        private void ShowApplicationWindow()
        {
            if (MainWindowVisibility == Visibility.Hidden)
                MainWindowVisibility = Visibility.Visible;

            if (MainWindowState == WindowState.Minimized)
                MainWindowState = WindowState.Normal;

            MainWindowShowInTaskbar = true;
        }

        private void BuildNavigationMenu()
        {
            NavigationItems = new ObservableCollection<object>
            {
                new NavigationViewItem()
                {
                    Content = "Dashboard",
                    ToolTip = "Dashboard",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(DashboardPage)
                },
                new NavigationViewItemSeparator(),
                new NavigationViewItem()
                {
                    Content = "Sync",
                    ToolTip = "Sync",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.ArrowSyncCircle24 },
                    TargetPageType = typeof(SyncPage)
                },
                new NavigationViewItem()
                {
                    Content = "Explorer",
                    ToolTip = "Explorer",
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
