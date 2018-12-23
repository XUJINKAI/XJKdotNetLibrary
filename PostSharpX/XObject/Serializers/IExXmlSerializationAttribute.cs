using PostSharp.Extensibility;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.XObject.Serializers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
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
