using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XJK.MethodWrapper
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
            System.Diagnostics.Debug.WriteLine("[MethodProxy.Invoke]" + (new MethodCallInfo() { Name = targetMethod.Name, Args = args.ToList() }).ToString());
            BeforeInvokeEventArgs beforeArgs = new BeforeInvokeEventArgs()
            {
                MethodInfo = targetMethod,
                Args = args,
            };
            BeforeInvoke?.Invoke(this, beforeArgs);
            if (beforeArgs.Handle)
            {
                return beforeArgs.FakeResult;
            }
            object result;
            switch (_invokeType)
            {
                case InvokeType.Proxy:
                    result = _invokerProxy.InvokeAsync(targetMethod, args);
                    break;
                case InvokeType.ProxyFunc:
                    result = GetIInvokerProxyFunc().InvokeAsync(targetMethod, args);
                    break;
                case InvokeType.Object:
                    result = targetMethod.Invoke(_invokeObject, args);
                    break;
                default:
                    throw new Exception("InvokeProxy: unknown InvokeType");
            }
            if (targetMethod.ReturnType?.BaseType == typeof(Task))
            {
                // Task<Object> to Task<T>
                var convertMethod = typeof(MethodProxy).GetMethod("ConvertTaskObject");
                if (convertMethod != null)
                {
                    Type type = targetMethod.ReturnType.GetGenericArguments()[0];
                    var genericMethod = convertMethod.MakeGenericMethod(type);
                    result = genericMethod.Invoke(null, new object[] { result });
                }
            }
            else if (targetMethod.ReturnType == typeof(Task))
            {
                result = Task.FromResult<object>(null);
            }
            AfterInvokeEventArgs afterArgs = new AfterInvokeEventArgs()
            {
                MethodInfo = targetMethod,
                Args = args,
                Result = result,
            };
            AfterInvoke?.Invoke(this, afterArgs);
            return result;
        }

        public static async Task<T> ConvertTaskObject<T>(Task<object> task)
        {
            return (T)(await task);
        }
    }
}
