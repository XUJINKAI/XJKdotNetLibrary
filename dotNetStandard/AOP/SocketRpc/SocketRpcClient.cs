using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XJK.Network.Socket;
using XJK.Serializers;

namespace XJK.AOP.SocketRpc
{
    public abstract class SocketRpcClient: SocketRpcBase
    {
        private SocketPipeClient SocketPipeClient;

        public void Connect(string host, int port)
        {
            SocketPipeClient = new SocketPipeClient(host, port);
            SocketPipeClient.Connect();
            SocketPipeClient.RequestReceived += SocketPipeClient_RequestReceived;
        }

        private async Task SocketPipeClient_RequestReceived(object sender, SocketRequestReceivedEventArgs args)
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

        public void Disconnect()
        {
            SocketPipeClient.Close();
        }

        protected override async Task<byte[]> SendBytesAsync(byte[] Bytes)
        {
            var result = await SocketPipeClient.SendBytes(Bytes);
            return result;
        }
    }
}
