using System.Drawing;
using System.Windows.Forms;

namespace XJK.SysX.Device
{
    public static class Mouse
    {
        public static Point MousePos()
        {
            return Control.MousePosition;
        }
    }
}
