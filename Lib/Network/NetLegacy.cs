﻿using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XJK.Logger;

namespace XJK.Network
{
    public static class NetLegacy
    {
        /// <summary>
        /// e.g. $"{AppName}/{AppVersion} (Windows NT {OS.CurrentVersion}; x64; {OS.BuildBranch})"
        /// </summary>
        public static string UA { get; set; } = "XJK.Network.NetLegacy";
        public static readonly Encoding Encoding = Encoding.UTF8;

        public static event BeforeConnectEventHandler BeforeConnect;
        public static event AfterConnectEventHandler AfterConnect;
        public static event Action<bool> EnableChanged;

        public static bool Enable
        {
            get => s_enable; set
            {
                s_enable = value;
                EnableChanged?.Invoke(value);
            }
        }
        private static bool s_enable = true;

        public static WebClient GetClient()
        {
            var x = new WebClient() { Encoding = Encoding };
            x.Headers.Add("user-agent", UA);
            return x;
        }


        public static async Task<bool> DownloadAsync(string url, string path, DownloadProgressEventHandler downloadProgressEventHandler)
        {
            if (!Enable)
            {
                Trace.WriteLine("Download: Enable = false");
                return false;
            }

            using (var client = GetClient())
            {
                var beforeNetworkEventArgs = new BeforeNetworkEventArgs(ConnectType.Download, url, "");
                beforeNetworkEventArgs.Data.Add("path", path);
                BeforeConnect?.Invoke(client, beforeNetworkEventArgs);
                if (beforeNetworkEventArgs.Cancel) return false;

                bool success = true;
                Exception exp = null;
                try
                {
                    if (downloadProgressEventHandler != null)
                    {
                        client.DownloadProgressChanged += (sender, e) =>
                        {
                            downloadProgressEventHandler(sender, new DownloadProgressEventArgs(e.BytesReceived, e.TotalBytesToReceive));
                        };
                    }
                    await client.DownloadFileTaskAsync(url, path);
                    return true;
                }
                catch (Exception ex)
                {
                    success = false;
                    exp = ex;
                    Log.Error(ex, true);
                    return false;
                }
                finally
                {
                    AfterNetworkEventArgs afterNetworkEventArgs = new AfterNetworkEventArgs(ConnectType.Download, url, "", success, "");
                    afterNetworkEventArgs.Data.Add("path", path);
                    afterNetworkEventArgs.Data.Add("exception", exp);
                    AfterConnect?.Invoke(client, afterNetworkEventArgs);
                }
            }
        }

        public static async Task<string> GetAsync(string url, bool includeHeaders = false)
        {
            if (!Enable)
            {
                Log.Verbose("GET: Enable = false");
                return null;
            }

            using (var client = GetClient())
            {
                var beforeNetworkEventArgs = new BeforeNetworkEventArgs(ConnectType.GET, url, "");
                BeforeConnect?.Invoke(client, beforeNetworkEventArgs);
                if (beforeNetworkEventArgs.Cancel) return null;

                bool success = true;
                Exception exp = null;
                string result = "";
                try
                {
                    result = await client.DownloadStringTaskAsync(url);
                    if (includeHeaders)
                    {
                        var header = new StringBuilder();
                        for (int i = 0; i < client.ResponseHeaders.Count; i++)
                        {
                            header.Append($"{client.ResponseHeaders.GetKey(i)}: {client.ResponseHeaders.GetValues(i).JoinToString(Environment.NewLine)}{Environment.NewLine}");
                        }
                        result = header.ToString() + result;
                    }
                    return result ?? "";
                }
                catch (Exception ex)
                {
                    success = false;
                    exp = ex;
                    Log.Error(ex, true);
                    return null;
                }
                finally
                {
                    AfterNetworkEventArgs afterNetworkEventArgs = new AfterNetworkEventArgs(ConnectType.GET, url, "", success, result);
                    afterNetworkEventArgs.Data.Add("exception", exp);
                    AfterConnect?.Invoke(client, afterNetworkEventArgs);
                }
            }
        }

        public static async Task<string> PostAsync(string url, string data = "")
        {
            if (!Enable)
            {
                Log.Verbose("POST: Enable = false");
                return null;
            }

            using (var client = GetClient())
            {
                var beforeNetworkEventArgs = new BeforeNetworkEventArgs(ConnectType.POST, url, data);
                BeforeConnect?.Invoke(client, beforeNetworkEventArgs);
                if (beforeNetworkEventArgs.Cancel) return null;

                bool success = true;
                Exception exp = null;
                string result = "";
                try
                {
                    result = await client.UploadStringTaskAsync(new Uri(url), data);
                    return result ?? "";
                }
                catch (Exception ex)
                {
                    success = false;
                    exp = ex;
                    Log.Error(ex, true);
                    return null;
                }
                finally
                {
                    AfterNetworkEventArgs afterNetworkEventArgs = new AfterNetworkEventArgs(ConnectType.POST, url, "", success, result);
                    afterNetworkEventArgs.Data.Add("exception", exp);
                    AfterConnect?.Invoke(client, afterNetworkEventArgs);
                }
            }
        }
    }
}
