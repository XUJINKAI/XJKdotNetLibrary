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
        public int wParam { get; private set; }
        public int lParam { get; private set; }
#pragma warning restore IDE1006 // 命名样式

        public WindowsHookExEventArgs(HookType type, IntPtr wparam, IntPtr lparam) :
            this(type, wparam.ToInt32(), lparam.ToInt32())
        { }

        public WindowsHookExEventArgs(HookType type, int wparam, IntPtr lparam) :
            this(type, wparam, lparam.ToInt32())
        { }

        public WindowsHookExEventArgs(HookType type, int wparam, int lparam)
        {
            Handled = false;
            HookType = type;
            wParam = wparam;
            lParam = lparam;
        }

        public override string ToString()
        {
            return $"<{HookType}: {wParam}[0x{wParam.ToString("x")}], {lParam}[0x{lParam.ToString("x")}]>";
        }
    }
}
