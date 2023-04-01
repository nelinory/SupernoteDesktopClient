using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace SupernoteDesktopClient.Core.Win32Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        [JsonConstructor]
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
