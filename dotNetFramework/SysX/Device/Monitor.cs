using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using XJK.PInvoke;

namespace XJK.SysX.Device
{
    public static class Monitor
    {
        public static void MonitorOn()
        {
            User32.SendMessage(HWND.BROADCAST, WindowsMessages.SYSCOMMAND, SysCommands.SC_MONITORPOWER, MonitorPower.MONITOR_ON);
        }

        public static void MonitorOff()
        {
            User32.SendMessage(HWND.BROADCAST, WindowsMessages.SYSCOMMAND, SysCommands.SC_MONITORPOWER, MonitorPower.MONITOR_OFF);
        }

        public static void MonitorStandby()
        {
            User32.SendMessage(HWND.BROADCAST, WindowsMessages.SYSCOMMAND, SysCommands.SC_MONITORPOWER, MonitorPower.MONITOR_STANDBY);
        }

        public static int GetTouchPointCount()
        {
            return User32.GetSystemMetrics(SystemMetric.SM_MAXIMUMTOUCHES);
        }

        public static List<Rectangle> AllMonitorsRectangle()
        {
            var list = new List<Rectangle>();
            foreach (var mon in Screen.AllScreens)
            {
                list.Add(mon.Bounds);
            }
            return list;
        }
    }
}
