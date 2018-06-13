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
            Debug.WriteLine($"[RpcCommBase.Recive] {methodCallInfo}");
            var response = new MethodCallInfo()
            {
                Name = methodCallInfo.Name,
            };
            try
            {
                response.Result = methodCallInfo.Excute(GetExcuteObject());
                Debug.WriteLine($"[RpcCommBase.Recive] {methodCallInfo}, result [{response.Result}]");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                response.Exception = ex.GetFullMessage();
            }
            var response_result = await SendResponse(response);
            Debug.WriteLine($"[RpcCommBase.Recive] {methodCallInfo}, response result [{response_result}]");
        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            Debug.WriteLine($"[RpcCommBase] InvokeAsync {targetMethod}");
            MethodCallInfo methodCall = new MethodCallInfo()
            {
                Name = targetMethod.Name,
                Args = new List<object>(args),
            };
            Debug.WriteLine($"[RpcCommBase] InvokeAsync {methodCall}");

            object result = null;
            if(targetMethod.ReturnType?.BaseType == typeof(Task))
            {
                var generic_method = typeof(RpcCommBase).GetMethod("SendAsyncTaskTHelperFunc");
                var method = generic_method.MakeGenericMethod(targetMethod.ReturnType.GenericTypeArguments[0]);
                result = method.Invoke(this, new object[] { methodCall });
            }
            else if(targetMethod.ReturnType == typeof(Task))
            {
                result = new Task(() => { });
            }

            result.AssertType(targetMethod.ReturnType);
            return result;
        }
        
        public async Task<T> SendAsyncTaskTHelperFunc<T>(MethodCallInfo methodCall)
        {
            var re = await SendMessageAsync(methodCall);
            return (T)re.Result;
        }
    }
}
