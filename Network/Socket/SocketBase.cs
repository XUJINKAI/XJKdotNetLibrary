using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XJK.Serializers;

namespace XJK.Network.Socket
{
    public delegate void TcpClientClosedEventHandler(object sender, TcpClientCloseEventArgs args);

    public class TcpClientCloseEventArgs : EventArgs
    {
        public TcpClient TcpClient { get; private set; }

        public TcpClientCloseEventArgs(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }
    }

    public abstract class SocketBase : ISocketPipe
    {
        public event RequestReceivedEventHandler RequestReceived;
        public event TcpClientClosedEventHandler Closed;

        public async Task OnRequestReceived(byte[] ReceiveBytes, TcpClient tcpClient, int MsgId)
        {
            SocketRequestReceivedEventArgs SocketRequest = new SocketRequestReceivedEventArgs(ReceiveBytes, async bytes =>
            {
                await SendResponseAsync(tcpClient, MsgId, bytes);
                return SocketStatus.Success;
            });
            await RequestReceived?.Invoke(this, SocketRequest);
            await SocketRequest.Finish();
        }

        public void OnClose(TcpClient tcpClient)
        {
            tcpClient.Close();
            TcpClientCloseEventArgs SocketCloseEventArgs = new TcpClientCloseEventArgs(tcpClient);
            Closed?.Invoke(this, SocketCloseEventArgs);
        }

        private static int IdPtr = 0;


        protected Dictionary<int, TaskCompletionSource<byte[]>> TaskCompletionDict { get; private set; } = new Dictionary<int, TaskCompletionSource<byte[]>>();
        
        protected Thread ReceivingLoopThread(TcpClient tcpClient, string Name, Action<TcpClient> OnClose)
        {
            return new Thread(async () =>
            {
                Debug.WriteLine($"ReceivingLoopThread ID '{Thread.CurrentThread.ManagedThreadId}'");
                while (tcpClient.Connected)
                {
                    Debug.WriteLine($"Client '{Name}' looping...");
                    Tuple<SocketHeader, byte[]> tuple = new Tuple<SocketHeader, byte[]>(new SocketHeader(), new byte[0]);
                    try
                    {
                        tuple = await ReadStreamAsync(tcpClient);
                    }
                    catch (Exception ex)
                    {
                        Debugger.Break();
                    }
                    int id = tuple.Item1.ID;
                    byte[] ReceiveBytes = tuple.Item2;
                    if (tuple.Item1.Type == SocketType.Request)
                    {
                        await OnRequestReceived(ReceiveBytes, tcpClient, id);
                    }
                    else if (tuple.Item1.Type == SocketType.Response)
                    {
                        var taskComp = TaskCompletionDict[id];
                        taskComp.SetResult(ReceiveBytes);
                    }
                    else if (tuple.Item1.Type == SocketType.Shutdown)
                    {
                        Debug.WriteLine($"Client '{Name}' disconnected.");
                        break;
                    }
                }
                OnClose(tcpClient);
            });
        }

        protected async Task<int> SendRequestAsync(TcpClient Client, byte[] Bytes)
        {
            SocketHeader header = new SocketHeader()
            {
                ID = IdPtr++,
                Length = Bytes.Length,
                Type = SocketType.Request,
            };
            var result = await WriteStreamAsync(Client, header, Bytes);
            return header.ID;
        }

        protected async Task<bool> SendResponseAsync(TcpClient Client, int ID, byte[] Bytes)
        {
            SocketHeader header = new SocketHeader()
            {
                ID = ID,
                Length = Bytes.Length,
                Type = SocketType.Response,
            };
            var result = await WriteStreamAsync(Client, header, Bytes);
            return true;
        }

        protected async Task<byte[]> GetResponseAsync(int ID)
        {
            var taskCompletionSource = new TaskCompletionSource<byte[]>();
            TaskCompletionDict.Add(ID, taskCompletionSource);
            return await taskCompletionSource.Task;
        }


        private async Task<bool> WriteStreamAsync(TcpClient Client, SocketHeader header, byte[] Bytes)
        {
            //Debug.WriteLine($"[WriteStream] {header}, {Bytes.Join(o => o.ToString())}");
            NetworkStream NetworkStream = Client.GetStream();
            return await Task.Run(() =>
            {
                NetworkStream.Write(header.ToBytesArray(), 0, SocketHeader.Size);
                NetworkStream.Write(Bytes, 0, Bytes.Length);
                return true;
            });
        }

        private Task<Tuple<SocketHeader, byte[]>> ReadStreamAsync(TcpClient Client)
        {
            NetworkStream NetworkStream = Client.GetStream();
            return Task.Run(() =>
            {
                byte[] headerBytes = new byte[SocketHeader.Size];
                try
                {
                    int _ = NetworkStream.Read(headerBytes, 0, SocketHeader.Size);
                }
                catch (Exception ex)
                {
                    Exception inner = ex.InnerException;
                    if (inner is SocketException socketException && socketException.ErrorCode == 10004)
                    {
                        return new Tuple<SocketHeader, byte[]>(new SocketHeader() { Type = SocketType.Shutdown}, new byte[0]);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                SocketHeader header = SocketHeader.FromBytes(headerBytes);
                if (headerBytes.All(b => b == 0))
                {
                    // shutdown
                    header.Type = SocketType.Shutdown;
                    return new Tuple<SocketHeader, byte[]>(header, new byte[0]);
                }

                int length = header.Length;
                byte[] result = new byte[length];
                if (length > 0)
                {
                    int _ = NetworkStream.Read(result, 0, length);
                }
                //Debug.WriteLine($"[ReadStream] {header}, {result.Join(o => o.ToString())}");
                var tuple = Tuple.Create(header, result);
                return tuple;
            });
        }

    }
}
