using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using static XJK.SysX.NativeMethods;

namespace XJK.SysX.Device
{
    public static class Monitor
    {
        public static int GetTouchPointCount()
        {
            return GetSystemMetrics(SM_MAXIMUMTOUCHES);
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
