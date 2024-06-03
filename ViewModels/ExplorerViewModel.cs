using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Messages;
using SupernoteDesktopClient.Models;
using SupernoteDesktopClient.Services.Contracts;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class ExplorerViewModel : ObservableObject, INavigationAware
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;
        private readonly ISnackbarService _snackbarService;

        [ObservableProperty]
        private ObservableCollection<FileSystemObjectInfo> _items;

        [ObservableProperty]
        private bool _hasItems;

        [ObservableProperty]
        private bool _conversionInProgress = false;

        public void OnNavigatedTo()
        {
            DiagnosticLogger.Log($"{this}");

            LoadExplorer();
        }

        public void OnNavigatedFrom()
        {
        }

        public ExplorerViewModel(IMediaDeviceService mediaDeviceService, ISnackbarService snackbarService)
        {
            _mediaDeviceService = mediaDeviceService;
            _snackbarService = snackbarService;

            // register a message subscriber
            WeakReferenceMessenger.Default.Register<ProgressTrackActionMessage>(this, (r, m) =>
            {
                ConversionInProgress = m.Value;
            });

            WeakReferenceMessenger.Default.Register<ConversionFailedMessage>(this, (r, m) =>
            {
                // events are invoked on a separate thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _snackbarService.Show("Conversion Error", m.Value, SymbolRegular.DocumentError24, ControlAppearance.Danger);
                });
            });
        }

        private void LoadExplorer()
        {
            Items = new ObservableCollection<FileSystemObjectInfo>();
            string localPath = Path.Combine(FileSystemManager.GetApplicationDeviceFolder(), _mediaDeviceService.SupernoteInfo.SerialNumberHash);

            DirectoryInfo deviceDirectory = new DirectoryInfo(localPath);
            if (deviceDirectory.Exists == true)
            {
                FileSystemObjectInfo fileSystemObject = new FileSystemObjectInfo(deviceDirectory)
                {
                    IsExpanded = true
                };

                Items = new ObservableCollection<FileSystemObjectInfo> { fileSystemObject };
            }

            HasItems = Items.Count > 0;
        }
    }
}
