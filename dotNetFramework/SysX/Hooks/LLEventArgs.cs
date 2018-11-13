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
        public VirtualKeys Key { get; set; } = VirtualKeys.None;
    }

    // mouse

    public delegate void MouseChangeEventHandler(object sender, MouseChangeEventArgs e);

    public class MouseChangeEventArgs : LLEventArgs
    {
        public int MouseClick { get; set; } = 0;
        public bool MouseMoved { get; set; } = false;
        public POINT MousePosition { get; set; }

        public override string ToString()
        {
            return $"<Mouse {PressType} {Key}, ({MousePosition})>";
        }
    }

    // keyboard

    public delegate void KeyChangeEventHandler(object sender, KeyChangeEventArgs e);

    public class KeyChangeEventArgs : LLEventArgs
    {
        public KeyboardState KeyboardState { get; set; }
        public char? InputChar { get; set; }

        public override string ToString()
        {
            return $"<Keyboard {PressType} {Key}, ({KeyboardState})>";
        }
    }

}
