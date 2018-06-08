using System.Reflection;
using System.Threading.Tasks;

namespace XJKdotNetLibrary.MethodWrapper
{
    public interface IInvokerProxy
    {
        Task<object> InvokeAsync(MethodInfo targetMethod, object[] args);
    }
}
