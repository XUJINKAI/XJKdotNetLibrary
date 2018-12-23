using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XJK.XObject.DefaultProperty
{
    public static class DefaultPropertyMethods
    {
        /// <summary>
        /// Get DefaultValue by DefaultValueAttribute or DefaultValueByMethodAttribute
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="PropertyName"></param>
        /// <param name="defaultValueType"></param>
        /// <returns></returns>
        public static object GetPropertyDefaultValueEx(this object instance, string propertyName, out ValueDefaultType defaultValueType)
        {
            var property = instance.GetType().GetProperty(propertyName);
            return GetPropertyDefaultValueEx(instance, property, out defaultValueType);
        }

        public static object GetPropertyDefaultValueEx(this object instance, PropertyInfo property, out ValueDefaultType defaultValueType)
        {
            if (property.GetCustomAttribute(typeof(DefaultValueAttribute)) is DefaultValueAttribute valueAttr)
            {
                defaultValueType = ValueDefaultType.ValueAttribute;
                return valueAttr.Value;
            }
            if (property.GetCustomAttribute(typeof(DefaultValueByMethodAttribute)) is DefaultValueByMethodAttribute methodAttr)
            {
                defaultValueType = ValueDefaultType.MethodAttribute;
                var method = instance.GetType().GetMethod(methodAttr.MethodName);
                return method.Invoke(instance, null);
            }
            if (property.GetCustomAttribute(typeof(DefaultValueNewInstanceAttribute)) is DefaultValueNewInstanceAttribute)
            {
                defaultValueType = ValueDefaultType.NewInstanceAttribute;
                return GetDefault(property.PropertyType);
            }
            defaultValueType = ValueDefaultType.NoAttribute;
            if (property.PropertyType.IsValueType)
            {
                return GetDefault(property.PropertyType);
            }
            return null;
        }

        private static object GetDefault(Type type)
        {
            if (type == typeof(string)) return "";
            return Activator.CreateInstance(type);
        }
    }
}
