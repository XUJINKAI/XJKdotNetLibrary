using System.Runtime.InteropServices;

namespace XJK.Win32.PInvoke
{
    public static class Powrprof
    {
        [DllImport("Powrprof.dll", SetLastError = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
