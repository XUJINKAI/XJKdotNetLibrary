using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using XJK.PInvoke;

namespace XJK.WPF
{
    public delegate void WndMsgProcPowerBroadcast(PBT pbt, WndMsgEventArgs e);

    public static class WinMsg
    {
        public static event WndMsgProc WndMsgProc;

        static WinMsg()
        {
            WinMsgHelperWindow.WndProcReceived += WinMsgHelperWindow_WndProcReceived;
        }

        private static void WinMsgHelperWindow_WndProcReceived(WndMsgEventArgs e)
        {
            WndMsgProc?.Invoke(e);
        }

        public static void AddEvent(WndMsgProc wndMsgProc)
        {
            WndMsgProc += wndMsgProc;
        }

        public static void AddEvent(int msg, WndMsgProc action)
        {
            void hanlder(WndMsgEventArgs e)
            {
                if (e.Msg == msg)
                {
                    action(e);
                }
            }
            WndMsgProc += hanlder;
        }
        
        public static void RegisterAutoRestart(WndMsgProc shutdownAction, string LaunchParameter = "")
        {
            Debug.Assert(shutdownAction != null, "shutdownAction Can't be null.");
            void hanlder(WndMsgEventArgs e)
            {
                if (e.Msg == WindowsMessages.QUERYENDSESSION)
                {
                    Kernel32.RegisterApplicationRestart(LaunchParameter, 0);
                }
                if (e.Msg == WindowsMessages.ENDSESSION)
                {
                    shutdownAction(e);
                }
            }
            WndMsgProc += hanlder;
        }

        public static void RegisterPowerBroadcast(WndMsgProcPowerBroadcast wndMsgProc)
        {
            AddEvent(WindowsMessages.POWERBROADCAST, e =>
            {
                wndMsgProc((PBT)e.wParam, e);
            });
        }

        public static void BroadcastMessage(int msg, string MsgBody = "")
        {
            User32.SendMessage(
                SpecialWindowHandles.HWND_BROADCAST,
                msg,
                IntPtr.Zero,
                MsgBody);
        }

    }
}
