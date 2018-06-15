using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XJK;
using XJK.AOP;
using XJK.Serializers;

namespace AOP
{
    public interface IInterceptTest
    {
        string ToBase64(string str);
        Task<string> ToBase64Async(string str);
    }

    public class InterceptTestClass : IInterceptTest
    {
        public static IInterceptTest GetProxy()
        {
            InterceptTestClass testClass = new InterceptTestClass();
            IInterceptTest interceptTest = MethodProxy.Intercept<IInterceptTest>(testClass,
            (sender, arg) =>
            {
                Log.Info(arg);
            }
            , (sender, arg) =>
            {
                Log.Info(arg);
            });
            return interceptTest;
        }

        public string ToBase64(string str)
        {
            Thread.Sleep(3000);
            return str.ToBase64BinaryString();
        }

        public Task<string> ToBase64Async(string str)
        {
            return Task<string>.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                return str.ToBase64BinaryString();
            });
        }
    }
}
