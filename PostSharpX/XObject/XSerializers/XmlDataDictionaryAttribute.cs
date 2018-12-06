using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Patterns.Model;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XJK.XSerializers
{
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(XmlDataPropertyAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(XmlDataCollectionAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(NotifyPropertyChangedAttribute))]
    [IntroduceInterface(typeof(IXmlSerializable), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [IntroduceInterface(typeof(IXmlParseData), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict, PersistMetaData = true, AllowMultiple = false, TargetTypeAttributes = MulticastAttributes.UserGenerated)]
    [PSerializable]
    public class XmlDataDictionaryAttribute : InstanceLevelAspect, IXmlSerializable, IXmlParseData
    {
        [XmlIgnore]
        [IgnoreAutoChangeNotification]
        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public string ParseError { get; private set; }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public string GetXmlData()
        {
            return SerializationHelper.GetXmlText(Instance);
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public void SetByXml(string xml)
        {
            StringReader stringReader = new StringReader(xml);
            XmlReader reader = XmlReader.Create(stringReader);
            ReadXml(reader);
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrFail)]
        public XmlSchema GetSchema()
        {
            return null;
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrFail)]
        public void ReadXml(XmlReader reader)
        {
            XElement root = XElement.Load(reader);
            StringBuilder stringBuilder = new StringBuilder();
            SerializationHelper.WritDataDictionary(Instance, root.Elements(), stringBuilder);
            ParseError = stringBuilder.ToString();
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrFail)]
        public void WriteXml(XmlWriter writer)
        {
            SerializationHelper.WriteXmlRecursive(writer, Instance);
        }
    }
}
