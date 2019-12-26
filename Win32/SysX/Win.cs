using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using XJK.Win32.PInvoke;
using static XJK.Win32.PInvoke.SetWindowPosFlags;
using static XJK.Win32.PInvoke.SpecialWindowHandles;
using static XJK.Win32.PInvoke.User32;
using static XJK.Win32.PInvoke.WindowStyles;

namespace XJK.Win32.SysX
{
    public class Win
    {
        #region Class Instance Wrap

        public IntPtr Handle { get; set; }
        public static implicit operator IntPtr(Win win) => win.Handle;
        public static implicit operator Win(IntPtr handle) => new Win() { Handle = handle };

        public string Title => GetTitle(Handle);
        public string Path => GetPath(Handle);
        public string ClassName => throw new NotImplementedException();
        public int ThreadID => GetThreadId(this);
        public int ProcessID => GetProcessId(this);
        public bool IsForeground
        {
            get { return Foreground.Get() == this; }
        }
        public bool IsTopmost
        {
            get => Topmost.Get(this);
            set => Topmost.Set(this, value);
        }
        public Process Process => Process.GetProcessById(ProcessID);

        #endregion

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
            GetWindowThreadProcessId(handle, out int pid);
            // system idle, system
            if (pid == 0 || pid == 4) return "";
            var proc = Process.GetProcessById(pid);
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

        public static int GetThreadId(IntPtr handle)
        {
            return GetWindowThreadProcessId(handle, IntPtr.Zero);
        }

        public static int GetProcessId(IntPtr handle)
        {
            GetWindowThreadProcessId(handle, out int processId);
            return processId;
        }

        public static void Close(IntPtr handle)
        {
            SendMessage(handle, WindowsMessages.CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        public static void Kill(IntPtr handle)
        {
            Process.GetProcessById(GetProcessId(handle)).Kill();
        }

        public static class Foreground
        {
            public static IntPtr Get() => GetForegroundWindow();
            public static bool Set(IntPtr handle) => SetForegroundWindow(handle);
        }

        public static class Find
        {
            public static IntPtr ByWindow(Window window)
            {
                return new WindowInteropHelper(window).Handle;
            }

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
                var exStyle = ExStyle.Get(handle);
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
                ExStyle.Set(handle, WS_EX_LAYERED);
                GetLayeredWindowAttributes(handle, out uint crKey, out byte bAlpha, out uint dwFlags);
                var re = bAlpha / 255.0;
                if (re == 0) // 未设置
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
                ExStyle.Set(handle, WS_EX_LAYERED);
                SetLayeredWindowAttributes(handle, 0, (byte)(opacity * 255), LayeredWindowAttributes.LWA_ALPHA);
            }
        }

        public static class Style
        {
            public static uint Get(IntPtr Handle)
            {
                return (uint)GetWindowLong(Handle, WindowLongFlags.GWL_STYLE);
            }

            public static void Set(IntPtr Handle, uint style)
            {
                SetWindowLong(Handle, WindowLongFlags.GWL_STYLE, style);
            }

            public static uint Add(IntPtr Handle, uint style)
            {
                uint newStyle = Get(Handle) | style;
                Set(Handle, newStyle);
                return newStyle;
            }

            public static uint Remove(IntPtr Handle, uint style)
            {
                uint newStyle = Get(Handle) & ~style;
                Set(Handle, newStyle);
                return newStyle;
            }
        }

        public static class ExStyle
        {
            public static uint Get(IntPtr Handle)
            {
                return (uint)GetWindowLong(Handle, WindowLongFlags.GWL_EXSTYLE);
            }

            public static void Set(IntPtr Handle, uint exstyle)
            {
                SetWindowLong(Handle, WindowLongFlags.GWL_EXSTYLE, exstyle);
            }

            public static uint Add(IntPtr Handle, uint exstyle)
            {
                uint newStyle = Get(Handle) | exstyle;
                Set(Handle, newStyle);
                return newStyle;
            }

            public static uint Remove(IntPtr Handle, uint exstyle)
            {
                uint newStyle = Get(Handle) & ~exstyle;
                Set(Handle, newStyle);
                return newStyle;
            }
        }
    }
}
