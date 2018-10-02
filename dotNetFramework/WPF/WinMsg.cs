using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using XJK.WPF.WpfWindow;
using XJK.PInvoke;

namespace XJK.WPF
{
    public static class WinMsg
    {
        public static AppHelperWindow Window { get; private set; }
        private static readonly Dictionary<string, int> RegisteredMessage = new Dictionary<string, int>();
        private static readonly Dictionary<int, Action> MsgActionDict = new Dictionary<int, Action>();

        public static bool AutoRestart { get; private set; } = false;
        private static string AutoRestartParameter = "";
        private static Action AutoRestartShutdownAction;

        #region Message

        public static void BroadcastMessage(string msgId, string MsgBody = "")
        {
            User32.SendMessage(
                SpecialWindowHandles.HWND_BROADCAST,
                GetMesssageId(msgId),
                IntPtr.Zero,
                MsgBody);
        }
        
        public static int GetMesssageId(string msg)
        {
            if (RegisteredMessage.ContainsKey(msg))
            {
                return RegisteredMessage[msg];
            }
            else
            {
                int msgid = User32.RegisterWindowMessage(msg);
                RegisteredMessage[msg] = msgid;
                return msgid;
            }
        }

        #endregion

        public static void RegisterReciveMessage(string msg, Action action)
        {
            EnsureHelperWindow();
            int msgid = GetMesssageId(msg);
            MsgActionDict[msgid] = action;
        }


        public static void RegisterAutoRestart(Action shutdownAction, string Parameter = "")
        {
            Debug.Assert(shutdownAction != null, "");
            EnsureHelperWindow();
            AutoRestartShutdownAction = shutdownAction;
            AutoRestartParameter = Parameter;
            AutoRestart = true;
        }

        private static void EnsureHelperWindow()
        {
            if (Window == null)
            {
                Window = new AppHelperWindow();
                Window.RecivingMsg += AppHelperWindow_RecivingMsg;
                Window.Show();
            }
        }

        public static void DisposeHelperWindow()
        {
            if (Window != null)
            {
                Window.RecivingMsg -= AppHelperWindow_RecivingMsg;
                Window.Close();
                Window = null;
            }
        }
        
        private static void AppHelperWindow_RecivingMsg(int msg)
        {
            if (AutoRestart)
            {
                if (msg == WM.QUERYENDSESSION)
                {
                    Kernel32.RegisterApplicationRestart(AutoRestartParameter, 0);
                }
                if (msg == WM.ENDSESSION)
                {
                    AutoRestartShutdownAction();
                }
            }
            if (MsgActionDict.ContainsKey(msg))
            {
                Action action = MsgActionDict[msg];
                action();
            }
        }
    }
}
