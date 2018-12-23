﻿using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Patterns.Model;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XJK.XObject.Serializers
{
    [IntroduceInterface(typeof(IXmlSerializable), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [IntroduceInterface(typeof(IExXmlSerializable), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict, PersistMetaData = true, AllowMultiple = false, TargetTypeAttributes = MulticastAttributes.UserGenerated)]
    [PSerializable]
    public class ImplementIExXmlSerializableAttribute : InstanceLevelAspect, IXmlSerializable, IExXmlSerializable
    {
        // IExXmlSerializable

        [XmlIgnore]
        [IgnoreAutoChangeNotification]
        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public string ParseError { get; private set; }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public string GetXmlData()
        {
            return SerializationHelper.GetObjectXmlText(Instance);
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public void SetByXml(string xml)
        {
            StringReader stringReader = new StringReader(xml);
            XmlReader reader = XmlReader.Create(stringReader);
            ReadXml(reader);
        }

        // IXmlSerializable

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public XmlSchema GetSchema()
        {
            return null;
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public void ReadXml(XmlReader reader)
        {
            var root = XElement.Load(reader);
            StringBuilder stringBuilder = new StringBuilder();
            if(Instance.GetType().GetCustomAttribute(typeof(IExXmlSerializationAttribute)) is IExXmlSerializationAttribute exXmlSerialization)
            {
                switch (exXmlSerialization.ExXmlType)
                {
                    case ExXmlType.Database:
                        SerializationHelper.ParseToProperties(Instance, root.Elements(), stringBuilder);
                        break;
                    case ExXmlType.Collection:
                        SerializationHelper.ParseToCollection(Instance, root.Elements(), stringBuilder);
                        break;
                    case ExXmlType.Dictionary:
                        SerializationHelper.ParseToDictionary(Instance, root.Elements(), stringBuilder);
                        break;
                }
            }
            ParseError = stringBuilder.ToString();
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public void WriteXml(XmlWriter writer)
        {
            SerializationHelper.WriteObjectToXmlWriterRecursive(writer, Instance);
        }

    }
}
