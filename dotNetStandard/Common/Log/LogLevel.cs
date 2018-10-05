using System;
using System.Collections.Generic;
using System.Text;

namespace XJK
{
    [Flags]
    public enum LogLevel : int
    {
        // Trace: Fail
        Fatal = 1,
        // Trace: TraceError
        Error = 2,
        // Trace: TraceWarning
        Warn = 4,
        // Trace: TraceInformation
        Info = 8,
        // Debug, Trace: Write, WriteLine
        Verbose = 16,
    }
}
