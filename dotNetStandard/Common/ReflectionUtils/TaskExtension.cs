using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XJK.ReflectionUtils
{
    public static class TaskExtension
    {
        public static async Task<object> InvokeAsync(this MethodInfo methodInfo, object obj, params object[] parameters)
        {
            var invokeResult = methodInfo.Invoke(obj, parameters);
            if (invokeResult is Task task)
            {
                await task;
                return invokeResult.GetType().GetProperty("Result").GetValue(invokeResult);
            }
            return invokeResult;
        }
    }
}
