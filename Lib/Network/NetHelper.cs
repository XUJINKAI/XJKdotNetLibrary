using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace XJK.Network
{
    public static class NetHelper
    {
        public static string UrlEncode(string s)
        {
            return System.Web.HttpUtility.UrlEncode(s).Replace("+", "%20");
        }

        public static string UrlEncode(Dictionary<string, string> dict)
        {
            string str = dict.Aggregate("", (sum, next) => $"{sum}&{UrlEncode(next.Key)}={UrlEncode(next.Value)}");
            if (str.Length > 1) str = str.Substring(1);
            return str;
        }

        public static List<int> GetUsedPortList()
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            IEnumerable<int> PortList = ipEndPoints.Select(end => end.Port);
            return PortList.ToList();
        }

        public static bool PortIsUsed(int port)
        {
            return GetUsedPortList().Contains(port);
        }

        public static int GetAvailablePort(int start = 1000, int end = 65535)
        {
            var list = GetUsedPortList();
            for (int i = start; i < end; i++)
            {
                if (list.Contains(i))
                {
                    continue;
                }
                else
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
