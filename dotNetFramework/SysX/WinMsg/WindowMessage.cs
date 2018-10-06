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
    public static class WindowMessage
    {
        public static void BroadcastMessage(int msg, string MsgBody = "")
        {
            User32.SendMessage(
                SpecialWindowHandles.HWND_BROADCAST,
                msg,
                IntPtr.Zero,
                MsgBody);
        }

        public static event WindowMessageEvent WindowMessageReceived
        {
            add
            {
                WndProcHelperWindow.WndProcEvent += value;
            }
            remove
            {
                WndProcHelperWindow.WndProcEvent -= value;
            }
        }

        public static void RegisterEvent(int msg, WindowMessageEvent action)
        {
            WindowMessageReceived += e =>
            {
                if (e.Msg == msg)
                {
                    action(e);
                }
            };
        }

        public static void RegisterAutoRestart(WindowMessageEvent shutdownAction, string LaunchParameter = "")
        {
            WindowMessageReceived += e =>
            {
                if (e.Msg == WindowsMessages.QUERYENDSESSION)
                {
                    Kernel32.RegisterApplicationRestart(LaunchParameter, 0);
                }
                if (e.Msg == WindowsMessages.ENDSESSION)
                {
                    shutdownAction(e);
                }
            };
        }

        public static void RegisterPowerBroadcast(WindowMessagePowerBroadcastEvent wndMsgProc)
        {
            WindowMessageReceived += e =>
            {
                if (e.Msg == WindowsMessages.POWERBROADCAST)
                {
                    wndMsgProc((PBT)e.wParam, e);
                }
            };
        }
    }
}
