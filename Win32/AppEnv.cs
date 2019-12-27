using System;
using System.Diagnostics;
using System.Security.Principal;
using XJK.Win32.PInvoke;

namespace XJK.Win32
{
    public static partial class AppEnv
    {
        public static IntPtr GetModuleHandle()
        {
            return Kernel32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
        }

        public static string GetModuleHandleHexString()
        {
            return "0x" + GetModuleHandle().ToString("x");
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
