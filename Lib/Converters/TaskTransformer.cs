using System;
using System.Threading.Tasks;
using XJK.ReflectionUtils;

namespace XJK.Converters
{
    public static class TaskTransformer
    {
        public static object ConvertTaskObject(Task<object> task, Type type)
        {
            var convertMethod = typeof(TaskTransformer).GetMethod(nameof(_ConvertTaskObjectToTaskTHelperFunction));
            var genericMethod = convertMethod.MakeGenericMethod(type);
            var result = genericMethod.Invoke(null, new object[] { task });
            return result;
        }

        public static Task<object> ConvertTaskReturnObject(Task task)
        {
            if (task.IsType(typeof(Task)))
            {
                return Task.Run<object>(async () => { await task; return null; });
            }
            Type type = task.GetType().GetGenericArguments()[0];
            var convertMethod = typeof(TaskTransformer).GetMethod(nameof(_ConvertTaskTToObjectHelperFunction));
            var genericMethod = convertMethod.MakeGenericMethod(type);
            var result = genericMethod.Invoke(null, new object[] { task });
            return (Task<object>)result;
        }

        // helper function

        public static async Task<TReturn> _ConvertTaskObjectToTaskTHelperFunction<TReturn>(Task<object> task)
        {
            return (TReturn)(await task.ConfigureAwait(false));
        }

        public static async Task<object> _ConvertTaskTToObjectHelperFunction<T>(Task<T> task)
        {
            return await task.ConfigureAwait(false);
        }
    }
}
