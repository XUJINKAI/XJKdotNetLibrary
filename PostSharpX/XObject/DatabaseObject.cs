using PostSharp.Extensibility;
using PostSharp.Patterns.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.XObject.DefaultProperty;
using XJK.XObject.NotifyProperty;
using XJK.XObject.Serializers;

namespace XJK.XObject
{
    /// <summary>
    /// Auto Notify, Xml Serialization Enhancement, DefaultValue Enhancement (Aggregatable, Observable as well, but not recommended)
    /// </summary>
    [Serializable]
    [IExXmlSerialization(ExXmlType.Database)]
    [ImplementIExXmlSerializable]
    [ImplementIDefaultProperty]
    public abstract class DatabaseObject : NotifyObject, IXmlSerializable, IExXmlSerializable, IDefaultProperty
    {
        public DatabaseObject()
        {
            InitializeProperties();
        }

        /// <summary>
        /// Set properties by Default value which has DefaultValue Attributes
        /// </summary>
        public void InitializeProperties()
        {
            DefaultPropertyMethods.ResetInstaceAllPropertiesByDefaultValue(this, ValueDefaultType.HasAttribute);
        }

        //IExXmlSerializable

        [XmlIgnore]
        [IgnoreAutoChangeNotification]
        public virtual string ParseError => throw new NotImplementedException();

        public virtual string GetXmlData()
        {
            throw new NotImplementedException();
        }

        public virtual void SetByXml(string xml)
        {
            throw new NotImplementedException();
        }

        // IXmlSerializable

        public virtual XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        // IDefaultProperty

        public virtual object GetPropertyDefaultValue(string PropertyName, out ValueDefaultType valueDefaultType)
        {
            throw new NotImplementedException();
        }

        public virtual object ResetPropertyDefaultValue(string PropertyName, out ValueDefaultType valueDefaultType)
        {
            throw new NotImplementedException();
        }

        public virtual void ResetAllPropertiesDefaultValue(ValueDefaultType filterType = (ValueDefaultType)(-1))
        {
            throw new NotImplementedException();
        }

    }
}
