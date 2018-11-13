using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;

namespace XJK.SysX.WinMsg
{
    public delegate void WndMsgEventHandler(object sender, WndMsgEventArgs e);

    public class WndMsgEventArgs : EventArgs
    {
#pragma warning disable IDE1006 // 命名样式
        public bool Handled { get; set; }
        public int Msg { get; private set; }
        public IntPtr wParam { get; private set; }
        public IntPtr lParam { get; private set; }
#pragma warning restore IDE1006 // 命名样式

        public WndMsgEventArgs(int msg, IntPtr wparam, IntPtr lparam)
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
