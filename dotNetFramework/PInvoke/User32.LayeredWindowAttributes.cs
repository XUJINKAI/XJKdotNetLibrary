using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XJK.PInvoke
{
    public static partial class User32
    {
        [DllImport("user32.dll")]
        public static extern bool GetLayeredWindowAttributes(IntPtr hwnd, out uint crKey, out byte bAlpha, out uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    }

    public static class LayeredWindowAttributes
    {
        public const uint
        LWA_ALPHA = 0x2,
        LWA_COLORKEY = 0x1;
    }
}
