using SupernoteDesktopClient.Core.Win32Api;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SupernoteDesktopClient.Core
{
    public static class ImageManager
    {
        private static object _syncObject = new object();
        private static Dictionary<string, ImageSource> _imageSourceCache = new Dictionary<string, ImageSource>();

        public static ImageSource GetImageSource(string filename)
        {
            return GetImageSourceFromCache(filename, new Size(24, 24), ItemType.File, ItemState.Undefined);
        }

        public static ImageSource GetImageSource(string directory, ItemState folderType)
        {
            return GetImageSourceFromCache(directory, new Size(24, 24), ItemType.Folder, folderType);
        }

        private static ImageSource GetFileImageSource(string filename, Size size)
        {
            using (var icon = NativeMethods.GetIcon(Path.GetExtension(filename), ItemType.File, IconSize.Large, ItemState.Undefined))
            {
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
            }
        }

        private static ImageSource GetDirectoryImageSource(string directory, Size size, ItemState folderType)
        {
            using (var icon = NativeMethods.GetIcon(directory, ItemType.Folder, IconSize.Large, folderType))
            {
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
            }
        }

        private static ImageSource GetImageSourceFromCache(string itemName, Size itemSize, ItemType itemType, ItemState itemState)
        {
            string cacheKey = $"{(itemType is ItemType.Folder ? ItemType.Folder : Path.GetExtension(itemName))}#{itemSize.Width}#{itemSize.Height}";

            ImageSource returnValue;
            _imageSourceCache.TryGetValue(cacheKey, out returnValue);

            if (returnValue == null)
            {
                lock (_syncObject)
                {
                    _imageSourceCache.TryGetValue(cacheKey, out returnValue);

                    if (returnValue == null)
                    {
                        if (itemType is ItemType.Folder)
                            returnValue = GetDirectoryImageSource(itemName, itemSize, itemState);
                        else
                            returnValue = GetFileImageSource(itemName, itemSize);

                        if (returnValue != null)
                            _imageSourceCache.Add(cacheKey, returnValue);
                    }
                }
            }

            return returnValue;
        }
    }
}
