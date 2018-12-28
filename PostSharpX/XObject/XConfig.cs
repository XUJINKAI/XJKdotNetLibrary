using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using XJK.XObject.DefaultProperty;
using XJK.XObject.Serializers;

namespace XJK.XObject
{
    internal static class XConfig
    {
        public const BindingFlags PublicDeclaredPropertiesFlag = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;

        public static IEnumerable<PropertyInfo> GetChildroperties(Type type)
        {
            return from property in type.GetProperties(PublicDeclaredPropertiesFlag)
                   where !Attribute.IsDefined(property, typeof(ParentAttribute))
                        && !Attribute.IsDefined(property, typeof(ReferenceAttribute))
                   select property;
        }

        public static IEnumerable<PropertyInfo> Select_ResetAllDefaultProperties(Type type)
        {
            return GetChildroperties(type);
        }
        
        public static IEnumerable<PropertyInfo> Select_NotifyProperties(Type type, bool canWrite)
        {
            return from property in GetChildroperties(type)
                   where property.CanWrite == canWrite
                        && !Attribute.IsDefined(property, typeof(IgnoreAutoChangeNotificationAttribute))
                   select property;
        }

        public static IEnumerable<PropertyInfo> Select_XmlSerializableProperties(Type type)
        {
            var ignoreTypes = type
                .GetCustomAttributes(typeof(IgnoreSerializeTypeAttribute))
                .Select(att => ((IgnoreSerializeTypeAttribute)att).Type);
            var properties = from property in GetChildroperties(type)
                             where (property.CanWrite || Attribute.IsDefined(property.PropertyType, typeof(IExXmlSerializationAttribute)))
                                && !Attribute.IsDefined(property, typeof(XmlIgnoreAttribute))
                                && !ignoreTypes.Any(t => t.IsAssignableFrom(property.PropertyType))
                             select property;
            return properties;
        }
    }
}
