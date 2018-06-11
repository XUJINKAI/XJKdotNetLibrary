using System;
using System.Runtime.InteropServices;

namespace XJK.WPF
{
    internal static class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public const UInt32 WM_QUERYENDSESSION = 0x0011;
        public const UInt32 WM_ENDSESSION = 0x0016;

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int RegisterApplicationRestart([MarshalAs(UnmanagedType.LPWStr)] string commandLineArgs, int Flags);
    }
}
