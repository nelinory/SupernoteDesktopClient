using System;
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
    }
}
