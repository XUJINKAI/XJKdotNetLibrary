using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace XJK.Network
{
    public class GoogleAnalyticsApi
    {
        public readonly Status CanAddItem = new Status(true);
        public Dictionary<string, string> EachHitDict { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> OnceDict { get; set; } = new Dictionary<string, string>();


        private static bool Debug_Mock => G.GaMock;
        private static bool Debug_OutputDetailFile => G.ReleaseCheck || G.DEBUG;
        public static readonly string LogFile = Path.Combine(G.AppDataFolder, "GA.log");
        private static void DebugLog(string s)
        {
            if (Debug_OutputDetailFile)
            {
                try
                {
                    File.AppendAllText(LogFile, DateTime.Now.ToString("[HH:mm:ss] ") + s + "\r\n---\r\n");
                }
                catch { }
            }
        }

        public int Count => Cache.Count;
        public readonly string TrackId;
        public readonly string ClientId;

        public const int MaxCacheCount = 20;
        public const string GAURL = "http://www.google-analytics.com/collect";
        public const string GABATCHURL = "http://www.google-analytics.com/batch";

        private readonly Dictionary<string, Dictionary<string, string>> Cache = new Dictionary<string, Dictionary<string, string>>();
        private readonly List<List<Dictionary<string, string>>> BadCache = new List<List<Dictionary<string, string>>>();
        
        private string OnceDictTag = null;
        private readonly Timer Timer;

        public AnalyticsApi(string tid, string cid)
        {
            if (Debug_OutputDetailFile && File.Exists(LogFile)) try { File.WriteAllText(LogFile, ""); } catch { }
            TrackId = tid;
            ClientId = cid;
            Timer = new Timer
            {
                Interval = G.AnalyticsLazySendInterval,
                AutoReset = false,
            };
            Timer.Elapsed += Timer_Elapsed;
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await SendAsync();
        }
        
        public Dictionary<string, string> Get(string tag)
        {
            if (String.IsNullOrEmpty(tag)) return null;
            if (Cache.ContainsKey(tag)) return Cache[tag];
            return null;
        }

        public string Add(Dictionary<string, string> data, string tag = "", bool CountDown = true)
        {
            if (!CanAddItem) return tag;
            if (CountDown)
            {
                Timer.Start();
            }
            if (String.IsNullOrEmpty(tag))
            {
                tag = FS.RandomString(8);
            }
            else
            {
                Remove(tag);
            }
            data["v"] = "1";
            data["tid"] = TrackId;
            data["cid"] = ClientId;
            foreach (var item in EachHitDict)
            {
                data[item.Key] = item.Value;
            }
            if (OnceDictTag == null)
            {
                foreach (var item in OnceDict)
                {
                    data[item.Key] = item.Value;
                }
                OnceDictTag = tag;
            }
            Cache.Add(tag, data);
            if (Debug_OutputDetailFile)
            {
                DebugLog($"Add [{tag}] {Cache.Count}\n" + FS.DictToString(data, true));
            }
            //if (Cache.Count >= MaxCacheCount) Send();
            return tag;
        }

        private bool Remove(string tag)
        {
            Timer.Start();
            if (OnceDictTag != null && Cache.ContainsKey(OnceDictTag))
            {
                OnceDictTag = null;
            }
            var result = Cache.Remove(tag);
            if (Debug_OutputDetailFile)
            {
                DebugLog($"Remove [{tag}] {Cache.Count}");
            }
            Trace.se
            return result;
        }

        public async void TryBadCache()
        {
            if (BadCache.Count > 0)
            {
                bool flag = true;
                for (int i = BadCache.Count - 1; flag && i >= 0; i--)
                {
                    var x = BadCache[i];
                    flag = await _Send(x);
                    if (flag)
                    {
                        BadCache.RemoveAt(i);
                    }
                }
                if (Debug_OutputDetailFile)
                {
                    DebugLog("=========================================================");
                    DebugLog($"TryBadCache remain {BadCacheCountFormat()}");
                    Notify.PopNewToast($"TryBadCache remain {BadCacheCountFormat()}");
                }
            }
        }

        private string BadCacheCountFormat()
        {
            string s = BadCache.Count.ToString() + "(";
            foreach(var x in BadCache)
            {
                s += x.Count.ToString() + ",";
            }
            s += ")";
            return s;
        }

        public async void Send()
        {
            await SendAsync();
        }

        public async Task SendAsync()
        {
            Timer.Stop();
            var list = Cache.Values.ToList();
            Cache.Clear();
            foreach (var x in FS.SplitList(list, MaxCacheCount))
            {
                var result = await _Send(x);
                if (result)
                {
                    if (Debug_OutputDetailFile)
                    {
                        DebugLog("=========================================================");
                        DebugLog($"Send {x.Count}, remain bad cache {BadCacheCountFormat()}");
                        Notify.PopNewToast($"Analytics Send {x.Count}, remain {BadCacheCountFormat()}");
                    }
                }
                else
                {
                    BadCache.Add(x);
                    if (Debug_OutputDetailFile)
                    {
                        DebugLog("=========================================================");
                        DebugLog($"Not send, Add {x.Count}, bad cache {BadCacheCountFormat()}");
                        Notify.PopNewToast($"Analytics Send {x.Count}, remain {BadCacheCountFormat()}");
                    }
                }
            }
        }

        private static async Task<bool> _Send(IEnumerable<Dictionary<string, string>> data)
        {
            if (data.Count() == 0) { return false; }
            if (data.Count() == 1) { return await CollectGoogleAnalyticsAsync(data.First()); }
            return await BatchCollectGoogleAnalyticsAsync(data);
        }

        private static async Task<bool> CollectGoogleAnalyticsAsync(Dictionary<string, string> dict)
        {
            try
            {
                //if (OutputDetailFormat)
                //{
                //    Log.Info($"CollectGoogleAnalyticsAsync" + (Debug_Mock ? " Mock" : "") + "\n" + Helper.DictToString(dict, true));
                //}
                if (G.DEBUG && Debug_Mock)
                {
                    return true;
                }
                string postStr = FS.DictToString(dict);
                return await NetLegacy.PostAsync(GAURL, postStr, false);
            }
            catch (Exception e)
            {
                Log.Error(e, "CollectGoogleAnalyticsAsync");
                return false;
            }
        }

        private static async Task<bool> BatchCollectGoogleAnalyticsAsync(IEnumerable<Dictionary<string, string>> batchData)
        {
            try
            {
                //if (OutputDetailFormat)
                //{
                //    Log.Info($"BatchCollectGoogleAnalyticsAsync" + (Debug_Mock ? " Mock" : "")
                //        + batchData.Aggregate("", (sum, next) => $"{sum}\n---\n{Helper.DictToString(next, true)}"));
                //}
                if (G.DEBUG && Debug_Mock)
                {
                    return true;
                }
                string batchStr = "";
                foreach (var dict in batchData)
                {
                    batchStr += "\n" + FS.DictToString(dict);
                }
                if (batchStr.Length > 1) batchStr = batchStr.Substring(1);
                return await NetLegacy.PostAsync(GABATCHURL, batchStr, false);
            }
            catch (Exception e)
            {
                Log.Error(e, "BatchCollectGoogleAnalyticsAsync");
                return false;
            }
        }
    }
}
