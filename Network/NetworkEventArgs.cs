using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.Network
{
    public delegate void DownloadProgressEventHandler(object sender, DownloadProgressEventArgs e);
    public delegate void BeforeConnectEventHandler(object sender, BeforeNetworkEventArgs e);
    public delegate void AfterConnectEventHandler(object sender, AfterNetworkEventArgs e);

    public enum ConnectType
    {
        GET,
        POST,
        Download,
    }

    public class NetworkEventArgs : EventArgs
    {
        public ConnectType Type { get; private set; }
        public string Url { get; private set; }
        public string Query { get; private set; }
        public IDictionary<string, object> Data { get; private set; }

        public NetworkEventArgs(ConnectType connectType, string url, string query)
        {
            Type = connectType;
            Url = url;
            Query = query;
            Data = new Dictionary<string, object>();
        }
    }

    public class BeforeNetworkEventArgs : NetworkEventArgs
    {
        public bool Cancel { get; set; } = false;

        public BeforeNetworkEventArgs(ConnectType connectType, string url, string query)
            : base(connectType, url, query) { }
    }

    public class AfterNetworkEventArgs : NetworkEventArgs
    {
        public bool IsSuccess { get; private set; }
        public string Result { get; private set; }

        public AfterNetworkEventArgs(ConnectType connectType, string url, string query, bool success, string result)
            : base(connectType, url, query)
        {
            IsSuccess = success;
            Result = result;
        }
    }
    
    public class DownloadProgressEventArgs : EventArgs
    {
        public int ProgressPercentage { get; private set; }
        public long BytesReceived { get; private set; }
        public long TotalBytesToReceive { get; private set; }

        public DownloadProgressEventArgs(long receivedBytes, long totalBytes)
        {
            BytesReceived = receivedBytes;
            TotalBytesToReceive = totalBytes;
            ProgressPercentage = (int)(BytesReceived / TotalBytesToReceive * 100);
        }
    }
}
