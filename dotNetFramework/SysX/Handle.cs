using System;
using System.Diagnostics;
using static XJK.SysX.NativeMethods;

namespace XJK.SysX
{
    public static class Handle
    {
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
