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
                response.Result = await methodCallInfo.InvokeAsync(GetExcuteObject());
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
            Debug.WriteLine($"[RpcCommBase.InvokeAsync] {targetMethod}");
            MethodCallInfo methodCall = new MethodCallInfo()
            {
                Name = targetMethod.Name,
                Args = new List<object>(args),
            };
            object result = SendMessageAsync(methodCall).ContinueWith(
                async task =>
                {
                    return (await task).Result;
                }).Unwrap();
            Debug.WriteLine($"[RpcCommBase.InvokeAsync] result {result}");
            return result;
        }
    }
}
