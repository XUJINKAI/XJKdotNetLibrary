using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private RunType _runType = RunType.Invoker;
        private WindowType _windowType = WindowType.Show;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public MainWindow()
        {
            InitializeComponent();
            Log.ListenSystemDiagnosticsLog();
            Log.TextListener += LogText;
            this.Title = ENV.IsAdministrator() ? "Administrator" : "Normal User";
            this.CommandBox.Text = ENV.ExePath; //"D:\\space space.exe";
            foreach (var x in Environment.GetCommandLineArgs())
            {
                LogText(x + C.LF);
            }
        }

        private void LogText(string obj)
        {
            LogBox.Dispatcher.Invoke(() =>
            {
                LogBox.Text += obj;
            });
        }

        private void CatchException(Exception ex)
        {

        }

        public string Command => CommandBox.Text;
        public string Args => ArgsBox.Text;
        public RunType RunType
        {
            get => _runType; set
            {
                _runType = value;
                OnPropertyChanged("RunType");
            }
        }
        public WindowType WindowType
        {
            get => _windowType; set
            {
                _windowType = value;
                OnPropertyChanged("WindowType");
            }
        }

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

        private async void RunCmdResult(object sender, RoutedEventArgs e)
        {
            var x = await Cmd.RunCmdResultAsync(Command, Args);
            LogText(x);
        }

        private void RunRadioOption(object sender, RoutedEventArgs e)
        {
            ProcessInfoChain.New(Command, Args).RunAs(RunType).Window(WindowType).Catch(CatchException).Start();
        }

        private void ParseArgs(object sender, RoutedEventArgs e)
        {
            string x = CommandBox.Text;
            var tuple = Cmd.SplitCommandArg(x);
            LogText($"Command :{tuple.Item1}{C.LF}Arg     :{tuple.Item2}{C.LF}");
        }

        private void SetCurrentExePath(object sender, RoutedEventArgs e)
        {
            CommandBox.Text = ENV.ExePath;
        }
    }
}
