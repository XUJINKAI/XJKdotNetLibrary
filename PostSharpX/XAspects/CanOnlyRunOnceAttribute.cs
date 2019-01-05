using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace XJK.XAspects
{
    /// <summary>
    /// throw InvalidOperationException if method run twice
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method)]
    public class CanOnlyRunOnceAttribute : MethodInterceptionAspect, IInstanceScopedAspect
    {
        public bool Invoked { get; private set; } = false;
        /// <summary>
        /// Flase (Default): method can run once per instance. True: method can globally run once despite of instance.
        /// </summary>
        public bool StaticScoped { get; set; } = false;

        public override void OnInvoke(MethodInterceptionArgs args)
        {
            if (!Invoked)
            {
                Invoked = true;
                base.OnInvoke(args);
            }
            else
            {
                throw new InvalidOperationException($"Method '{args.Method.Name}' already invoked.");
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
                throw new InvalidOperationException($"Method '{args.Method.Name}' already invoked.");
            }
        }

        public object CreateInstance(AdviceArgs adviceArgs)
        {
            if (StaticScoped) return this;
            else return this.MemberwiseClone();
        }

        public void RuntimeInitializeInstance()
        {

        }
    }
}
