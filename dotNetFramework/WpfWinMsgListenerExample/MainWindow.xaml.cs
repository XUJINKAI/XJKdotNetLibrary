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
using XJK.WPF;

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
            WinMsg.WndMsgProc += WinMsg_WndMsgProc;
            Listen = true;
            ListenCheckBox.IsChecked = true;
        }

        private void Log(string text)
        {
            if (Listen)
            {
                TextBox.AppendText(text + Environment.NewLine);
                ScrollViewer.ScrollToEnd();
            }
        }

        private void WinMsg_WndMsgProc(WndMsgEventArgs e)
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
    }
}
