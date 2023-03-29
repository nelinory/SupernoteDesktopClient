using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Wpf.Ui.Common.Interfaces;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class AboutViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private string _appVersion = String.Empty;

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        public AboutViewModel()
        {
            AppVersion = $"Version - {GetAssemblyVersion()}";
        }

        private string GetAssemblyVersion()
        {
            Version versionObject = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

            return $"v{versionObject.Major}.{versionObject.Minor}.{versionObject.Build}";
        }
    }
}
