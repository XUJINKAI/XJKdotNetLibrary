using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XJK.Serializers;

namespace XJK.Network.Socket
{
    public class SocketPipeClient: SocketBase
    {
        public string Name { get; set; } = "";
        public string HostName { get; private set; }
        public int Port { get; private set; }

        public TcpClient Client { get; private set; }
        private Thread ListenThread;

        public SocketPipeClient(TcpClient tcpClient, string name = "")
        {
            Name = name;
            Client = tcpClient;
            ListenThread = ReceivingLoopThread(Client, Name, client =>
            {
                Close();
            });
            ListenThread.Start();
        }

        public SocketPipeClient(int port) : this("127.0.0.1", port) { }

        public SocketPipeClient(string host, int port)
        {
            HostName = host;
            Port = port;
        }

        public void Connect()
        {
            Log.Info($"[{Name}] Connect {HostName}:{Port}");
            if (Client?.Connected ?? false) throw new InvalidOperationException("Already connected");
            if(Client == null)
            {
                Client = new TcpClient(HostName, Port);
            }
            else
            {
                Client.Connect(HostName, Port);
            }
            ListenThread = ReceivingLoopThread(Client, Name, client =>
            {
                Close();
            });
            ListenThread.Start();
        }

        public void Close()
        {
            Debug.WriteLine($"[{Name}] Close Client: Current Thread ID '{Thread.CurrentThread.ManagedThreadId}', Close Thread ID '{ListenThread?.ManagedThreadId}'");
            ListenThread?.Abort();
            if (Client != null) OnClose(Client);
        }
        
        public async Task<byte[]> SendBytes(byte[] Bytes)
        {
            if (!Client.Connected) throw new Exception($"[{Name}] Can't send cause client not connected.");
            Log.Debug($"[{Name}] SendBytes byte[{Bytes.Length}]");
            int id = await SendRequestAsync(Client, Bytes);
            var response = await GetResponseAsync(id);
            Log.Debug($"[{Name}] ResponseBytes byte[{response.Length}]");
            return response;
        }

        public async Task<string> SendString(string str)
        {
            var response = await SendBytes(str.ConvertBytes());
            return response.ConvertString();
        }

    }
}
