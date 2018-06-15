using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace XJK
{
    public static class TypeHelper
    {
        public static void AssertType(this object obj, Type type)
        {
            if (obj?.GetType() != type)
            {
                if (type == typeof(Task) &&
                obj.GetType().GetGenericTypeDefinition() == typeof(Task<>) &&
                obj.GetType().GetGenericArguments()[0].Name == "VoidTaskResult")
                {
                    return;
                }

                string error_msg = $"[AssertType] Not Match: expect '{type}', object type '{obj?.GetType()}', object value '{obj}'";

                Log.Error(error_msg);
#if DEBUG
                Debugger.Break();
#else
                throw new Exception(error_msg);
#endif
            }
        }

        public static object DefaultValue(this Type type)
        {
            object result = null;
            if (type == null)
            {
                result = null;
            }
            else if (type.IsValueType)
            {
                result = Activator.CreateInstance(type);
            }
            else if (type == typeof(string))
            {
                result = "";
            }
            else if (type == typeof(object))
            {
                result = null;
            }
            else if (type == typeof(Task))
            {
                result = Task.FromResult<object>(null);
            }
            else if (type.BaseType == typeof(Task))
            {
                var convertMethod = typeof(TypeHelper).GetMethod("WrapTaskHelperFunction");
                Type TType = type.GetGenericArguments()[0];
                var genericMethod = convertMethod.MakeGenericMethod(TType);
                var DefT = TType.DefaultValue();
                result = genericMethod.Invoke(null, new object[] { DefT });
            }
            else
            {
                Trace.TraceWarning($"TypeHelper Unknown Type: {type}");
            }
            Debug.WriteLine($"DefaultValue for [{type}] is [{result}]");
            return result;
        }
        
        public static object ConvertTaskObject(object task, Type type)
        {
            var convertMethod = typeof(TypeHelper).GetMethod("ConvertTaskObjectHelperFunction");
            var genericMethod = convertMethod.MakeGenericMethod(type);
            var result = genericMethod.Invoke(null, new object[] { task });
            return result;
        }

        // Helper Function

        public static Task<T> WrapTaskHelperFunction<T>(object value)
        {
            return Task.FromResult((T)value);
        }
        
        public static async Task<T> ConvertTaskObjectHelperFunction<T>(object task)
        {
            return (T)(await (Task<object>)task);
        }
    }
}
