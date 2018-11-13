using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XJK.SysX.Hooks
{
    public enum PressType
    {
        None = 0,
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        WheelDown = 0x1000,
        WheelUp = 0x1001,
        WheelLeft = 0x1002,
        WheelRight = 0x1003,
        LeftButtonDoubleClick = 0x2000,
        RightButtonDoubleClick = 0x2001,
        MiddleButtonDoubleClick = 0x2002,
    }
}
