using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using XJK.PInvoke;

namespace XJK.SysX.Device
{
    public static class Monitor
    {
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
