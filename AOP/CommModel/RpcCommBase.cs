using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace XJK.AOP.CommunicationModel
{
    public abstract class RpcCommBase : IInvokerProxy
    {
        public abstract bool IsConnceted();
        protected abstract object GetExcuteObject();
        protected abstract Task<MethodCallInfo> SendMessageAsync(MethodCallInfo methodCallInfo);

        public virtual void BeforeInvoke(object sender, BeforeInvokeEventArgs args)
        {
            if (!IsConnceted())
            {
                throw new Exception("[RpcCommBase.Invoke] Not Connected");
            }
        }
        public virtual void AfterInvoke(object sender, AfterInvokeEventArgs args) { }

        protected async Task OnReceiveMethodCallAsync(MethodCallInfo methodCallInfo, Func<MethodCallInfo, Task<string>> SendResponse)
        {
            Log.Debug($"[RpcCommBase.Recive] {methodCallInfo}");
            var response = new MethodCallInfo()
            {
                Name = methodCallInfo.Name,
            };
            try
            {
                response.Result = methodCallInfo.Excute(GetExcuteObject());
                Log.Debug($"[RpcCommBase.Recive] {methodCallInfo}, result [{response.Result}]");
            }
            catch (Exception ex)
            {
                Debugger.Break();
                Log.Error(ex);
                response.Exception = ex.GetFullMessage();
            }
            var response_result = await SendResponse(response);
            Log.Debug($"[RpcCommBase.Recive] {methodCallInfo}, response result [{response_result}]");
        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            Debug.WriteLine($"[RpcCommBase] InvokeAsync {targetMethod}");
            MethodCallInfo methodCall = new MethodCallInfo()
            {
                Name = targetMethod.Name,
                Args = new List<object>(args),
            };

            object result = null; Log.TagDebugger();
            if (targetMethod.ReturnType?.BaseType == typeof(Task))
            {
                Log.TagDebugger();
                var generic_method = typeof(RpcCommBase).GetMethod("SendAsyncTaskGenericHelperFunc");
                var method = generic_method.MakeGenericMethod(targetMethod.ReturnType.GenericTypeArguments[0]);
                result = method.Invoke(this, new object[] { methodCall });
            }
            else if (targetMethod.ReturnType == typeof(Task))
            {
                Log.TagDebugger();
                var method = typeof(RpcCommBase).GetMethod("SendAsyncTaskHelperFunc");
                result = method.Invoke(this, new object[] { methodCall });
            }
            else
            {
                throw new Exception("[RpcCommBase] Type of result must be Task");
            }
            Log.TagDebugger();
            result.AssertType(targetMethod.ReturnType);
            return result;
        }

        public async Task SendAsyncTaskHelperFunc(MethodCallInfo methodCall)
        {
            var re = await SendMessageAsync(methodCall); Log.TagDebugger();
            if (re.Exception != null) Log.Error(re.Exception);
            Debug.WriteLine("re.Dump() " + re.Dump());
        }

        public async Task<T> SendAsyncTaskGenericHelperFunc<T>(MethodCallInfo methodCall)
        {
            var re = await SendMessageAsync(methodCall); Log.TagDebugger();
            if (re.Exception != null)
            {
                Log.Error(re.Exception);
            }
            return (T)re.Result;
        }
    }
}
