using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;

namespace XJK.SysX.WinMsg
{
    public delegate void WndMsgPowerBroadcastEventHandler(object sender, WndMsgPowerBroadcastEventArgs e);

    public class WndMsgPowerBroadcastEventArgs : WndMsgEventArgs
    {
        public PBT PBT => (PBT)wParam;

        public WndMsgPowerBroadcastEventArgs(WndMsgEventArgs e)
            : base(e.Msg, e.wParam, e.lParam)
        {

        }
    }
}
