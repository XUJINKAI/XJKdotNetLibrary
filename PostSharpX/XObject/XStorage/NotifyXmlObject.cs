using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.NotifyProperty;
using XJK.XSerializers;

namespace XJK.XStorage
{
    [Serializable]
    public class NotifyXmlObject : NotifyObject, IXmlSerializable, IXmlParseData
    {
        public virtual XmlSchema GetSchema() { return null; }
        public virtual void ReadXml(XmlReader reader) { }
        public virtual void WriteXml(XmlWriter writer) { }

        [XmlIgnore]
        [IgnoreAutoChangeNotification]
        public virtual string ParseError { get; }
        public virtual string GetXmlData() { return null; }
        public virtual void SetByXml(string xml) { }
    }
}
