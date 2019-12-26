using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XJK.ReflectionUtils
{
    public static class InstancePropertyExtension
    {
        public static IEnumerable<PropertyInfo> GetProperties<T>(this T Instance)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
            var properties = from property in typeof(T).GetProperties(bindingFlags)
                             select property;
            return properties;
        }

        public static bool HasAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            return Attribute.IsDefined(propertyInfo, typeof(T));
        }

        public static object GetPropertyValue(this object Instance, string key)
        {
            var property = Instance.GetType().GetProperty(key);
            return property.GetValue(Instance);
        }

        public static bool TryGetPropertyValue(object Instance, string key, out object value)
        {
            value = null;
            try
            {
                value = GetPropertyValue(Instance, key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SetPropertyValue(this object Instance, string key, object value)
        {
            var property = Instance.GetType().GetProperty(key);
            if (!property.CanWrite) throw new ArgumentException($"[SetPropertyValue] property {property.Name} cannot write.");
            property.SetValue(Instance, value);
        }

        public static bool TrySetPropertyValue(object Instance, string key, object value)
        {
            try
            {
                SetPropertyValue(Instance, key, value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
