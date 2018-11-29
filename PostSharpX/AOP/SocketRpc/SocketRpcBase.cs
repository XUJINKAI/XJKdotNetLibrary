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
        protected abstract Task<byte[]> SendBytesAsync(byte[] Bytes);
        
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
            byte[] methodCallBytes = BytesSerialization.ObjectToBytesArray(methodCall);
            object result = SendBytesAsync(methodCallBytes).ContinueWith(
                async task =>
                {
                    byte[] responseBytes = await task;
                    MethodCallInfo responseMethodCall = (MethodCallInfo)BytesSerialization.BytesArrayToObject(responseBytes);
                    return responseMethodCall.Result;
                }).Unwrap();
            return result;
        }
    }
}
