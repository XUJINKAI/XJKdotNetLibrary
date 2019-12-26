using System;
using System.Runtime.InteropServices;

namespace XJK.Win32.PInvoke
{
    public static partial class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);
    }
}
