using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using XJKdotNetLibrary.MethodWrapper;

namespace XJKdotNetLibrary.CommunicationModel
{
    public abstract class RpcCommBase : IInvokerProxy
    {
        public abstract bool IsConnceted();
        protected abstract object GetExcuteObject();
        protected abstract Task<MethodCallInfo> SendMessageAsync(MethodCallInfo methodCallInfo);

        protected async Task OnReceiveMethodCallAsync(MethodCallInfo methodCallInfo, Func<MethodCallInfo, Task<string>> SendResponse)
        {
            LogDebug($"[ReciveInvoke] {methodCallInfo}");
            object excuteResult;
            try
            {
                excuteResult = methodCallInfo.Excute(GetExcuteObject());
                LogDebug("[ExcuteResult] " + excuteResult?.ToString());
            }
            catch (Exception ex)
            {
                LogError(ex);
                excuteResult = null;
            }
            if (excuteResult != null)
            {
                LogDebug("[SendingResponse]");
                var response = (new MethodCallInfo() { Name = methodCallInfo.Name, Result = excuteResult });
                var result = await SendResponse(response);
                LogDebug($"[Response] {result}");
            }
        }

        public async Task<object> InvokeAsync(MethodInfo targetMethod, object[] args)
        {
            if (!IsConnceted())
            {
                LogInfo("Not Connected");
                if (targetMethod.ReturnType.IsValueType)
                {
                    return Activator.CreateInstance(targetMethod.ReturnType);
                }
                return null;
            }
            MethodCallInfo methodCall = new MethodCallInfo()
            {
                Name = targetMethod.Name,
                Args = new List<object>(args),
            };
            LogDebug($"[SendInvoke] {methodCall}");
            var result = await SendMessageAsync(methodCall);
            return result?.Result;
        }

        
        protected virtual void LogDebug(string msg)
        {
            Trace.WriteLine("[RpcCommBase.LogDebug(virtual)]" + msg);
        }

        protected virtual void LogError(Exception ex)
        {
            Trace.WriteLine("[RpcCommBase.LogError(virtual)]" + ex.Message + "\r\n" + ex.StackTrace);
        }

        protected virtual void LogInfo(string msg)
        {
            Trace.WriteLine("[RpcCommBase.LogInfo(virtual)]" + msg);
        }
    }
}
