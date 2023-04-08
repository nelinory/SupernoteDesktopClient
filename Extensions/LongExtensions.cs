namespace SupernoteDesktopClient.Extensions
{
    internal static class LongExtensions
    {
        private static readonly string[] DataSizes = { "B", "KB", "MB", "GB", "TB" };

        public static string GetDataSizeAsString(this long value)
        {
            double bytes = value;
            int order = 0;

            while (bytes >= 1024 && order < DataSizes.Length - 1)
            {
                ++order;
                bytes /= 1024;
            }

            return string.Format("{0:0.## }{1}", bytes, DataSizes[order]);
        }
    }
}
