using PostSharp.Extensibility;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.XObject.Serializers
{
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict, PersistMetaData = true, AllowMultiple = false, TargetTypeAttributes = MulticastAttributes.UserGenerated)]
    [Serializable]
    public class IExXmlSerializationAttribute : Attribute
    {
        public ExXmlType ExXmlType { get; }

        public IExXmlSerializationAttribute(ExXmlType exXmlType)
        {
            ExXmlType = exXmlType;
        }
    }
}
