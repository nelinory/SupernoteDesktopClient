using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SupernoteDesktopClient.Core.Win32Api
{
    public sealed class NativeMethods
    {
        // imports
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string path, uint attributes, out ShellFileInfo fileInfo, uint size, uint flags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr pointer);

        // constants
        public const int SW_SHOW_NORMAL_WINDOW = 1;
        public const int SW_SHOW_MINIMIZED_WINDOW = 2;
        public const int SW_RESTORE_WINDOW = 9;

        // public wrappers
        public static void ShowWindowEx(IntPtr hWnd, int nCmdShow) { ShowWindow(hWnd, nCmdShow); }

        public static void SetForegroundWindowEx(IntPtr hWnd) { SetForegroundWindow(hWnd); }

        public static bool SetWindowPlacementEx(IntPtr hWnd, [In] ref WindowPlacement lpwndpl)
        {
            if (lpwndpl.Length > 0)
            {
                try
                {
                    lpwndpl.Length = Marshal.SizeOf(typeof(WindowPlacement));
                    lpwndpl.Flags = 0;
                    lpwndpl.ShowCmd = (lpwndpl.ShowCmd == NativeMethods.SW_SHOW_MINIMIZED_WINDOW ? NativeMethods.SW_SHOW_NORMAL_WINDOW : lpwndpl.ShowCmd);
                }
                catch
                {
                }

                return SetWindowPlacement(hWnd, ref lpwndpl);
            }
            else
                return true;
        }

        public static WindowPlacement GetWindowPlacementEx(IntPtr hWnd)
        {
            WindowPlacement lpwndpl;
            GetWindowPlacement(hWnd, out lpwndpl);

            return lpwndpl;
        }

        public static Icon GetIcon(string path, ItemType type, IconSize iconSize, ItemState state)
        {
            uint attributes = (uint)(type == ItemType.Folder ? FileAttribute.Directory : FileAttribute.File);
            uint flags = (uint)(ShellAttribute.Icon | ShellAttribute.UseFileAttributes);

            if (type == ItemType.Folder && state == ItemState.Open)
                flags = flags | (uint)ShellAttribute.OpenIcon;

            if (iconSize == IconSize.Small)
                flags = flags | (uint)ShellAttribute.SmallIcon;
            else
                flags = flags | (uint)ShellAttribute.LargeIcon;

            ShellFileInfo fileInfo = new ShellFileInfo();
            uint size = (uint)Marshal.SizeOf(fileInfo);
            IntPtr result = SHGetFileInfo(path, attributes, out fileInfo, size, flags);

            if (result == IntPtr.Zero)
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());

            try
            {
                return (Icon)Icon.FromHandle(fileInfo.hIcon).Clone();
            }
            catch
            {
                throw;
            }
            finally
            {
                DestroyIcon(fileInfo.hIcon);
            }
        }
    }
}
