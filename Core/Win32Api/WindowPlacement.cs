using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace SupernoteDesktopClient.Core.Win32Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement
    {
        public int Length { get; set; }
        public int Flags { get; set; }
        public int ShowCmd { get; set; }
        public Point MinPosition { get; set; }
        public Point MaxPosition { get; set; }
        public Rect NormalPosition { get; set; }

        [JsonConstructor]
        public WindowPlacement(int length, int flags, int showCmd, Point minPosition, Point maxPosition, Rect normalPosition)
        {
            Length = length;
            Flags = flags;
            ShowCmd = showCmd;
            MinPosition = minPosition;
            MaxPosition = maxPosition;
            NormalPosition = normalPosition;
        }
    }
}
