using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XJK.Storage
{
    public static class SerializationHelper
    {
        internal const string _DATABASE_ = "XJK.Database";
        internal const string _COLLECTION_ = "XJK.Collection";
        internal const string _TYPE_ = "Type";
        internal const string _ENTRY_ = "Entry";
        internal const string _KEY_ = "Key";
        internal const string _ITEM_ = "Item";
        
        // reader
        
        public static void OverrideDatabaseProperty(object obj, string xml, ref List<ParseError> ParseErrors)
        {
            StringReader stringReader = new StringReader(xml);
            XmlReader reader = XmlReader.Create(stringReader);
            OverrideDatabaseProperty(obj, reader, ref ParseErrors);
        }

        public static void OverrideDatabaseProperty(object obj, XmlReader reader, ref List<ParseError> ParseErrors)
        {
            var root = XElement.Load(reader);
            OverrideDatabaseProperty(obj, root, ref ParseErrors);
        }

        public static void OverrideDatabaseProperty(object obj, XElement element, ref List<ParseError> ParseErrors)
        {
            if (element.Attribute(_TYPE_).Value == _DATABASE_) element = element.Elements().First();
            foreach (var entry in element.Elements(_ENTRY_))
            {
                string key = entry.Attribute(_KEY_).Value;
                string entrytype = entry.Attribute(_TYPE_).Value;
                try
                {
                    var value = ParseXmlRecursive(entry.Elements().First(), entrytype, ref ParseErrors);
                    obj.GetType().GetProperty(key).SetValue(obj, value);
                }
                catch (Exception ex)
                {
                    ParseErrors.Add(new ParseError() { ErrorXml = entry.ToString(), Exception = ex });
                }
            }
        }

        public static void CollectionAddRange(object obj, XmlReader reader, ref List<ParseError> ParseErrors)
        {
            var root = XElement.Load(reader);
            CollectionAddRange(obj, root, ref ParseErrors);
        }

        public static void CollectionAddRange(object obj, XElement element, ref List<ParseError> ParseErrors)
        {
            if (element.Attribute(_TYPE_).Value == _COLLECTION_) element = element.Elements().First();
            foreach (var item in element.Elements(_ITEM_))
            {
                string itemtype = item.Attribute(_TYPE_).Value;
                try
                {
                    var value = ParseXmlRecursive(item.Elements().First(), itemtype, ref ParseErrors);
                    obj.GetType().GetMethod("Add").Invoke(obj, new object[] { value });
                }
                catch (Exception ex)
                {
                    ParseErrors.Add(new ParseError() { ErrorXml = item.ToString(), Exception = ex });
                }
            }
        }

        public static object ParseXmlRecursive(XElement element, string typeString, ref List<ParseError> ParseErrors)
        {
            if (element.Name == _DATABASE_)
            {
                Type databaseType = Type.GetType(element.Attribute(_TYPE_).Value);
                var obj = Activator.CreateInstance(databaseType);
                OverrideDatabaseProperty(obj, element, ref ParseErrors);
                return obj;
            }
            else if (element.Name == _COLLECTION_)
            {
                Type itemType = Type.GetType(element.Attribute(_TYPE_).Value);
                Type collectionType = typeof(DataCollection<>).MakeGenericType(itemType);
                var obj = Activator.CreateInstance(collectionType);
                CollectionAddRange(obj, element, ref ParseErrors);
                return obj;
            }
            else if (typeString == _DATABASE_ || typeString == _COLLECTION_)
            {
                return ParseXmlRecursive(element.Elements().First(), typeString, ref ParseErrors);
            }
            else
            {
                Type type = Type.GetType(typeString);
                var Serializer = new XmlSerializer(type);
                return Serializer.Deserialize(XmlReader.Create(new StringReader(element.ToString())));
            }
        }
        
        // writer

        public static string GetXmlText(object obj, string RootName = null)
        {
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true,
                IndentChars = "    ",
            });
            writer.WriteStartElement(RootName ?? obj.GetType().Name);
            WriteXmlRecursive(writer, obj);
            writer.WriteEndElement();
            writer.Flush();
            return stringWriter.ToString();
        }

        public static void WriteXmlRecursive(XmlWriter writer, object obj)
        {
            Type type = obj.GetType();
            if (obj is DatabaseObject databaseObject)
            {
                writer.WriteAttributeString(_TYPE_, _DATABASE_);
                writer.WriteStartElement(_DATABASE_);
                writer.WriteAttributeString(_TYPE_, SerializableTypeName(type));
                var properties = SelectDatabaseProperties(databaseObject);
                foreach (var property in properties)
                {
                    string key = property.Name;
                    object value = property.GetValue(obj);
                    if (value == null) continue;
                    writer.WriteStartElement(_ENTRY_);
                    writer.WriteAttributeString(_KEY_, key);
                    WriteXmlRecursive(writer, value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DataCollection<>))
            {
                writer.WriteAttributeString(_TYPE_, _COLLECTION_);
                writer.WriteStartElement(_COLLECTION_);
                Type genericType = type.GetGenericArguments().First();
                writer.WriteAttributeString(_TYPE_, SerializableTypeName(genericType));
                MethodInfo method = typeof(SerializationHelper).GetMethod(nameof(WriteCollectionXml)).MakeGenericMethod(genericType);
                method.Invoke(null, new object[] { writer, obj });
                writer.WriteEndElement();
            }
            else
            {
                var Serializer = new XmlSerializer(type);
                var valueType = obj.GetType();
                writer.WriteAttributeString(_TYPE_, SerializableTypeName(valueType));
                Serializer.Serialize(writer, obj);
            }
        }

        public static void WriteCollectionXml<T>(XmlWriter writer, DataCollection<T> dataCollection)
        {
            foreach (var item in dataCollection)
            {
                writer.WriteStartElement(_ITEM_);
                WriteXmlRecursive(writer, item);
                writer.WriteEndElement();
            }
        }
        
        public static string SerializableTypeName(Type type)
        {
            return type.Assembly.GetName().Name == "mscorlib" ? type.FullName : type.AssemblyQualifiedName;
        }

        public static IEnumerable<PropertyInfo> SelectDatabaseProperties<T>(T databaseObj) where T : DatabaseObject
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
            return from property in databaseObj.GetType().GetProperties(bindingFlags)
                   where property.CanWrite && !Attribute.IsDefined(property, typeof(XmlIgnoreAttribute))
                   select property;
        }

    }
}
