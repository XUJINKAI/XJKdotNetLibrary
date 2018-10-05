using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XJK;
using XJK.AOP;
using XJK.Serializers;

namespace AOPExample
{
    public class ClientInvoker : IInvokerProxy
    {
        public async Task<MethodCallInfo> SendMessageAsync(MethodCallInfo methodCallInfo)
        {
            var Client = new Client();
            var response = new MethodCallInfo()
            {
                Name = methodCallInfo.Name,
            };
            var result = await methodCallInfo.InvokeAsync(Client);
            response.Result = result;
            return response;
        }

        public void AfterInvoke(object sender, AfterInvokeEventArgs args)
        {
            Log.Info(args);
        }

        public void BeforeInvoke(object sender, BeforeInvokeEventArgs args)
        {
            Log.Info(args);
        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            MethodCallInfo methodCall = new MethodCallInfo()
            {
                Name = targetMethod.Name,
                Args = new List<object>(args),
            };
            var task = SendMessageAsync(methodCall);
            var result = task.ContinueWith(async t => { return (await t).Result; }).Unwrap();
            return result;
        }
    }
}
