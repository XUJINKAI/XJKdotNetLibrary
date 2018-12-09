using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XJK.DefaultProperty
{
    public static class DefaultPropertyAttributeExtension
    {
        public static int WritePropertiesByDefaultValueAttribute(object isntance)
        {
            var attribute = typeof(DefaultValueAttribute);
            var attributeValue = attribute.GetProperty(nameof(DefaultValueAttribute.Value));
            var properties = from property in isntance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                             where property.CanWrite && Attribute.IsDefined(property, attribute)
                             select property;
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute(attribute);
                var value = attributeValue.GetValue(attr);
                prop.SetValue(isntance, value);
            }
            return properties.Count();
        }
    }
}
