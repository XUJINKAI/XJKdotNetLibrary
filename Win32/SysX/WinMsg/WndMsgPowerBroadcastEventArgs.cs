using XJK.Win32.PInvoke;

namespace XJK.Win32.SysX.WinMsg
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
