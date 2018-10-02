using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace XJK.SysX
{
    public static class ENV
    {
        public readonly static string Directory = AppDomain.CurrentDomain.BaseDirectory;
        public readonly static string ExePath = System.Reflection.Assembly.GetEntryAssembly().Location;

        public static readonly IntPtr ModuleHandle;
        public static readonly string ModuleHandleHex;

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        
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
    }
}
