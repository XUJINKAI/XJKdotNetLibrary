using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using XJK.XObject.Serializers;

namespace XJK.XObject
{
    internal static class XConfig
    {
        public const BindingFlags PublicDeclaredPropertiesFlag = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;

        public static IEnumerable<PropertyInfo> Select_DefaultProperties(Type type)
        {
            return type.GetProperties(PublicDeclaredPropertiesFlag);
        }

        public static IEnumerable<PropertyInfo> Select_NotifyProperties(Type type, bool canWrite)
        {
            return from property in type.GetProperties(PublicDeclaredPropertiesFlag)
                   where property.CanWrite == canWrite
                        && !Attribute.IsDefined(property, typeof(IgnoreAutoChangeNotificationAttribute))
                        && !Attribute.IsDefined(property, typeof(ParentAttribute))
                        && !Attribute.IsDefined(property, typeof(ReferenceAttribute))
                   select property;
        }

        public static IEnumerable<PropertyInfo> Select_XmlSerializableProperties(Type type)
        {
            var ignoreTypes = type
                .GetCustomAttributes(typeof(IgnoreSerializeTypeAttribute))
                .Select(att => ((IgnoreSerializeTypeAttribute)att).Type);
            var properties = from property in type.GetProperties(PublicDeclaredPropertiesFlag)
                             where (property.CanWrite
                                    || Attribute.IsDefined(property.PropertyType, typeof(IExXmlSerializationAttribute))
                                    )
                                && !Attribute.IsDefined(property, typeof(XmlIgnoreAttribute))
                                && !Attribute.IsDefined(property, typeof(ParentAttribute))
                                && !Attribute.IsDefined(property, typeof(ReferenceAttribute))
                                && !ignoreTypes.Any(t => t.IsAssignableFrom(property.PropertyType))
                             select property;
            return properties;
        }
    }
}
