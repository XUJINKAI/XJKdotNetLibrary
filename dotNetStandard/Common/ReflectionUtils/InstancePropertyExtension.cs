using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.ReflectionUtils
{
    public static class InstancePropertyExtension
    {
        public static object GetPropertyValue(object Instance, string key)
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

        public static void SetPropertyValue(object Instance, string key, object value)
        {
            var property = Instance.GetType().GetProperty(key);
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
