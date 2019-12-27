using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XJK;
using XJK.Win32.WinMsg;

namespace Win32_Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WinMsgListenerWindow : Window
    {
        public bool Listen { get; set; }
        private readonly WindowEx msgWin;

        public WinMsgListenerWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            App.Current.MainWindow = this;
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            FilterBox.Text = "13";
            msgWin = new WindowEx().ShowInvisible(this);
            msgWin.AllMsg += WindowEx_AllMsg;
            Listen = true;
            ListenCheckBox.IsChecked = true;
            //RegisterHook(HookType.WH_KEYBOARD_LL);
        }

        private void WindowEx_AllMsg(object sender, WndMsgEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (!FilterBox.Text.Split(' ', ',', ';').Contains(e.Msg.ToString()))
                {
                    Log(e.ToString());
                }
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

        private void ClearBox(object sender, RoutedEventArgs e)
        {
            TextBox.Clear();
        }

        private void BroadcastMsg(object sender, RoutedEventArgs e)
        {
            WindowCommunicate.BroadcastMessage("WpfWinMsgListenerExample_BROADCAST_MSG", RandomGenerator.RandomString(20));
        }
    }
}
