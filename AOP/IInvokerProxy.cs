using System.Reflection;
using System.Threading.Tasks;

namespace XJK.AOP
{
    public interface IInvokerProxy
    {
        object Invoke(MethodInfo targetMethod, object[] args);
        void BeforeInvoke(object sender, BeforeInvokeEventArgs args);
        void AfterInvoke(object sender, AfterInvokeEventArgs args);
    }
}
