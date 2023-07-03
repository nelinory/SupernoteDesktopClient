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
using SupernoteSharp.Business;
using SupernoteSharp.Common;
using SupernoteSharp.Entities;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Messages;
using System.Threading.Tasks;

namespace SupernoteDesktopClient.Models
{
    // Credit: https://medium.com/@mikependon/designing-a-wpf-treeview-file-explorer-565a3f13f6f2

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
        private async Task OnOpenSelectedItem(object parameter)
        {
            await Task.Run(() => ConvertNoteDocument(parameter));
        }

        private void ConvertNoteDocument(object parameter)
        {
            FileSystemObjectInfo item = parameter as FileSystemObjectInfo;
            if ((item.FileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                return;

            // skip *.mark files until the application can support them
            if (item.FileSystemInfo.Extension == ".mark")
                return;

            try
            {
                string selectedItemFullName = item.FileSystemInfo.FullName;

                if (item.FileSystemInfo.Extension == ".note" && item.FileSystemInfo.Exists == true)
                {
                    WeakReferenceMessenger.Default.Send(new ProgressTrackActionMessage(true)); // action started

                    selectedItemFullName = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(item.FileSystemInfo.Name) + "_sdc.pdf");

                    using (FileStream fileStream = new FileStream(item.FileSystemInfo.FullName, FileMode.Open, FileAccess.Read))
                    {
                        Parser parser = new Parser();
                        Notebook notebook = parser.LoadNotebook(fileStream, Policy.Strict);
                        Converter.PdfConverter converter = new Converter.PdfConverter(notebook, DefaultColorPalette.Grayscale);

                        // convert all pages to vector PDF and build all links
                        byte[] allPages = converter.ConvertAll(vectorize: true, enableLinks: true);
                        // save the result
                        File.WriteAllBytes(selectedItemFullName, allPages);
                    }
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = selectedItemFullName,
                    UseShellExecute = true
                };

                Process process = Process.Start(psi);
            }
            catch (Exception)
            {
                // TODO: Error handling
            }
            finally
            {
                WeakReferenceMessenger.Default.Send(new ProgressTrackActionMessage(false)); // action completed
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
