using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XJK.Network.Socket
{
    public interface ISocketPipe
    {
        event RequestReceivedEventHandler RequestReceived;
    }

    public delegate Task RequestReceivedEventHandler(object sender, SocketRequestReceivedEventArgs args);

    public class SocketRequestReceivedEventArgs : EventArgs
    {
        public byte[] RawBytes { get; private set; }
        public string Message => RawBytes.ConvertString();

        private readonly Func<byte[], Task<SocketStatus>> SendResponseFunc;
        private bool ResponseSended = false;

        public SocketRequestReceivedEventArgs(byte[] bytes, Func<byte[], Task<SocketStatus>> responseFunc)
        {
            RawBytes = bytes;
            SendResponseFunc = responseFunc;
        }


        public async Task<SocketStatus> SendResponseAsync(byte[] Bytes)
        {
            if (ResponseSended) throw new Exception("SocketRequest: you can only invoke SendResponseAsync Once");
            ResponseSended = true;
            return await SendResponseFunc(Bytes);
        }

        public async Task<SocketStatus> SendResponseAsync(string response)
        {
            return await SendResponseAsync(response.ConvertBytes());
        }

        public async Task Finish()
        {
            if (!ResponseSended)
            {
                await SendResponseFunc(new byte[0]);
            }
        }
    }
}
