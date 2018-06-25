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
using XJK.Network;
using XJK.Network.Socket;
using XJK.Serializers;
using XJK.SysX;

namespace Web
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Log.LogLocation = true;
            Log.ListenSystemDiagnosticsLog();
            Log.TextListener += Log_TextListener;
            RandMsg();
        }

        private void OpenNewThread(object sender, RoutedEventArgs e)
        {
            Cmd.RunSmart(ENV.ExePath);
        }

        private void ClearLog(object sender, RoutedEventArgs e)
        {
            LogBox.Text = "";
        }

        private void RandMsg(object sender = null, RoutedEventArgs e = null)
        {
            MsgBox.Text = Helper.RandomString(Helper.RandomInt(3, 10));
        }

        private void Log_TextListener(string obj)
        {
            LogBox.Dispatcher.Invoke(() =>
            {
                LogBox.Text += obj;
            });
        }

        private int Port
        {
            get
            {
                string text = SocketPortBox.Text;
                bool success = int.TryParse(text, out int result);
                if (success) return result;
                else { Log.Error($"Parse Port Error {text}"); return 0; }
            }
        }
        private string Msg => MsgBox.Text;

        SocketPipeServer SocketPipeServer;
        SocketPipeClient SocketPipeClient;

        private void SocketServerNew(object sender, RoutedEventArgs e)
        {
            Log.Info("New server");
            SocketPipeServer = new SocketPipeServer(Port);
            SocketPipeServer.RequestReceived += SocketPipeServer_RequestReceived;
        }

        private void SocketServerStart(object sender, RoutedEventArgs e)
        {
            SocketPipeServer.StartListen();
        }

        private void SocketServerClose(object sender, RoutedEventArgs e)
        {
            SocketPipeServer.StopListen();
        }

        private async Task SocketPipeServer_RequestReceived(object sender, SocketRequestReceivedEventArgs args)
        {
            string msg = args.Message;
            Log.Info($"Server Receive '{msg}', response '{msg.Length}'");
            await args.SendResponseAsync(msg.Length.ToString());
        }


        private void SocketClientNew(object sender, RoutedEventArgs e)
        {
            Log.Info("New client");
            SocketPipeClient = new SocketPipeClient(Port);
            SocketPipeClient.RequestReceived += SocketPipeClient_RequestReceived;
        }

        private void SocketClientConnect(object sender, RoutedEventArgs e)
        {
            SocketPipeClient.Connect();
        }

        private void SocketClientClose(object sender, RoutedEventArgs e)
        {
            SocketPipeClient.Close();
        }

        private async Task SocketPipeClient_RequestReceived(object sender, SocketRequestReceivedEventArgs args)
        {
            string msg = args.Message;
            Log.Info($"Client Receive '{msg}', response '{msg.Length}'");
            await args.SendResponseAsync(msg.Length.ToString());
        }


        private async void SendMsg(object sender, RoutedEventArgs e)
        {
            Log.Info($"Client Send '{Msg}'");
            var response = await SocketPipeClient.SendString(Msg);
            Log.Info($"Client Result '{response}'");
        }

        private async void ServerBroadcastMsg(object sender, RoutedEventArgs e)
        {
            Log.Info($"Server Broadcast '{Msg}'");
            var list = await SocketPipeServer.BroadcastBytes(Msg.ConvertBytes());
            Log.Info($"Server Result List({list.Count}), {list.Join(o => o.ConvertString())}");
        }

        private void TestPort(object sender, RoutedEventArgs e)
        {
            string s = NetHelper.PortIsUsed(Port) ? "Used" : "Not Used";
            Log.Info($"Port {Port} is {s}");
        }

        private void UnusedPort(object sender, RoutedEventArgs e)
        {
            int port = NetHelper.GetAvailablePort(Port);
            Log.Info($"Available port: {port}");
        }
    }
}
