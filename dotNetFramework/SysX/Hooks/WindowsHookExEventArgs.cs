using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;

namespace XJK.SysX.Hooks
{
    public enum WindowsHookExOnType
    {
        Auto,
        Thread,
        Module,
    }

    public delegate void WindowsHookExEvent(object sender, WindowsHookExEventArgs e);

    public class WindowsHookExEventArgs : EventArgs
    {
#pragma warning disable IDE1006 // 命名样式
        public bool Handled { get; set; }
        public HookType HookType { get; private set; }
        public int nCode { get; private set; }
        public IntPtr wParam { get; private set; }
        public IntPtr lParam { get; private set; }
#pragma warning restore IDE1006 // 命名样式

        public WindowsHookExEventArgs(HookType type, int code, IntPtr wparam, IntPtr lparam)
        {
            Handled = false;
            HookType = type;
            nCode = code;
            wParam = wparam;
            lParam = lparam;
        }

        public WindowsHookExEventArgs(HookType type, int code, int wparam, IntPtr lparam) :
            this(type, code, new IntPtr(wparam), lparam)
        {
        }

        public WindowsHookExEventArgs(HookType type, int code, int wparam, int lparam) :
            this(type, code, new IntPtr(wparam), new IntPtr(lparam))
        {
        }

        public override string ToString()
        {
            return $"<{HookType}: {wParam}[0x{wParam.ToString("x")}], {lParam}[0x{lParam.ToString("x")}]>";
        }
    }
}
