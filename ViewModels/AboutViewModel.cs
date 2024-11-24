using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using System;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class AboutViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private bool _isUpdateCheckEnabled = true;

        [ObservableProperty]
        private bool _isUpdateAvailable = false;

        [ObservableProperty]
        private string _updateMessage = String.Empty;

        [ObservableProperty]
        private string _updateDetails = String.Empty;

        public void OnNavigatedTo()
        {
            DiagnosticLogger.Log($"{this}");

            RefreshUpdateStatusAsync(false).Await();
        }

        public void OnNavigatedFrom()
        {
        }

        public AboutViewModel()
        {
            AppVersion = $"Version - {ApplicationManager.GetAssemblyVersion()}";
        }

        [RelayCommand]
        private async Task ExecuteCheckForUpdate()
        {
            IsUpdateCheckEnabled = false;

            await RefreshUpdateStatusAsync(true);
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

            if (IsUpdateAvailable == true)
                IsUpdateCheckEnabled = false;
            else
                IsUpdateCheckEnabled = true;
        }
    }
}
