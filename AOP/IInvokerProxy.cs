using System.Reflection;
using System.Threading.Tasks;

namespace XJK.MethodWrapper
{
    public interface IInvokerProxy
    {
        Task<object> InvokeAsync(MethodInfo targetMethod, object[] args);
    }
}
