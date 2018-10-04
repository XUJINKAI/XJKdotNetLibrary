using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;

namespace XJK.WPF
{
    public delegate void WndMsgProc(WndMsgEventArgs e);

    public class WndMsgEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public int Msg { get; set; }

#pragma warning disable IDE1006 // 命名样式
        public IntPtr wParam { get; set; }
        public IntPtr lParam { get; set; }

        public int wParamInt => wParam.ToInt32();
        public int lParamInt => lParam.ToInt32();
#pragma warning restore IDE1006 // 命名样式

        public override string ToString()
        {
            string msgName = WindowsMessages.GetNameByValue(Msg);
            string M = (msgName == null) ? Msg.ToString() : $"WM_{msgName}({Msg})";
            return $"<{M}[0x{Msg.ToString("x")}], {wParam}[0x{wParam.ToString("x")}], {lParam}[0x{lParam.ToString("x")}]>";
        }
    }

}
