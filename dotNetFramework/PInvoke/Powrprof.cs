using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace XJK.PInvoke
{
    public static class Powrprof
    {
        [DllImport("Powrprof.dll", SetLastError = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
