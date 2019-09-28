using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.DefaultProperty
{
    public static class DefaultValue
    {
        public static object GetDefault(Type type)
        {
            if (type == typeof(string)) return "";
            return Activator.CreateInstance(type);
        }

        public static object GetDefault<T>()
        {
            return GetDefault(typeof(T));
        }
    }
}
