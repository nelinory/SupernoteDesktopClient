using SupernoteDesktopClient.Extensions;
using System;

namespace SupernoteDesktopClient.Models
{
    public class FileAttributes
    {
        public string Name { get; private set; }

        public string Path { get; private set; }

        public DateTime CreateDateTime { get; private set; }

        public long SizeBytes { get; private set; }

        public string SizeAsString { get; private set; }

        public FileAttributes(string name, string path, DateTime createDateTime, long sizeBytes)
        {
            Name = name;
            Path = path;
            CreateDateTime = createDateTime;
            SizeBytes = sizeBytes;
            SizeAsString = sizeBytes.GetDataSizeAsString();
        }
    }
}
