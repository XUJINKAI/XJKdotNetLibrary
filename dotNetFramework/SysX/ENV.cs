using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XJK
{
    public static class ENV
    {
        public static string EntryLocation => System.Reflection.Assembly.GetEntryAssembly().Location;
        public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public static IntPtr ModuleHandle { get; }
        public static string ModuleHandleHex { get; }
        
        static ENV()
        {
            try
            {
                ModuleHandle = PInvoke.Kernel32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                ModuleHandleHex = "0x" + ModuleHandle.ToString("x");
            }
            catch
            {
                ModuleHandle = IntPtr.Zero;
                ModuleHandleHex = "0x0";
            }
        }
        
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
