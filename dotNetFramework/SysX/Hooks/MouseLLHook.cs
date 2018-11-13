using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XJK.Objects;
using XJK.PInvoke;
using WM = XJK.PInvoke.WindowsMessages;

namespace XJK.SysX.Hooks
{
    public class MouseLLHook : DisposeBase
    {
        private WindowsHookEx HookEx;
        public event MouseChangeEventHandler MouseChange;
        public int LastMousePosX { get; private set; }
        public int LastMousePosY { get; private set; }

        public MouseLLHook()
        {
            HookEx = new WindowsHookEx(HookType.WH_MOUSE_LL);
            HookEx.Events += HookEx_Events;
        }

        protected override void OnDispose()
        {
            HookEx.Events -= HookEx_Events;
            HookEx.Dispose();
            HookEx = null;
        }

        private void HookEx_Events(object sender, WindowsHookExEventArgs e)
        {
            if (MouseChange == null) return;
            MouseLLHookStruct @struct = (MouseLLHookStruct)Marshal.PtrToStructure((IntPtr)e.lParam, typeof(MouseLLHookStruct));
            PressType pressType = PressType.None;
            VirtualKeys vk = VirtualKeys.None;
            int click = 0;
            switch (e.wParam)
            {
                // left
                case WM.LBUTTONDOWN:
                    pressType = PressType.KeyDown;
                    vk = VirtualKeys.LeftButton;
                    click = 1;
                    break;
                case WM.LBUTTONUP:
                    pressType = PressType.KeyUp;
                    vk = VirtualKeys.LeftButton;
                    click = 1;
                    break;
                case WM.LBUTTONDBLCLK:
                    pressType = PressType.LeftButtonDoubleClick;
                    break;
                // right
                case WM.RBUTTONDOWN:
                    pressType = PressType.KeyDown;
                    vk = VirtualKeys.RightButton;
                    click = 1;
                    break;
                case WM.RBUTTONUP:
                    pressType = PressType.KeyUp;
                    vk = VirtualKeys.RightButton;
                    click = 1;
                    break;
                case WM.RBUTTONDBLCLK:
                    pressType = PressType.RightButtonDoubleClick;
                    break;
                // middle
                case WM.MBUTTONDOWN:
                    pressType = PressType.KeyDown;
                    vk = VirtualKeys.MiddleButton;
                    click = 1;
                    break;
                case WM.MBUTTONUP:
                    pressType = PressType.KeyUp;
                    vk = VirtualKeys.MiddleButton;
                    click = 1;
                    break;
                case WM.MBUTTONDBLCLK:
                    pressType = PressType.MiddleButtonDoubleClick;
                    break;
                // wheel
                case WM.MOUSEWHEEL:
                    //If the message is WM_MOUSEWHEEL, the high-order word of MouseData member is the wheel delta. 
                    //One wheel click is defined as WHEEL_DELTA, which is 120. 
                    //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                    var mouseDelta = (short)((@struct.MouseData >> 16) & 0xffff);
                    if (mouseDelta > 0)
                    {
                        pressType = PressType.WheelUp;
                    }
                    else
                    {
                        pressType = PressType.WheelDown;
                    }
                    break;
                    //TODO: X BUTTONS (I havent them so was unable to test)
                    //If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
                    //or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
                    //and the low-order word is reserved. This value can be one or more of the following values. 
                    //Otherwise, MouseData is not used. 
            }
            var arg = new MouseChangeEventArgs()
            {
                Handled = false,
                PressType = pressType,
                Key = vk,
                Click = click,
                Point = @struct.Point,
                HookExSender = sender,
            };
            if (LastMousePosX != @struct.Point.X || LastMousePosY != @struct.Point.Y)
            {
                LastMousePosX = @struct.Point.X;
                LastMousePosY = @struct.Point.Y;
                arg.MouseMoved = true;
            }
            foreach (var dele in MouseChange.GetInvocationList().Reverse())
            {
                dele.DynamicInvoke(this, arg);
                if (arg.Handled)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

    }
}
