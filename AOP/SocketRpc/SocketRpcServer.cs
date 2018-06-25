using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Windows.Networking.Sockets;
using XJK.Network.Socket;

namespace XJK.AOP.SocketRpc
{
    public abstract class SocketRpcServer : SocketRpcBase
    {
        private SocketPipeServer SocketPipeServer;
        
        public void Serve(string host, int port)
        {
            SocketPipeServer = new SocketPipeServer(host, port);
        }
    }
}
