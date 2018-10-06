using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XJK;
using XJK.PInvoke;
using XJK.SysX.Hooks;
using XJK.SysX.WinMsg;

namespace WpfWinMsgListenerExample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool Listen { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            App.Current.MainWindow = this;
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            FilterBox.Text = "13";
            WindowMessage.WindowMessageReceived += WindowMessage_WindowMessageReceived;
            Listen = true;
            ListenCheckBox.IsChecked = true;
            //RegisterHook(HookType.WH_KEYBOARD_LL);
        }

        private void RegisterHook(HookType hookType)
        {
            var x = new WindowsHookEx(hookType, (_, e) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Log(e.ToString());
                });
            });
        }

        private void Log(string text)
        {
            if (Listen)
            {
                TextBox.AppendText(text + Environment.NewLine);
                ScrollViewer.ScrollToEnd();
            }
        }

        private void WindowMessage_WindowMessageReceived(WindowMessageEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (!FilterBox.Text.Split(' ', ',', ';').Contains(e.Msg.ToString()))
                {
                    Log(e.ToString());
                }
            });
        }

        private void ClearBox(object sender, RoutedEventArgs e)
        {
            TextBox.Clear();
        }

        private void BroadcastMsg(object sender, RoutedEventArgs e)
        {
            WindowMessage.BroadcastMessage(User32.RegisterWindowMessage("WpfWinMsgListenerExample_BROADCAST_MSG"), Helper.RandomString(20));
        }
    }
}
