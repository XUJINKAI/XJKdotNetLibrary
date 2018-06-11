using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;

namespace XJK.Network
{
    public static class NetLegacy
    {
        public static bool Enable { get; set; } = true;
        
        public static readonly Encoding Encoding = Encoding.UTF8;
        /// <summary>
        /// e.g. $"{AppName}/{AppVersion} (Windows NT {OS.CurrentVersion}; x64; {OS.BuildBranch})"
        /// </summary>
        public static string UA { get; set; } = "XJK.Network.NetLegacy";

        public static WebClient GetClient()
        {
            var x = new WebClient() { Encoding = Encoding };
            x.Headers.Add("user-agent", UA);
            return x;
        }
        

        public static async Task<bool> DownloadAsync(string url, string path, DownloadProgressChangedEventHandler downloadProgressChangedEventHandler)
        {
            if (!Enable)
            {
                Log.Verbose("Download: Enable = false");
                return false;
            }
            try
            {
                using (var client = GetClient())
                {
                    if (downloadProgressChangedEventHandler != null)
                    {
                        client.DownloadProgressChanged += downloadProgressChangedEventHandler;
                    }
                    await client.DownloadFileTaskAsync(url, path);
                }
                Log.Info($"Downloaded {url} to {path}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Download " + url);
                return false;
            }
        }

        public static async Task<string> GetAsync(string url)
        {
            if (!Enable)
            {
                Log.Verbose("GET: Enable = false");
                return null;
            }
            try
            {
                using (var client = GetClient())
                {
                    string str = await client.DownloadStringTaskAsync(url);
                    Log.Info($"GET {url}" + (OutputDetailResponse ? "\n" + str : ""));
                    return str;
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "GET " + url);
                return null;
            }
        }

        public static async Task<string> PostAsync(string url, string data)
        {
            string result = null;
            Trace.WriteLine($"POST {url} {data}");
            if (!Enable)
            {
                Trace.WriteLine("POST: Enable = false");
            }
            try
            {
                if (data == null) data = "";
                using (var client = GetClient())
                {
                    result = await client.UploadStringTaskAsync(new Uri(url), data);
                }
            }
            catch (Exception ex)
            {
                Trace.Fail($"POST {url} {ex.GetFullMessage()}");
            }
            return result;
        }
    }
}
