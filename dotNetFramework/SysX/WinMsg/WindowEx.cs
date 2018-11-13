using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using XJK.PInvoke;
using XJK.SysX.Hooks;

namespace XJK.SysX.WinMsg
{
    public class WindowEx : Window
    {
        public IntPtr Handle { get; private set; }
        public event WndMsgEventHandler MsgHotkey;
        public event WndMsgEventHandler MsgClipboardChanged;
        public event WndMsgPowerBroadcastEventHandler MsgPowerBroadcast;
        public event WndMsgEventHandler MsgDefault;
        public event WndMsgEventHandler AllMsg;

        private WindowCommunicate _wndMsg;
        public WindowCommunicate WndMsg
        {
            get
            {
                if (_wndMsg == null) _wndMsg = new WindowCommunicate(this);
                return _wndMsg;
            }
        }

        private SystemHotkey _systemHotkey;
        public SystemHotkey SystemHotkey
        {
            get
            {
                if (_systemHotkey == null) _systemHotkey = new SystemHotkey(this);
                return _systemHotkey;
            }
        }

        protected void OnWndMsgReceived(WndMsgEventArgs e)
        {
            AllMsg?.Invoke(this, e);
            switch (e.Msg)
            {
                case WindowsMessages.HOTKEY:
                    MsgHotkey?.Invoke(this, e);
                    break;
                case WindowsMessages.CLIPBOARDUPDATE:
                    MsgClipboardChanged?.Invoke(this, e);
                    break;
                case WindowsMessages.POWERBROADCAST:
                    MsgPowerBroadcast?.Invoke(this, new WndMsgPowerBroadcastEventArgs(e));
                    break;
                default:
                    MsgDefault?.Invoke(this, e);
                    break;
            }
        }

        public WindowEx ShowInvisible(Window closeRelatedWindow)
        {
            ShowInvisible();
            closeRelatedWindow.Closed += (s, e) =>
            {
                this.Close();
            };
            return this;
        }

        private void ShowInvisible()
        {
            ShowActivated = false;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
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
            Handle = new WindowInteropHelper(this).Handle;
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(new HwndSourceHook(WndProcHook));
            // Clipboard
            User32.AddClipboardFormatListener(Handle);
        }
        
        private IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var e = new WndMsgEventArgs(msg, wParam, lParam);
            OnWndMsgReceived(e);
            if (e.Handled)
            {
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void RegisterAutoRestart(WndMsgEventHandler shutdownAction, string LaunchParameter = "")
        {
            MsgDefault += (s, e) =>
            {
                if (e.Msg == WindowsMessages.QUERYENDSESSION)
                {
                    Kernel32.RegisterApplicationRestart(LaunchParameter, 0);
                }
                if (e.Msg == WindowsMessages.ENDSESSION)
                {
                    shutdownAction(this, e);
                }
            };
        }

    }
}
