using CommunityToolkit.Mvvm.ComponentModel;
using SupernoteDesktopClient.Core;
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
            AppVersion = $"Version - {ApplicationManager.GetAssemblyVersion()}";
        }
    }
}
