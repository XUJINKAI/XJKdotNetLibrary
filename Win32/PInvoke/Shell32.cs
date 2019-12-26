using System;
using System.Runtime.InteropServices;

namespace XJK.Win32.PInvoke
{
    public static partial class Shell32
    {
        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
    }
}
