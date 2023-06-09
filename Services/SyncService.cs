﻿using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Services.Contracts;
using System;
using System.Diagnostics;
using System.IO;

namespace SupernoteDesktopClient.Services
{
    public class SyncService : ISyncService
    {
        // services
        private readonly IMediaDeviceService _mediaDeviceService;

        private const string BACKUP_FOLDER = "Backup";
        private const string ARCHIVE_FOLDER = "Archive";

        public bool IsBusy { get; private set; }

        public string SourceFolder { get { return _mediaDeviceService.SupernoteInfo.RootFolder; } }

        public string BackupFolder { get { return GetFolderByType(BACKUP_FOLDER); } }

        public string ArchiveFolder { get { return GetFolderByType(ARCHIVE_FOLDER); } }

        public SyncService(IMediaDeviceService mediaDeviceService)
        {
            // services
            _mediaDeviceService = mediaDeviceService;

            IsBusy = false;
        }

        public bool Sync()
        {
            // sync in progress
            if (IsBusy == true)
                return false;

            bool returnResult = false;

            IsBusy = true;

            if (_mediaDeviceService.IsDeviceConnected == true)
            {
                if (Directory.Exists(BackupFolder) == true)
                {
                    ArchiveManager.Archive(BackupFolder, ArchiveFolder, SettingsManager.Instance.Settings.Sync.MaxDeviceArchives);

                    // delete existing storage folder if exists
                    FileSystemManager.ForceDeleteDirectory(BackupFolder);
                }

                var supernoteFolder = _mediaDeviceService.SupernoteManager.GetDirectoryInfo(@"\");
                var files = supernoteFolder.EnumerateFiles("*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    Debug.WriteLine(file.FullName);
                    string destinationFileName = file.FullName.ReplaceFirstOccurrence(SourceFolder, BackupFolder);
                    string destinationFolder = Path.GetDirectoryName(destinationFileName);

                    if (Directory.Exists(destinationFolder) == false)
                        Directory.CreateDirectory(destinationFolder);

                    if (File.Exists(destinationFileName) == false)
                    {
                        using (FileStream fs = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write))
                        {
                            _mediaDeviceService.SupernoteManager.DownloadFile(file.FullName, fs);
                        }
                    }
                }

                returnResult = true;
            }

            IsBusy = false;

            return returnResult;
        }

        private string GetFolderByType(string folderType)
        {
            string folder = FileSystemManager.GetApplicationFolder();
            if (String.IsNullOrWhiteSpace(folder) == false && _mediaDeviceService.SupernoteInfo.SerialNumberHash.Contains("N/A") == false)
                return Path.Combine(folder, $@"Device\{_mediaDeviceService.SupernoteInfo.SerialNumberHash}\{folderType}");
            else
                return null;
        }
    }
}
