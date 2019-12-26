using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace XJK.ReflectionUtils
{
    public static class AttributeExtension
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

        public static int WriteFieldsByDefaultValueAttribute(object isntance)
        {
            var attribute = typeof(DefaultValueAttribute);
            var attributeValue = attribute.GetProperty(nameof(DefaultValueAttribute.Value));
            var fields = from field in isntance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                         where Attribute.IsDefined(field, attribute)
                         select field;
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute(attribute);
                var value = attributeValue.GetValue(attr);
                field.SetValue(isntance, value);
            }
            return fields.Count();
        }
    }
}
