using CommunityToolkit.Mvvm.ComponentModel;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Models;
using System.Collections.ObjectModel;
using System.IO;
using Wpf.Ui.Common.Interfaces;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class ExplorerViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private ObservableCollection<FileSystemObjectInfo> _items;

        [ObservableProperty]
        private bool _hasItems;

        public void OnNavigatedTo()
        {
            LoadExplorer();
        }

        public void OnNavigatedFrom()
        {
        }

        public ExplorerViewModel()
        {
        }

        private void LoadExplorer()
        {
            Items = new ObservableCollection<FileSystemObjectInfo>();

            DirectoryInfo deviceDirectory = new DirectoryInfo(FileSystemManager.GetApplicationDeviceFolder());
            if (deviceDirectory.Exists == true)
            {
                FileSystemObjectInfo fileSystemObject = new FileSystemObjectInfo(new DirectoryInfo(FileSystemManager.GetApplicationDeviceFolder()))
                {
                    IsExpanded = true
                };

                Items = new ObservableCollection<FileSystemObjectInfo> { fileSystemObject };
            }

            HasItems = Items.Count > 0;
        }
    }
}
