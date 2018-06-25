using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XJK.Network.Socket;
using XJK.Serializers;

namespace XJK.AOP.SocketRpc
{
    public abstract class SocketRpcBase : IInvokerProxy
    {
        public abstract void DispatchInvoke(Action action);
        protected abstract object GetExcuteObject();
        
        SocketPipeClient SocketPipeClient;

        private async Task<MethodCallInfo> SendMessageAsync(MethodCallInfo methodCallInfo)
        {
            byte[] bytes = BytesSerialization.ObjectToBytesArray(methodCallInfo);
            var response = await SocketPipeClient.SendBytes(bytes);
            MethodCallInfo result = (MethodCallInfo)BytesSerialization.BytesArrayToObject(response);
            return result;
        }

        public void AfterInvoke(object sender, AfterInvokeEventArgs args)
        {

        }

        public void BeforeInvoke(object sender, BeforeInvokeEventArgs args)
        {

        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
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
            return result;
        }
    }
}
