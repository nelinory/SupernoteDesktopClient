﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Models;
using SupernoteDesktopClient.Services.Contracts;
using SupernoteDesktopClient.Views.Pages;
using System.Collections.ObjectModel;
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
        // services
        private readonly ISnackbarService _snackbarService;
        private readonly IUsbHubDetector _usbHubDetector;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationFooter = new();

        [ObservableProperty]
        private bool _minimizeToTrayEnabled = SettingsManager.Instance.Settings.General.MinimizeToTrayEnabled;

        public MainWindowViewModel(ISnackbarService snackbarService, IUsbHubDetector usbHubDetector, INavigationService navigationService)
        {
            // services
            _snackbarService = snackbarService;
            _usbHubDetector = usbHubDetector;
            _navigationService = navigationService;

            // event handler
            _usbHubDetector.UsbHubStateChanged += UsbHubDetector_UsbHubStateChanged;

            BuildNavigationMenu();

            // Register a message subscriber
            WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, (r, m) =>
            {
                MinimizeToTrayEnabled = SettingsManager.Instance.Settings.General.MinimizeToTrayEnabled;
            });
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

            SettingsManager.Instance.Settings.General.CurrentTheme = Theme.GetAppTheme().ToString();
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
                        _snackbarService.Show("Information", $"Device: {deviceId} connected.", SymbolRegular.Notebook24, ControlAppearance.Success);
                    else
                        _snackbarService.Show("Information", $"Device disconnected.", SymbolRegular.Notebook24, ControlAppearance.Caution);
                }

                // auto sync on connect
                if (SettingsManager.Instance.Settings.Sync.AutomaticSyncOnConnect == true && isConnected == true)
                    _navigationService.Navigate(typeof(SyncPage));

                // Notify all subscribers
                WeakReferenceMessenger.Default.Send(new MediaDeviceChangedMessage(new DeviceInfo(deviceId, isConnected)));
            });
        }
    }
}
