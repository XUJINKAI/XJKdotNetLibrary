﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;
using static XJK.PInvoke.User32;
using static XJK.PInvoke.SetWindowPosFlags;
using static XJK.PInvoke.SpecialWindowHandles;
using static XJK.PInvoke.WindowStyles;
using System.Drawing;

namespace XJK.SysX
{
    public static class Win
    {
        public static string GetTitle(IntPtr handle)
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return string.Empty;
        }

        public static string GetPath(IntPtr handle)
        {
            if (Environment.Is64BitOperatingSystem != Environment.Is64BitProcess) return "";
            GetWindowThreadProcessId(handle, out uint pid);
            // system idle, system
            if (pid == 0 || pid == 4) return "";
            var proc = Process.GetProcessById((int)pid);
            try
            {
                return proc.MainModule.FileName;
            }
            catch
            {
                return "";
            }
        }

        public static RECT GetRect(IntPtr handle)
        {
            GetWindowRect(handle, out var rect);
            return rect;
        }

        public static Point GetPosition(IntPtr handle)
        {
            GetWindowRect(handle, out var rect);
            return new Point(rect.X, rect.Y);
        }


        public static class Foreground
        {
            public static IntPtr Get() => GetForegroundWindow();
            public static bool Set(IntPtr handle) => SetForegroundWindow(handle);
        }

        public static class Find
        {
            public static IntPtr ByTitle(string Title)
            {
                IntPtr hWnd = IntPtr.Zero;
                foreach (Process pList in Process.GetProcesses())
                {
                    if (pList.MainWindowTitle.Contains(Title))
                    {
                        hWnd = pList.MainWindowHandle;
                    }
                }
                return hWnd;
            }
        }

        public static class Topmost
        {
            public static bool Get(IntPtr handle)
            {
                int exStyle = (int)GetWindowLong(handle,  WindowLongFlags.GWL_EXSTYLE);
                return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
            }

            public static void Set(IntPtr handle, bool topmost)
            {
                IntPtr hWndInsertAfter = topmost ? HWND_TOPMOST : HWND_NOTOPMOST;
                SetWindowPos(handle, hWndInsertAfter, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            }

            public static void Toggle(IntPtr handle)
            {
                Set(handle, !Get(handle));
            }
        }

        public static class Opacity
        {
            public static double Get(IntPtr handle)
            {
                SetWindowLong(handle, WindowLongFlags.GWL_EXSTYLE, WS_EX_LAYERED);
                GetLayeredWindowAttributes(handle, out uint crKey, out byte bAlpha, out uint dwFlags);
                var re = bAlpha / 255.0;
                if (re == 0)
                {
                    Set(handle, 1);
                    re = 1;
                }
                return re;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="opacity">0 ~ 1</param>
            /// <param name="hWnd"></param>
            public static void Set(IntPtr handle, double opacity)
            {
                if (opacity < 0 || opacity > 1) throw new ArgumentOutOfRangeException("opacity must in range 0 - 1.");
                SetWindowLong(handle, WindowLongFlags.GWL_EXSTYLE, WS_EX_LAYERED);
                SetLayeredWindowAttributes(handle, 0, (byte)(opacity * 255), LayeredWindowAttributes.LWA_ALPHA);
            }
        }
    }
}