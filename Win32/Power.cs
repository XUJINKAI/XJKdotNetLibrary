﻿using XJK.Win32.CommandHelper;
using XJK.Win32.PInvoke;

namespace XJK.Win32
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
            CmdRunner.RunAsInvoker("shotdown", "/s /t 0");
        }

        public static void Restart()
        {
            CmdRunner.RunAsInvoker("shotdown", "/r /t 0");
        }

        /// <summary>
        /// prevent monitor off and system sleep
        /// </summary>
        public static void KeepSystemMonitorOn(bool keep)
        {
            if (keep)
            {
                Kernel32.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED
                    | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
            }
            else
            {
                Kernel32.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            }
        }
    }
}
