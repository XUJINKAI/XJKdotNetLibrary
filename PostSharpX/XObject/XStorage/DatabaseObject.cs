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
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.NotifyProperty;
using XJK.XSerializers;

namespace XJK.XStorage
{
    /// <summary>
    /// Aggregatable, Observable, Serializable
    /// </summary>
    [Serializable]
    [XmlDataProperty]
    public class DatabaseObject : NotifyXmlObject
    {
        public static T Parse<T>(string xml) where T : DatabaseObject, new()
        {
            T result = new T();
            result.SetByXml(xml);
            return result;
        }
    }
}
