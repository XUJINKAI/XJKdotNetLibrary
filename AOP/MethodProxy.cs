using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XJK.AOP
{
    public enum InvokeType
    {
        None,
        Proxy,
        ProxyFunc,
        Object,
    }

    public class MethodProxy : DispatchProxy
    {
        public event BeforeInvokeEventHanlder BeforeInvoke;
        public event AfterInvokeEventHanlder AfterInvoke;

        private Type _type;
        private InvokeType _invokeType = InvokeType.None;
        private IInvokerProxy _invokerProxy;
        private Func<IInvokerProxy> GetIInvokerProxyFunc;
        private object _invokeObject;

        private static T Create<T>(InvokeType invokeType, object invoker_object)
        {
            if (invoker_object == null)
            {
                throw new Exception("MethodProxy.Create object == null");
            }
            object proxy = Create<T, MethodProxy>();
            var p = Cast(proxy);
            p._type = typeof(T);
            p._invokeType = invokeType;
            switch (invokeType)
            {
                case InvokeType.Proxy:
                    p._invokerProxy = (IInvokerProxy)invoker_object;
                    break;
                case InvokeType.ProxyFunc:
                    p.GetIInvokerProxyFunc = (Func<IInvokerProxy>)invoker_object;
                    break;
                case InvokeType.Object:
                    p._invokeObject = invoker_object;
                    break;
                default:
                    throw new Exception("InvokeProxy.Create unknown type");
            }
            return (T)proxy;
        }

        public static T CreateProxy<T>(IInvokerProxy invoker)
        {
            return Create<T>(InvokeType.Proxy, invoker);
        }

        public static T CreateProxy<T>(Func<IInvokerProxy> GetIInvokerProxy)
        {
            return Create<T>(InvokeType.ProxyFunc, GetIInvokerProxy);
        }

        public static T Intercept<T>(object obj)
        {
            return Create<T>(InvokeType.Object, obj);
        }

        public static T Intercept<T>(object obj, BeforeInvokeEventHanlder beforeInvoke, AfterInvokeEventHanlder afterInvoke)
        {
            var proxy = Intercept<T>(obj);
            var p = Cast(proxy);
            p.BeforeInvoke += beforeInvoke;
            p.AfterInvoke += afterInvoke;
            return proxy;
        }

        public static MethodProxy Cast(object obj)
        {
            return (MethodProxy)obj;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Debug.WriteLine("[MethodProxy.Invoke] " + (new MethodCallInfo() { Name = targetMethod.Name, Args = args.ToList() }).ToString());

            bool IsIInvokeProxy = _invokeType == InvokeType.Proxy || _invokeType == InvokeType.ProxyFunc;
            IInvokerProxy invoker;
            switch (_invokeType)
            {
                case InvokeType.Proxy:
                    invoker = _invokerProxy;
                    break;
                case InvokeType.ProxyFunc:
                    invoker = GetIInvokerProxyFunc();
                    break;
                default:
                    invoker = null;
                    break;
            }

            if (IsIInvokeProxy || BeforeInvoke != null)
            {
                BeforeInvokeEventArgs beforeArgs = new BeforeInvokeEventArgs()
                {
                    MethodInfo = targetMethod,
                    Args = args,
                };
                BeforeInvoke?.Invoke(this, beforeArgs);
                invoker?.BeforeInvoke(this, beforeArgs);
                if (beforeArgs.Handled)
                {
                    return beforeArgs.FakeResult;
                }
            }

            object result;
            if (IsIInvokeProxy)
            {
                result = invoker.Invoke(targetMethod, args);
            }
            else if(_invokeType == InvokeType.Object)
            {
                result = targetMethod.Invoke(_invokeObject, args);
            }
            else
            {
                throw new Exception("InvokeProxy: unknown InvokeType");
            }

            if (result is Task task)
            {
                if (targetMethod.ReturnType.IsGenericType)
                {
                    Type type = targetMethod.ReturnType.GetGenericArguments()[0];
                    result = TypeHelper.ConvertTaskObject(task, type);
                }
                else
                {
                    Task.Run(async () => await task);
                    result = Task.CompletedTask;
                }
            }
            
            result.AssertType(targetMethod.ReturnType);
            
            if (IsIInvokeProxy || AfterInvoke != null)
            {
                AfterInvokeEventArgs afterArgs = new AfterInvokeEventArgs()
                {
                    MethodInfo = targetMethod,
                    Args = args,
                    Result = result,
                };
                AfterInvoke?.Invoke(this, afterArgs);
                invoker?.AfterInvoke(this, afterArgs);
            }
            return result;
        }

    }
}
