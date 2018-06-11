using System;
using System.Windows;
using System.Windows.Interop;

namespace XJK.WPF.WpfWindow
{
    public class AppHelperWindow : Window
    {
        public event Action<int> RecivingMsg;

        public AppHelperWindow()
        {
            Visibility = Visibility.Hidden;
            Opacity = 0;
            Top = 0;
            Left = 0;
            Width = 0;
            Height = 0;
            WindowStyle = WindowStyle.ToolWindow;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(new HwndSourceHook(WndProc));

            Visibility = Visibility.Hidden;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            RecivingMsg?.Invoke(msg);
            return IntPtr.Zero;
        }
    }
}
