using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XJK.SysX.Hooks
{
    public enum PressType
    {
        /// <summary>
        /// Mouse Move, unknown...
        /// </summary>
        None = 0,

        // VirtualKeys

        KeyDown = 0x0100,
        KeyUp = 0x0101,

        // Custom

        WheelDown = 0x1000,
        WheelUp = 0x1001,
        WheelLeft = 0x1002,
        WheelRight = 0x1003,
        LeftButtonDoubleClick = 0x2000,
        RightButtonDoubleClick = 0x2001,
        MiddleButtonDoubleClick = 0x2002,
    }

    public static class PressTypeExtension
    {
        /// <summary>
        /// KeyDown or KeyUp
        /// </summary>
        /// <param name="pressType"></param>
        /// <returns></returns>
        public static bool IsKeyEvent(this PressType pressType)
        {
            return pressType == PressType.KeyDown || pressType == PressType.KeyUp;
        }

        public static bool IsWheelEvent(this PressType pressType)
        {
            return pressType == PressType.WheelDown || pressType == PressType.WheelUp
                || pressType == PressType.WheelLeft || pressType == PressType.WheelRight; ;
        }

        public static bool IsDoubleClickEvent(this PressType pressType)
        {
            return pressType == PressType.LeftButtonDoubleClick || pressType == PressType.RightButtonDoubleClick || pressType == PressType.MiddleButtonDoubleClick;
        }
    }
}
