using System;
using System.Windows;
using System.Windows.Interop;
using XJK.Win32.Hooks;
using XJK.Win32.PInvoke;

namespace XJK.Win32.WinMsg
{
    public class WindowEx : Window
    {
        public IntPtr Handle { get; private set; }
        public event WndMsgEventHandler MsgHotkey;
        public event WndMsgEventHandler MsgClipboardChanged;
        public event WndMsgPowerBroadcastEventHandler MsgPowerBroadcast;
        public event WndMsgEventHandler AllMsg;

        private WindowCommunicate _windowCommunicate;
        public WindowCommunicate WindowCommunicate
        {
            get
            {
                if (_windowCommunicate == null) _windowCommunicate = new WindowCommunicate(this);
                return _windowCommunicate;
            }
        }

        private SystemHotkeyHook _systemHotkey;
        public SystemHotkeyHook SystemHotkey
        {
            get
            {
                if (_systemHotkey == null) _systemHotkey = new SystemHotkeyHook(this);
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
            }
        }

        public WindowEx ShowInvisible(Window RelatedWindow)
        {
            ShowInvisible();
            if (RelatedWindow != null)
            {
                RelatedWindow.Closed += (s, e) =>
                {
                    this.Close();
                };
            }
            return this;
        }

        private void ShowInvisible()
        {
            ShowActivated = false;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
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
            AllMsg += (s, e) =>
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
