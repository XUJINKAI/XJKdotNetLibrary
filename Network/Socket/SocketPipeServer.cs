using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XJK;
using XJK.Serializers;

namespace XJK.Network.Socket
{
    public class SocketPipeServer : ISocketPipe
    {
        public event RequestReceivedEventHandler RequestReceived;

        public string HostName { get; private set; }
        public int Port { get; private set; }

        private TcpListener Server;
        private Thread ListenThread;
        private readonly List<SocketPipeClient> ClientList = new List<SocketPipeClient>();

        public int ClientCount => ClientList.Count;

        public SocketPipeServer(int port) : this("127.0.0.1", port) { }

        public SocketPipeServer(string host, int port)
        {
            HostName = host;
            Port = port;
        }

        public void StopListen()
        {
            Debug.WriteLine($"Stop Listen {Server.LocalEndpoint}");
            ListenThread?.Abort();
            Server.Stop();
        }

        public void StartListen()
        {
            if (Server?.Server.Connected ?? false) throw new InvalidOperationException("Already connected");
            if (Server == null)
            {
                IPAddress localAddr = IPAddress.Parse(HostName);
                Server = new TcpListener(localAddr, Port);
            }
            Server.Start();
            ListenThread = ListenningLoopThread();
            ListenThread.Start();
        }

        public Thread ListenningLoopThread()
        {
            return new Thread(async() =>
            {
                Log.Info($"listen {Server.LocalEndpoint}");
                while (true)
                {
                    try
                    {
                        var tcpClient = await Server.AcceptTcpClientAsync();
                        SocketPipeClient client = new SocketPipeClient(tcpClient, $"Income[{ClientCount}]");
                        ClientList.Add(client);
                        client.Closed += (sender, args) =>
                        {
                            ClientList.Remove(client);
                        };
                        client.RequestReceived += async (sender, args) =>
                        {
                            await RequestReceived?.Invoke(this, args);
                        };
                        Log.Info($"new incomming client, count {ClientCount}");
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"Stop listen {Server.LocalEndpoint}");
                        break;
                    }
                }
            });
        }
        
        public async Task<List<byte[]>> BroadcastBytes(byte[] Bytes)
        {
            List<byte[]> Result = new List<byte[]>();
            List<Task> TaskList = new List<Task>();
            foreach (var Client in ClientList)
            {
                TaskList.Add(Task.Run(async () =>
                {
                    var response = await Client.SendBytes(Bytes);
                    Result.Add(response);
                }));
            }
            return await Task.Run(() =>
            {
                Task.WaitAll(TaskList.ToArray());
                return Result;
            });
        }
    }
}
