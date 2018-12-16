using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XJK
{
    public static class ReflectionExtension
    {
        public static object GetProperty<T>(this T obj, string propertyName)
        {
            return typeof(T).GetProperty(propertyName).GetValue(obj);
        }

        public static object InvokeMethod<T>(this T obj, string methodName, params object[] args)
        {
            return typeof(T).GetMethod(methodName).Invoke(obj, args);
        }
    }
}
