using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XJK.DefaultProperty
{
    /// <summary>
    /// Extension methods for <see cref="IDefaultPropertyExtension"/>
    /// </summary>
    public static class DefaultPropertyExtension
    {
        public static object GetPropertyDefaultValue(this IDefaultPropertyExtension Instance, string PropertyName)
        {
            var property = Instance.GetType().GetProperty(PropertyName);
            return GetPropertyDefaultValue(Instance, property);
        }

        public static object GetPropertyDefaultValue(this IDefaultPropertyExtension Instance, PropertyInfo property)
        {
            // DefaultValue
            if (property.GetCustomAttribute(typeof(DefaultValueAttribute)) is DefaultValueAttribute valueAttr)
            {
                return valueAttr.Value;
            }
            // DefaultValueByMethod
            if (property.GetCustomAttribute(typeof(DefaultValueByMethodAttribute)) is DefaultValueByMethodAttribute methodAttr)
            {
                var method = Instance.GetType().GetMethod(methodAttr.MethodName);
                if (method == null)
                    throw new Exception();
                return method.Invoke(Instance, null);
            }
            // NewInstance
            if (property.GetCustomAttribute(typeof(DefaultValueNewInstanceAttribute)) is DefaultValueNewInstanceAttribute)
            {
                return DefaultValue.GetDefault(property.PropertyType);
            }
            // ValueType
            if (property.PropertyType.IsValueType)
            {
                return DefaultValue.GetDefault(property.PropertyType);
            }
            // object
            return null;
        }

        public static void ResetAllProperties(this IDefaultPropertyExtension Instance)
        {
            var properties = from property in Instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                             select property;
            ResetProperties(Instance, properties);
        }

        public static void ResetProperties(this IDefaultPropertyExtension Instance, params string[] PropertyNames)
        {
            var properties = from property in Instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                             where PropertyNames.Contains(property.Name)
                             select property;
            ResetProperties(Instance, properties);
        }

        public static void ResetProperties(this IDefaultPropertyExtension Instance, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    var value = Instance.GetPropertyDefaultValue(property);
                    property.SetValue(Instance, value);
                }
                else if (property.CanRead && property.GetValue(Instance) is IDefaultPropertyExtension propertyInstance)
                {
                    propertyInstance.ResetAllProperties();
                }
            }
        }
    }
}
