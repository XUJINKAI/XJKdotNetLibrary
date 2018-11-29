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
        public const string GAURL = "http://www.google-analytics.com/collect";
        public const string GABATCHURL = "http://www.google-analytics.com/batch";
        public const int MaxCacheCount = 20;

        public static event Action<string> DetailListener;

        public readonly string TrackId;
        public readonly string ClientId;
        public Dictionary<string, string> EachHitDict { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> OnceDict { get; set; } = new Dictionary<string, string>();
        private string OnceDictTag = null;

        private readonly Dictionary<string, Dictionary<string, string>> Cache = new Dictionary<string, Dictionary<string, string>>();
        private readonly List<List<Dictionary<string, string>>> BadCache = new List<List<Dictionary<string, string>>>();

        private readonly Timer Timer;
        public double LazySendInterval = 20 * 1000;
        public readonly Status CanAddItem = new Status(true);

        public GoogleAnalyticsApi(string tid, string cid)
        {
            TrackId = tid;
            ClientId = cid;
            Timer = new Timer
            {
                Interval = LazySendInterval,
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
            if (tag.IsNullOrEmpty()) return null;
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
            if (tag.IsNullOrEmpty())
            {
                tag = Helper.RandomString(8);
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
            DetailListener?.Invoke($"Add [{tag}] {Cache.Count}{data.ToFormatTableString()}{C.LF}");
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
            DetailListener?.Invoke($"Remove [{tag}] {Cache.Count}{C.LF}");
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
                DetailListener?.Invoke($"{'='.Dup(60)}{C.LF}TryBadCache remain {BadCacheCountFormat()}");
                Log.Debug($"TryBadCache remain {BadCacheCountFormat()}");
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
            foreach (var x in list.Split(MaxCacheCount))
            {
                var result = await _Send(x);
                if (result)
                {
                    DetailListener?.Invoke($"{'='.Dup(60)}{C.LF}Send {x.Count}, remain bad cache {BadCacheCountFormat()}");
                }
                else
                {
                    BadCache.Add(x);
                    DetailListener?.Invoke($"{'='.Dup(60)}{C.LF}Not send, Add {x.Count}, bad cache {BadCacheCountFormat()}");
                    Log.Debug($"Analytics Send {x.Count}, remain {BadCacheCountFormat()}");
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
                DetailListener?.Invoke($"CollectGoogleAnalyticsAsync{C.LF}{dict.ToFormatTableString()}");
                string postStr = NetHelper.UrlEncode(dict);
                var result = await NetLegacy.PostAsync(GAURL, postStr);
                return result != null;
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

                DetailListener?.Invoke($"BatchCollectGoogleAnalyticsAsync{C.LF}{batchData.Join(o => o.ToFormatTableString(), C.LF)}");
                string batchStr = "";
                foreach (var dict in batchData)
                {
                    batchStr += "\n" + NetHelper.UrlEncode(dict);
                }
                if (batchStr.Length > 1) batchStr = batchStr.Substring(1);
                var result = await NetLegacy.PostAsync(GABATCHURL, batchStr);
                return result != null;
            }
            catch (Exception e)
            {
                Log.Error(e, "BatchCollectGoogleAnalyticsAsync");
                return false;
            }
        }
    }
}
