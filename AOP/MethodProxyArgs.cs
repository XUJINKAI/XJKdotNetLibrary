using System;
using System.Reflection;

namespace XJKdotNetLibrary.MethodWrapper
{
    public class InvokeEventArgsBase : EventArgs
    {
        public MethodInfo MethodInfo { get; set; }
        public object[] Args { get; set; }
    }

    public class BeforeInvokeEventArgs : InvokeEventArgsBase
    {
        public bool Handle { get; set; } = false;
        public object FakeResult { get; set; }
    }

    public class AfterInvokeEventArgs : InvokeEventArgsBase
    {
        public object Result { get; set; }
    }

    public delegate void BeforeInvokeEventHanlder(object sender, BeforeInvokeEventArgs args);
    public delegate void AfterInvokeEventHanlder(object sender, AfterInvokeEventArgs args);
}
