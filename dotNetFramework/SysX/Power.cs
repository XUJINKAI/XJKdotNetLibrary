using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;

namespace XJK.SysX
{
    public static class Power
    {
        public static void Lock()
        {
            User32.LockWorkStation();
        }

        public static void LogOff()
        {
            User32.ExitWindowsEx(ExitWindows.LogOff, ShutdownReason.MajorOther);
        }

        public static void MonitorOff()
        {
            User32.SendMessage(0xFFFF, 0x112, 0xF170, 2);
        }

        public static void Suspend()
        {
            Powrprof.SetSuspendState(false, false, false);
        }

        public static void Hibernate()
        {
            Powrprof.SetSuspendState(true, false, false);
        }

        public static void Shutdown()
        {
            Cmd.RunAsInvoker("shotdown", "/s /t 0");
        }

        public static void Restart()
        {
            Cmd.RunAsInvoker("shotdown", "/r /t 0");
        }
    }
}
