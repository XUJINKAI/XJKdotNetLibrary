using System.Runtime.InteropServices;

namespace XJK.Win32.WinMsg
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
