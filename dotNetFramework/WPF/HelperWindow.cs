using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using XJK.PInvoke;

namespace XJK.WPF
{
    public class WinMsgHelperWindow : Window
    {
        private static WinMsgHelperWindow Instance = null;
        private static WndMsgProc WndMsgProcDelegate;
        public static event WndMsgProc WndProcReceived
        {
            add
            {
                WndMsgProcDelegate += value;
                if (Instance == null) Instance = new WinMsgHelperWindow();
            }
            remove
            {
                WndMsgProcDelegate -= value;
                if (WndMsgProcDelegate.GetInvocationList().Length == 0)
                {
                    Instance.Close();
                    Instance = null;
                }
            }
        }
        
        private WinMsgHelperWindow()
        {
            WindowStyle = WindowStyle.None;
            Opacity = 0;
            Top = int.MinValue;
            Left = int.MinValue;
            Width = 0;
            Height = 0;
            Show();
            Visibility = Visibility.Hidden;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (WndMsgProcDelegate == null) return IntPtr.Zero;
            WndMsgEventArgs arg = new WndMsgEventArgs()
            {
                Handled = false,
                Msg = msg,
                wParam = wParam,
                lParam = lParam,
            };
            foreach (var dele in WndMsgProcDelegate.GetInvocationList())
            {
                dele.DynamicInvoke(arg);
                if (arg.Handled)
                {
                    handled = true;
                    break;
                }
            }
            return IntPtr.Zero;
        }
    }
}
