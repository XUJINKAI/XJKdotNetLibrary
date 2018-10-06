using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using XJK.PInvoke;

namespace XJK.SysX.WinMsg
{
    public class WndProcHelperWindow : Window
    {
        private static WndProcHelperWindow Instance = null;
        private static WindowMessageEvent WndMsgProcDelegate;
        public static event WindowMessageEvent WndProcEvent
        {
            add
            {
                WndMsgProcDelegate += value;
                if (Instance == null) Instance = new WndProcHelperWindow();
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
        
        private WndProcHelperWindow()
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
            WindowMessageEventArgs arg = new WindowMessageEventArgs(msg, wParam, lParam);
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
