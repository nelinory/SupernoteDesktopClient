using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Wpf.Ui.Common.Interfaces;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class SyncViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private string _appVersion = String.Empty;

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        public SyncViewModel()
        {
        }
    }
}
