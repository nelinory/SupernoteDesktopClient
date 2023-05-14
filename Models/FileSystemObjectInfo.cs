using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using SupernoteDesktopClient.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace SupernoteDesktopClient.Models
{
    [ObservableObject]
    public partial class FileSystemObjectInfo : BaseObject
    {
        [ObservableProperty]
        private ObservableCollection<FileSystemObjectInfo> _children;

        [ObservableProperty]
        private bool _IsExpanded;

        [ObservableProperty]
        private ImageSource _imageSource;

        [ObservableProperty]
        private FileSystemInfo _fileSystemInfo;

        public FileSystemObjectInfo(FileSystemInfo info)
        {
            if (this is DummyFileSystemObjectInfo)
                return;

            Children = new ObservableCollection<FileSystemObjectInfo>();
            FileSystemInfo = info;

            if (info is DirectoryInfo)
            {
                ImageSource = ImageManager.GetImageSource(info.FullName, ItemState.Close);

                // Show expander, i.e. dummy object only if the folder have sub items
                if (FolderHaveSubItems(info.FullName) == true)
                    Children.Add(new DummyFileSystemObjectInfo());
            }
            else if (info is FileInfo)
            {
                ImageSource = ImageManager.GetImageSource(info.FullName);
            }

            PropertyChanged += new PropertyChangedEventHandler(FileSystemObjectInfo_PropertyChanged);
        }

        [RelayCommand]
        private void OnOpenSelectedItem(object parameter)
        {
            FileSystemObjectInfo item = parameter as FileSystemObjectInfo;
            if ((item.FileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                return;

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = item.FileSystemInfo.FullName;
            psi.UseShellExecute = true;
            try
            {
                Process process = Process.Start(psi);
            }
            catch (Exception)
            {
            }
        }

        private void FileSystemObjectInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FileSystemInfo is DirectoryInfo && String.Equals(e.PropertyName, "IsExpanded", StringComparison.CurrentCultureIgnoreCase))
            {
                if (IsExpanded == true)
                {
                    ImageSource = ImageManager.GetImageSource(FileSystemInfo.FullName, ItemState.Open);
                    DummyFileSystemObjectInfo dummyNode = Children.OfType<DummyFileSystemObjectInfo>().FirstOrDefault();

                    if (dummyNode != null)
                    {
                        Children.Remove(dummyNode);
                        ExploreDirectories();
                        ExploreFiles();
                    }
                }
                else
                    ImageSource = ImageManager.GetImageSource(FileSystemInfo.FullName, ItemState.Close);
            }
        }

        private void ExploreDirectories()
        {
            try
            {
                if (FileSystemInfo is DirectoryInfo)
                {
                    DirectoryInfo[] directories = ((DirectoryInfo)FileSystemInfo).GetDirectories();
                    foreach (DirectoryInfo directory in directories.OrderBy(d => d.Name))
                    {
                        if ((directory.Attributes & FileAttributes.System) != FileAttributes.System &&
                            (directory.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {
                            Children.Add(new FileSystemObjectInfo(directory));
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // continue
            }
        }

        private void ExploreFiles()
        {
            try
            {
                if (FileSystemInfo is DirectoryInfo)
                {
                    FileInfo[] files = ((DirectoryInfo)FileSystemInfo).GetFiles();
                    foreach (FileInfo file in files.OrderBy(d => d.Name))
                    {
                        if ((file.Attributes & FileAttributes.System) != FileAttributes.System &&
                            (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {
                            Children.Add(new FileSystemObjectInfo(file));
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // continue
            }
        }

        private static bool FolderHaveSubItems(string path)
        {
            try
            {
                IEnumerable<string> folders = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
                bool result = folders != null && folders.Any();

                if (result == false)
                {
                    IEnumerable<string> files = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
                    result = files != null && files.Any();
                }

                return result;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }

    internal class DummyFileSystemObjectInfo : FileSystemObjectInfo
    {
        public DummyFileSystemObjectInfo()
            : base(new DirectoryInfo("DummyFileSystemObjectInfo"))
        {
        }
    }
}
