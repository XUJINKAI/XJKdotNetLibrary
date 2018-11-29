using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using XJK.Network.Socket;
using XJK.Serializers;

namespace XJK.AOP.SocketRpc
{
    public abstract class SocketRpcServer : SocketRpcBase
    {
        private SocketPipeServer SocketPipeServer;
        
        public void Serve(string host, int port)
        {
            SocketPipeServer = new SocketPipeServer(host, port);
            SocketPipeServer.StartListen();
            SocketPipeServer.RequestReceived += SocketPipeServer_RequestReceived;
        }

        private async Task SocketPipeServer_RequestReceived(object sender, SocketRequestReceivedEventArgs args)
        {
            MethodCallInfo methodCallInfo = (MethodCallInfo)BytesSerialization.BytesArrayToObject(args.RawBytes);
            var invoke_result = await methodCallInfo.InvokeAsync(GetExcuteObject());
            var response = new MethodCallInfo()
            {
                Name = methodCallInfo.Name,
                Result = invoke_result,
            };
            var resBytes = BytesSerialization.ObjectToBytesArray(response);
            var status = await args.SendResponseAsync(resBytes);
        }

        public void Stop()
        {
            SocketPipeServer.StopListen();
        }
        
        protected override async Task<byte[]> SendBytesAsync(byte[] Bytes)
        {
            var list = await SocketPipeServer.BroadcastBytes(Bytes);
            return list[0];
        }
    }
}
