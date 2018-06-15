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

namespace AOP
{
    public class Message
    {
        public object Msg { get; set; }

        public static Message InvokeMethod(MethodInfo targetMethod, object[] args)
        {
            var Client = new Client();
            var result = targetMethod.Invoke(Client, args);
            return new Message() { Msg = result };
        }
    }
    
    public class ClientInvoker : IInvokerProxy
    {
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
            object result;
            result = Message.InvokeMethod(targetMethod, args).Msg;
            return result;
        }
    }
}
