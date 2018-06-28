using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using XJK.SysX;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title += $", Admin({ENV.IsAdministrator()})";
            Log.ListenSystemDiagnosticsLog();
            Log.TextListener += Log_TextListener;
        }

        private void Log_TextListener(string obj)
        {
            LogBox.Dispatcher.Invoke(() =>
            {
                LogBox.Text += obj;
            });
        }

        private void Break(object sender, RoutedEventArgs e)
        {
            Debugger.Break();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            LogBox.Text = "";
        }

        private void LogBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LogBox.SelectedText = C.LF;
                e.Handled = true;
            }
        }

        public void TestDumpObject(object sender = null, RoutedEventArgs e = null)
        {
            DumpObject.Test();
        }
        
        private void Test(object sender, RoutedEventArgs e)
        {
            int local1 = 222;
            string local2 = "yyy";
            DumpObject.ThrowException();
        }

    }
}
