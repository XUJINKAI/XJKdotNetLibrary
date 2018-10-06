using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;

namespace XJK.SysX.WinMsg
{
    public delegate void WindowMessageEvent(WindowMessageEventArgs e);
    public delegate void WindowMessagePowerBroadcastEvent(PBT pbt, WindowMessageEventArgs e);

    public class WindowMessageEventArgs : EventArgs
    {
#pragma warning disable IDE1006 // 命名样式
        public bool Handled { get; set; }
        public int Msg { get; private set; }
        public int wParam { get; private set; }
        public int lParam { get; private set; }
#pragma warning restore IDE1006 // 命名样式


        public WindowMessageEventArgs(int msg, IntPtr wparam, IntPtr lparam) :
            this(msg, wparam.ToInt32(), lparam.ToInt32())
        { }

        public WindowMessageEventArgs(int msg, int wparam, IntPtr lparam) :
            this(msg, wparam, lparam.ToInt32())
        { }

        public WindowMessageEventArgs(int msg, int wparam, int lparam)
        {
            Handled = false;
            Msg = msg;
            wParam = wparam;
            lParam = lparam;
        }

        public override string ToString()
        {
            string msgName = WindowsMessages.GetNameByValue(Msg);
            string M = (msgName == null) ? Msg.ToString() : $"WM_{msgName}({Msg})";
            return $"<{M}[0x{Msg.ToString("x")}], {wParam}[0x{wParam.ToString("x")}], {lParam}[0x{lParam.ToString("x")}]>";
        }
    }
}
