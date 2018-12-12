using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using XJK.PInvoke;

namespace XJK.SysX.Hooks
{
    /// <summary>
    /// work with SetWindowsHookEx
    /// </summary>
    public class WindowsHookEx : DisposeBase
    {
        public event WindowsHookExEvent Events;
        public int HookId { get; private set; }
        public HookType HookType { get; private set; }

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private HookProc _hookDele;
        
        public WindowsHookEx(HookType Type, WindowsHookExOnType hookOn = WindowsHookExOnType.Auto)
        {
            HookType = Type;
            _hookDele = ProcMsg;
            bool OnThread = true;
            if (hookOn == WindowsHookExOnType.Auto)
            {
                if (new HookType[] { HookType.WH_MOUSE_LL, HookType.WH_KEYBOARD_LL }.Contains(Type))
                {
                    OnThread = false;
                }
            }
            else if (hookOn == WindowsHookExOnType.Module)
            {
                OnThread = false;
            }
            if (OnThread)
            {
                uint threadId = (uint)Kernel32.GetCurrentThreadId();
                HookId = User32.SetWindowsHookEx(HookType, _hookDele, IntPtr.Zero, threadId);
            }
            else
            {
                var hmod = Kernel32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName); //Marshal.GetHINSTANCE(GetType().Module);
                HookId = User32.SetWindowsHookEx(HookType, _hookDele, hmod, 0);
            }

            if (HookId == 0)
            {
                int err = Marshal.GetLastWin32Error();
                if (err != 0)
                {
                    Debugger.Break();
                    throw new Win32Exception(err);
                }
            }
            Application.Current.Exit += Current_Exit;
        }

        private int ProcMsg(int nCode, int wParam, IntPtr lParam)
        {
            if (Events != null)
            {
                WindowsHookExEventArgs e = new WindowsHookExEventArgs(HookType, nCode, wParam, lParam);
                foreach(var eve in Events.GetInvocationList().Reverse())
                {
                    eve.DynamicInvoke(this, e);
                    if (e.Handled) return -1;
                }
            }
            return User32.CallNextHookEx(HookId, nCode, wParam, lParam);
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            OnDispose();
        }

        protected override sealed void OnDispose()
        {
            User32.UnhookWindowsHookEx(HookId);
        }
    }
}