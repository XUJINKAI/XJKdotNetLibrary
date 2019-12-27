using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using XJK.Win32.PInvoke;
using XJK.Win32.WinMsg;

namespace XJK.Win32.Hooks
{
    public class SystemHotkeyHook : DisposeBase
    {
        public WindowEx WindowEx { get; private set; }
        private readonly Dictionary<int, Action> IdActionDict = new Dictionary<int, Action>();

        public SystemHotkeyHook(WindowEx windowEx)
        {
            WindowEx = windowEx;
            WindowEx.MsgHotkey += Window_MsgHotkey;
        }

        protected override void OnDispose()
        {
            WindowEx.MsgHotkey -= Window_MsgHotkey;
            WindowEx = null;
        }

        private void Window_MsgHotkey(object sender, WndMsgEventArgs e)
        {
            IdActionDict[e.wParam.ToInt32()]();
        }

        public int GetHotkeyId(Modifiers modifiers, VirtualKeys vk) => ((int)modifiers * 256 + (int)vk).GetHashCode();

        public int RegisterHotkey(Modifiers modifiers, VirtualKeys vk, Action func)
        {
            int id = GetHotkeyId(modifiers, vk);
            bool success = User32.RegisterHotKey(WindowEx.Handle, id, modifiers, (uint)vk);
            if (!success)
            {
                int err = Marshal.GetLastWin32Error();
                throw new Win32Exception(err);
            }
            IdActionDict[id] = func;
            return id;
        }

        public bool UnregisterHotkey(int id)
        {
            bool success = User32.UnregisterHotKey(WindowEx.Handle, id);
            if (!success)
            {
                int err = Marshal.GetLastWin32Error();
                throw new Win32Exception(err);
            }
            return success;
        }

        public bool UnregisterHotkey(Modifiers modifiers, VirtualKeys vk)
        {
            return UnregisterHotkey(GetHotkeyId(modifiers, vk));
        }

    }
}
