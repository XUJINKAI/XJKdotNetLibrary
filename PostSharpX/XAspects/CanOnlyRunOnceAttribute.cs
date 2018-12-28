using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace XJK.XAspects
{
    [Serializable]
    public class CanOnlyRunOnceAttribute : MethodInterceptionAspect
    {
        public bool Invoked { get; private set; } = false;

        public override void OnInvoke(MethodInterceptionArgs args)
        {
            if (!Invoked)
            {
                Invoked = true;
                base.OnInvoke(args);
            }
            else
            {
                throw new InvalidOperationException($"Method {args.Method.Name} already invoked.");
            }
        }

        public override Task OnInvokeAsync(MethodInterceptionArgs args)
        {
            if (!Invoked)
            {
                Invoked = true;
                return base.OnInvokeAsync(args);
            }
            else
            {
                throw new InvalidOperationException($"Method {args.Method.Name} already invoked.");
            }
        }
    }
}
