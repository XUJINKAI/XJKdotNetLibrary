using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using XJK.Objects;
using XJK.PInvoke;

namespace XJK.SysX.WinMsg
{
    public class WindowCommunicate : DisposeBase
    {
        public WindowEx Window { get; private set; }
        private readonly Dictionary<int, WndMsgCommEventHandler> DelegatesDict = new Dictionary<int, WndMsgCommEventHandler>();

        public WindowCommunicate(WindowEx windowEx)
        {
            Window = windowEx;
            Window.MsgDefault += Window_MsgProc;
        }

        protected override void OnDispose()
        {
            Window.MsgDefault -= Window_MsgProc;
            Window = null;
        }

        private void Window_MsgProc(object sender, WndMsgEventArgs e)
        {
            if (DelegatesDict.ContainsKey(e.Msg))
            {
                DelegatesDict[e.Msg].Invoke(sender, new WndMsgCommEventArgs(e));
            }
        }

        public void RegisterEvent(string msg, WndMsgCommEventHandler action) => RegisterEvent(GetMsgId(msg), action);
        public void RegisterEvent(int msg, WndMsgCommEventHandler action)
        {
            if (DelegatesDict.ContainsKey(msg))
            {
                DelegatesDict[msg] += action;
            }
            else
            {
                DelegatesDict[msg] = action;
            }
        }

        public void UnregisterEvent(string msg, WndMsgCommEventHandler action) => UnregisterEvent(GetMsgId(msg), action);
        public void UnregisterEvent(int msg, WndMsgCommEventHandler action)
        {
            if (DelegatesDict.ContainsKey(msg))
            {
                DelegatesDict[msg] -= action;
            }
        }


        public static int GetMsgId(string msg) => User32.RegisterWindowMessage(msg);
        public static int BroadcastMessage(string msgCode, string MsgBody = "") => SendMessage(SpecialWindowHandles.HWND_BROADCAST, GetMsgId(msgCode), MsgBody);
        public static int BroadcastMessage(int msgId, string MsgBody = "") => SendMessage(SpecialWindowHandles.HWND_BROADCAST, msgId, MsgBody);
        public static int SendMessage(IntPtr handle, string msgCode, string MsgBody = "") => SendMessage(handle, GetMsgId(msgCode), MsgBody);
        public static int SendMessage(IntPtr handle, int msgId, string MsgBody = "") { User32.SendMessage(handle, msgId, IntPtr.Zero, MsgBody); return msgId; }

    }
}
