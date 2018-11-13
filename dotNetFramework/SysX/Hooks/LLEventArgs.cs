using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;

namespace XJK.SysX.Hooks
{
    public class LLEventArgs : EventArgs
    {
        public bool Handled { get; set; } = false;
        public PressType PressType { get; set; } = PressType.None;
        public object HookExSender { get; set; }
    }

    // mouse

    public delegate void MouseChangeEventHandler(object sender, MouseChangeEventArgs e);

    public class MouseChangeEventArgs : LLEventArgs
    {
        public VirtualKeys Key { get; set; } = VirtualKeys.None;
        public int Click { get; set; } = 0;
        public bool MouseMoved { get; set; } = false;
        public POINT Point { get; set; }
        public override string ToString()
        {
            return $"<Mouse {PressType} {Key}, ({Point})>";
        }
    }

    // keyboard

    public delegate void KeyChangeEventHandler(object sender, KeyChangeEventArgs e);

    public class KeyChangeEventArgs : LLEventArgs
    {
        public VirtualKeys VirtualKey { get; set; } = VirtualKeys.None;
        public KeyboardState KeyboardState { get; set; }
        public char? InputChar { get; set; }

        public override string ToString()
        {
            return $"<Keyboard {PressType} {VirtualKey}, ({KeyboardState})>";
        }
    }

}
