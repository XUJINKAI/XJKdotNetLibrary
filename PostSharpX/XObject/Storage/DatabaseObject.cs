using PostSharp.Patterns.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.NotifyPropertyChanged;

namespace XJK.Storage
{
    [Serializable]
    public class DatabaseObject : NotifyObject, IXmlSerializable
    {
        public List<ParseError> ParseErrors = new List<ParseError>();

        public static T Parse<T>(string xml) where T : DatabaseObject, new()
        {
            T t = new T();
            SerializationHelper.OverrideDatabaseProperty(t, xml, ref t.ParseErrors);
            return t;
        }

        public string GetXml()
        {
            return SerializationHelper.GetXmlText(this);
        }

        public void OverrideProperties(string xml) 
        {
            SerializationHelper.OverrideDatabaseProperty(this, xml, ref ParseErrors);
        }
        
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            SerializationHelper.OverrideDatabaseProperty(this, reader, ref ParseErrors);
        }
        
        public void WriteXml(XmlWriter writer)
        {
            SerializationHelper.WriteXmlRecursive(writer, this);
        }
    }
}
