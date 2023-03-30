using CommunityToolkit.Mvvm.ComponentModel;
using SupernoteDesktopClient.Services;
using System;
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

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        public DashboardViewModel(ISnackbarService snackbarService, IUsbHubDetector usbHubDetector)
        {
            _snackbarService = snackbarService;
            _usbHubDetector = usbHubDetector;

            _usbHubDetector.UsbHubStateChanged += UsbHubDetector_UsbHubStateChanged;
        }

        private void UsbHubDetector_UsbHubStateChanged(string deviceId, bool isConnected)
        {
            // events are invoked on a separate thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (isConnected == true)
                    _snackbarService.Show("Information", $"Device: {deviceId} connected.", SymbolRegular.Notebook24, ControlAppearance.Success);
                else
                    _snackbarService.Show("Information", $"Device: {deviceId} disconnected.", SymbolRegular.Notebook24, ControlAppearance.Caution);
            });
        }
    }
}
