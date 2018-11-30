using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PostSharp.Aspects;

namespace XJK.XDebugger
{
    [Conditional("DEBUG")]
    public class ExceptionBreakAttribute : OnExceptionAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            Debugger.Break();
            base.OnException(args);
        }
    }
}
