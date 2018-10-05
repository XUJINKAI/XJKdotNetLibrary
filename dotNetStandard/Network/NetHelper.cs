using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace XJK.Network
{
    public static class NetHelper
    {
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
