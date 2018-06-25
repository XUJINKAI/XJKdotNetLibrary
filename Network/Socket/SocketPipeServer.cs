using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XJK;
using XJK.Serializers;

namespace XJK.Network.Socket
{
    public class SocketPipeServer : ISocketPipe
    {
        public event RequestReceivedEventHandler RequestReceived;

        private TcpListener Server;
        private readonly List<SocketPipeClient> ClientList = new List<SocketPipeClient>();

        public int ClientCount => ClientList.Count;

        public SocketPipeServer(int port) : this("127.0.0.1", port) { }

        public SocketPipeServer(string host, int port)
        {
            IPAddress localAddr = IPAddress.Parse(host);
            Server = new TcpListener(localAddr, port);
            StartListen();
            Log.Info($"listen {host}:{port}");
        }

        public void StopListen()
        {
            Server.Stop();
            
        }

        public async void StartListen()
        {
            Server.Start();
            while (true)
            {
                var tcpClient = await Server.AcceptTcpClientAsync();
                SocketPipeClient client = new SocketPipeClient(tcpClient)
                {
                    Name = $"Income[{ClientCount}]",
                };
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
