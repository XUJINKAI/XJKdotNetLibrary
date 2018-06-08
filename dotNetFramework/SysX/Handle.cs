using System;
using System.Diagnostics;
using static XJK.SysX.NativeMethods;

namespace XJK.SysX
{
    public static class Handle
    {
        public static readonly IntPtr ModuleHandle;
        public static readonly string ModuleHandleHex;

        static Handle()
        {
            try
            {
                ModuleHandle = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                ModuleHandleHex = "0x" + ModuleHandle.ToString("x");
            }
            catch
            {
                ModuleHandle = IntPtr.Zero;
                ModuleHandleHex = "0x0";
            }
        }

        public static IntPtr GetActiveWindow()
        {
            return GetForegroundWindow();
        }

        public static IntPtr GetByTitle(string Title)
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(Title))
                {
                    hWnd = pList.MainWindowHandle;
                }
            }
            return hWnd;
        }
    }
}
