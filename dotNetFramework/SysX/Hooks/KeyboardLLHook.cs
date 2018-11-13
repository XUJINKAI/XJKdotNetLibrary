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
    public class KeyboardLLHook : DisposeBase
    {
        private WindowsHookEx HookEx;
        public event KeyChangeEventHandler KeyChange;

        public KeyboardLLHook()
        {
            HookEx = new WindowsHookEx(HookType.WH_KEYBOARD_LL);
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
            if (KeyChange == null) return;
            KeyboardLLHookStruct @struct = (KeyboardLLHookStruct)Marshal.PtrToStructure((IntPtr)e.lParam, typeof(KeyboardLLHookStruct));
            PressType pressType = PressType.None;
            VirtualKeys vk = (VirtualKeys)@struct.VirtualKeyCode;
            KeyboardState state = KeyboardState.FromCurrentState();
            char? inputChar = null;
            if (e.wParam == WM.KEYDOWN || e.wParam == WM.SYSKEYDOWN)
            {
                pressType = PressType.KeyDown;
            }
            else if (e.wParam == WM.KEYUP || e.wParam == WM.SYSKEYUP)
            {
                pressType = PressType.KeyUp;
            }
            if (e.wParam == WM.KEYDOWN)
            {
                // Get Press Char
                char? PressKey = null;
                byte[] inBuffer = new byte[2];
                if (User32.ToAscii(@struct.VirtualKeyCode,
                          @struct.ScanCode,
                          state.Bytes,
                          inBuffer,
                          @struct.Flags) == 1)
                {
                    char ch = (char)inBuffer[0];
                    if (!char.IsControl(ch))
                    {
                        PressKey = ch;
                        if ((state.CapsLockToggled ^ state.ShiftPressed) && char.IsLetter(ch))
                        {
                            PressKey = char.ToUpper(ch);
                        }
                        inputChar = PressKey;
                    }
                }
            }
            var args = new KeyChangeEventArgs()
            {
                Handled = false,
                PressType = pressType,
                Key = vk,
                KeyboardState = state,
                InputChar = inputChar,
            };
            foreach (var action in KeyChange.GetInvocationList().Reverse())
            {
                action.DynamicInvoke(this, args);
                if (args.Handled)
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}
