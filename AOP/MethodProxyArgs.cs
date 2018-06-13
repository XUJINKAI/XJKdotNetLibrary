using System;
using System.Diagnostics;
using System.Reflection;

namespace XJK.AOP
{
    public class InvokeEventArgsBase : EventArgs
    {
        public MethodInfo MethodInfo { get; set; }
        public object[] Args { get; set; }
    }

    public class BeforeInvokeEventArgs : InvokeEventArgsBase
    {
        public bool Handled { get; private set; } = false;
        public object FakeResult { get; private set; }

        public void Handle(object fakeResult)
        {
            Handled = true;
            FakeResult = fakeResult;
            if (fakeResult?.GetType() != MethodInfo.ReturnType)
            {
                throw new Exception($"[BeforeInvokeEventArgs] Type of FakeResult not Match: except [{MethodInfo.ReturnType}], result [{fakeResult?.GetType()}]");
            }
        }
    }

    public class AfterInvokeEventArgs : InvokeEventArgsBase
    {
        public object Result { get; set; }
    }

    public delegate void BeforeInvokeEventHanlder(object sender, BeforeInvokeEventArgs args);
    public delegate void AfterInvokeEventHanlder(object sender, AfterInvokeEventArgs args);
}
