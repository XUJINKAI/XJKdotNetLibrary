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

namespace CmdRun
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Log.ListenSystemDiagnosticsLog();
            Log.TextListener += Log_TextListener;
            IsAdminTextBox.Text = ENV.IsAdministrator() ? "Administrator" : "Normal User";
            CurrentExePath.Text = ENV.ExePath;
            CommandBox.Text = ENV.ExePath; //"D:\\space space.exe";
            ArgsBox.Text = "";
            ShowWindowCheckBox.IsChecked = false;
            foreach(var x in Environment.GetCommandLineArgs())
            {
                LogBox.Text += x + C.LF;
            }
        }

        private void Log_TextListener(string obj)
        {
            LogBox.Dispatcher.Invoke(() =>
            {
                LogBox.Text += obj;
            });
        }

        public string Command => CommandBox.Text;
        public string Args => ArgsBox.Text;
        public bool ShowWindow => ShowWindowCheckBox.IsChecked ?? false;

        private void RunAsInvoker(object sender, RoutedEventArgs e)
        {
            Cmd.RunAsInvoker(Command, Args);
        }

        private void RunAsAdmin(object sender, RoutedEventArgs e)
        {
            Cmd.RunAsAdmin(Command, Args);
        }

        private void RunAsLimitedPrevilege(object sender, RoutedEventArgs e)
        {
            Cmd.RunAsLimitedPrivilege(Command, Args);
        }

        private void RunWithCmdStart(object sender, RoutedEventArgs e)
        {
            Cmd.RunWithCmdStart(Command, Args);
        }

        private void DumpFile(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(new ProcessStartInfo(Command).Dump());
        }

        private void ParseArgsBox(object sender, RoutedEventArgs e)
        {
            foreach (var x in CommandLineHelper.CommandLineToArgs(Args))
            {
                LogBox.Text += x + C.LF;
            }
        }

        private async void RunCmdResult(object sender, RoutedEventArgs e)
        {
            await Cmd.RunCmdResultAsync(Command, Args);
        }
    }
}
