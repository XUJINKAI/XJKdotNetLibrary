using System;
using static XJK.SysX.NativeMethods;

namespace XJK.SysX
{
    public static class Topmost
    {
        public static bool Get(IntPtr? hWnd = null)
        {
            IntPtr handle = hWnd ?? GetForegroundWindow();
            int exStyle = GetWindowLong(handle, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
        }

        public static void SetOrToggle(IntPtr? hWnd = null, bool? SetOrToggle = null)
        {
            IntPtr handle = hWnd ?? GetForegroundWindow();
            bool topmost = SetOrToggle ?? !Get();
            IntPtr hWndInsertAfter = topmost ? HWND_TOPMOST : HWND_NOTOPMOST;
            SetWindowPos(handle, hWndInsertAfter, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
    }
}
