using SupernoteDesktopClient.Extensions;
using System;

namespace SupernoteDesktopClient.Models
{
    public class File
    {
        public string Name { get; private set; }

        public string Path { get; private set; }

        public DateTime CreateDateTime { get; private set; }

        public long SizeBytes { get; private set; }

        public string SizeAsString { get; private set; }

        public File(string name, string path, DateTime createDateTime, long sizeBytes)
        {
            Name = name;
            Path = path;
            CreateDateTime = createDateTime;
            SizeBytes = sizeBytes;
            SizeAsString = sizeBytes.GetDataSizeAsString();
        }
    }
}
