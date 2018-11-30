using PostSharp.Aspects;
using System;
using System.Diagnostics;
using System.Reflection;

namespace XJK.XDebugger
{
    /*
     * https://stackoverflow.com/a/2414053
     */
    [Serializable]
    public sealed class TraceArgumentsAttribute : OnMethodBoundaryAspect
    {
        private string MethodName(MethodBase methodBase)
        {
            return $"{methodBase.Name} ({methodBase.DeclaringType.Name})";
        }

        private string Format(object s1, object s2, object s3)
        {
            return $"{s1,-20} {s2} {s3}";
        }

        private void WriteLine(string line)
        {
            Trace.WriteLine(line);
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            WriteLine($"Entering {MethodName(args.Method)})");
            for (int x = 0; x < args.Arguments.Count; x++)
            {
                WriteLine(Format(args.Method.GetParameters()[x].Name, "=", args.Arguments.GetArgument(x)));
            }
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            WriteLine(Format("Return Value", ":", args.ReturnValue));
            WriteLine($"Leaving {MethodName(args.Method)}");
        }
    }
}
