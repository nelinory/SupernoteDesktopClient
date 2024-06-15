using System.Collections.Generic;

namespace SupernoteDesktopClient.Models
{
    public class FileSystemItem
    {
        public string Date { get; set; }
        public string Extension { get; set; }
        public bool IsDirectory { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
    }

    public class FileSystem
    {
        public List<FileSystemItem> FileList { get; set; }
    }
}
