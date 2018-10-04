using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.PInvoke
{
    public static class HWND
    {
        public const int
            BROADCAST = 0xffff,
            TOP = 0,
            BOTTOM = 1,
            TOPMOST = -1,
            NOTOPMOST = -2,
            MESSAGE = -3;
    }
    /// <summary>
    /// Special window handles
    /// </summary>
    public static class SpecialWindowHandles
    {
        public readonly static IntPtr
        HWND_BROADCAST = new IntPtr(0xffff),
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        HWND_TOP = new IntPtr(0),
        /// <summary>
        /// Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>
        HWND_BOTTOM = new IntPtr(1),
        /// <summary>
        /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        /// </summary>
        HWND_TOPMOST = new IntPtr(-1),
        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        HWND_NOTOPMOST = new IntPtr(-2),
        // ReSharper restore InconsistentNaming
        HWND_MESSAGE = new IntPtr(-3);
    }
}
