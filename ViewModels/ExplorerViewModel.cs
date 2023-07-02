﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Messages;
using SupernoteDesktopClient.Models;
using SupernoteDesktopClient.Services.Contracts;
using System.Collections.ObjectModel;
using System.IO;
using Wpf.Ui.Common.Interfaces;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class ExplorerViewModel : ObservableObject, INavigationAware
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;

        [ObservableProperty]
        private ObservableCollection<FileSystemObjectInfo> _items;

        [ObservableProperty]
        private bool _hasItems;

        [ObservableProperty]
        private bool _convertionInProgress = false;

        public void OnNavigatedTo()
        {
            DiagnosticLogger.Log($"{this}");

            LoadExplorer();
        }

        public void OnNavigatedFrom()
        {
        }

        public ExplorerViewModel(IMediaDeviceService mediaDeviceService)
        {
            _mediaDeviceService = mediaDeviceService;

            // Register a message subscriber
            WeakReferenceMessenger.Default.Register<ProgressTrackActionMessage>(this, (r, m) =>
            {
                ConvertionInProgress = m.Value;
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
