using System;
using System.Runtime.InteropServices;
using System.Text;

namespace XJK.WPF
{
    internal static class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public const UInt32 WM_QUERYENDSESSION = 0x0011;
        public const UInt32 WM_ENDSESSION = 0x0016;


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string lpString);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int RegisterApplicationRestart([MarshalAs(UnmanagedType.LPWStr)] string commandLineArgs, int Flags);
    }
}
