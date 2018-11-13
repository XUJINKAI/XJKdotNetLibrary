using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;

namespace XJK.SysX.WinMsg
{
    public delegate void WndMsgCommEventHandler(object sender, WndMsgCommEventArgs e);

    public class WndMsgCommEventArgs : WndMsgEventArgs
    {
        public string MsgBody => Marshal.PtrToStringAuto(lParam);

        public WndMsgCommEventArgs(WndMsgEventArgs e)
            : base(e.Msg, e.wParam, e.lParam)
        {

        }
    }
}
