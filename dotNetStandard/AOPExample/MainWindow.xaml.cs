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
using XJK.AOP;

namespace AOPExample
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
        }

        private void Log_TextListener(string obj)
        {
            LogBox.Dispatcher.Invoke(() =>
            {
                LogBox.Text += obj;
            });
        }
        
        private async void MethodCallInfoInvokeObject(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            MethodCallInfo methodCallInfo = new MethodCallInfo()
            {
                Name = "ToBase64",
                Args = new List<object>()
                {
                    Helper.RandomString(20),
                }
            };
            var result = await methodCallInfo.InvokeAsync(client);
            Log.Info($"{result}");
        }

        private async void MethodCallInfoInvokeTask(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            MethodCallInfo methodCallInfo = new MethodCallInfo()
            {
                Name = "WriteLog",
                Args = new List<object>()
                {
                    Helper.RandomString(20),
                }
            };
            var result = await methodCallInfo.InvokeAsync(client);
            Log.Info($"{result}");
        }

        private async void MethodCallInfoInvokeTaskObject(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            MethodCallInfo methodCallInfo = new MethodCallInfo()
            {
                Name = "ToBase64Async",
                Args = new List<object>()
                {
                    Helper.RandomString(20),
                }
            };
            var result = await methodCallInfo.InvokeAsync(client);
            Log.Info(result);
        }

        private void InterceptSync(object sender, RoutedEventArgs e)
        {
            IInterceptTest interceptTest = InterceptTestClass.GetProxy();
            string ori = Helper.RandomString(20);
            string tra = interceptTest.ToBase64(ori);
            Log.Info($"{ori} => {tra}");
        }

        private async void InterceptAsync(object sender, RoutedEventArgs e)
        {
            IInterceptTest interceptTest = InterceptTestClass.GetProxy();
            string ori = Helper.RandomString(20);
            string tra = await interceptTest.ToBase64Async(ori);
            Log.Info($"{ori} => {tra}");
        }

        private async void ProxyTask(object sender, RoutedEventArgs e)
        {
            IClient IClient = Client.GetProxy();
            string ori = Helper.RandomString(20);
            await IClient.WriteLog(ori);
        }

        private async void ProxyTaskObject(object sender, RoutedEventArgs e)
        {
            IClient IClient = Client.GetProxy();
            string ori = Helper.RandomString(20);
            string tra = await IClient.ToBase64Async(ori);
            Log.Info($"{ori} => {tra}");
        }
    }
}
