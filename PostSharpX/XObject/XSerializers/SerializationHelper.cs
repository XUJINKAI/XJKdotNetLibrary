using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using XJK.XStorage;
using PostSharp.Patterns.Model;

namespace XJK.XSerializers
{
    public static class SerializationHelper
    {
        internal const string _DATABASE_ = "XJK.Database";
        internal const string _COLLECTION_ = "XJK.Collection";
        internal const string _DICTIONARY_ = "XJK.Dictionary";
        internal const string _XTYPE_ = "XType";
        internal const string _TYPE_ = "Type";
        internal const string _TITEM_ = "TItem";
        internal const string _TKEY_ = "TKey";
        internal const string _TVALUE_ = "TValue";
        internal const string _ENTRY_ = "Entry";
        internal const string _ITEM_ = "Item";
        internal const string _Pair_ = "Pair";
        internal const string _KEY_ = "Key";
        internal const string _VALUE_ = "Value";

        // reader

        public static T ParseXml<T>(string xml, StringBuilder ParseErrors) where T : class
        {
            var root = XElement.Load(xml);
            return (T)ParseXmlRecursive(root, ParseErrors);
        }

        public static object ParseXmlRecursive(XElement element, StringBuilder ParseErrors)
        {
            if (element.Attribute(_XTYPE_)?.Value == _DATABASE_)
            {
                Type databaseType = GetTypeByAttribute(element);
                var obj = Activator.CreateInstance(databaseType);
                WriteDatabaseProperty(obj, element.Elements(), ParseErrors);
                return obj;
            }
            else if (element.Attribute(_XTYPE_)?.Value == _COLLECTION_)
            {
                Type collectionType = GetTypeByAttribute(element);
                var obj = Activator.CreateInstance(collectionType);
                WritDataCollection(obj, element.Elements(), ParseErrors);
                return obj;
            }
            else if (element.Attribute(_XTYPE_)?.Value == _DICTIONARY_)
            {
                Type dictType = GetTypeByAttribute(element);
                var obj = Activator.CreateInstance(dictType);
                WritDataDictionary(obj, element.Elements(), ParseErrors);
                return obj;
            }
            else
            {
                Type type = GetTypeByAttribute(element);
                var Serializer = new XmlSerializer(type);
                try
                {
                    return Serializer.Deserialize(new StringReader(element.InnerXml()));
                }
                catch (Exception ex)
                {
                    ParseErrors?.Append($"{ex.Message}{Environment.NewLine}{element.ToString()}{Environment.NewLine}");
                    return null;
                }
            }
        }

        public static void WriteDatabaseProperty(object obj, IEnumerable<XElement> elements, StringBuilder ParseErrors)
        {
            foreach (var element in elements)
            {
                try
                {
                    string key = element.Attribute(_KEY_).Value;
                    var newValue = ParseXmlRecursive(element, ParseErrors);
                    var property = obj.GetType().GetProperty(key);
                    var valueXType = element.Attribute(_XTYPE_);
                    var oldValue = property.GetValue(obj);
                    if (oldValue == null)
                    {
                        property.SetValue(obj, newValue);
                    }
                    else
                    {
                        switch (valueXType?.Value)
                        {
                            case _DATABASE_:
                                WriteDatabaseProperty(oldValue, element.Elements(), ParseErrors);
                                break;
                            case _COLLECTION_:
                                oldValue.GetType().GetMethod("Clear").Invoke(oldValue, null);
                                WritDataCollection(oldValue, element.Elements(), ParseErrors);
                                break;
                            case _DICTIONARY_:
                                oldValue.GetType().GetMethod("Clear").Invoke(oldValue, null);
                                WritDataDictionary(oldValue, element.Elements(), ParseErrors);
                                break;
                            default:
                                property.SetValue(obj, newValue);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ParseErrors?.Append($"{ex.GetFullMessage()}{Environment.NewLine}{element.ToString()}{Environment.NewLine}");
                }
            }
        }

        public static void WritDataCollection(object obj, IEnumerable<XElement> elements, StringBuilder ParseErrors)
        {
            foreach (var element in elements)
            {
                try
                {
                    if (element.Name != _ITEM_) throw new Exception($"Item.Name '{element.Name}' supposed to be {_ITEM_}.");
                    var value = ParseXmlRecursive(element, ParseErrors);
                    var itemtype = GetTypeByAttribute(element);
                    var method = obj.GetType().GetMethod("Add", new Type[] { itemtype });
                    method.Invoke(obj, new object[] { value });
                }
                catch (Exception ex)
                {
                    ParseErrors?.Append($"{ex.Message}{Environment.NewLine}{element.ToString()}{Environment.NewLine}");
                }
            }
        }

        public static void WritDataDictionary(object obj, IEnumerable<XElement> elements, StringBuilder ParseErrors)
        {
            foreach (var element in elements)
            {
                try
                {
                    if (element.Name != _Pair_) throw new Exception($"Item.Name '{element.Name}' supposed to be {_Pair_}.");
                    var KeyElement = element.Element(_KEY_);
                    var ValueElement = element.Element(_VALUE_);
                    var key = ParseXmlRecursive(KeyElement, ParseErrors);
                    var value = ParseXmlRecursive(ValueElement, ParseErrors);
                    var keyType = GetTypeByAttribute(KeyElement);
                    var valueType = GetTypeByAttribute(ValueElement);
                    var method = obj.GetType().GetMethod("Add", new Type[] { keyType, valueType });
                    method.Invoke(obj, new object[] { key, value });
                }
                catch (Exception ex)
                {
                    ParseErrors?.Append($"{ex.Message}{Environment.NewLine}{element.ToString()}{Environment.NewLine}");
                }
            }
        }

        public static Type GetTypeByAttribute(XElement element)
        {
            var type = element.Attribute(_TYPE_);
            if (type == null)
            {
                switch (element.Attribute(_XTYPE_)?.Value)
                {
                    case _DATABASE_:
                        return typeof(DatabaseObject);
                    case _COLLECTION_:
                        Type itemType = Type.GetType(element.Attribute(_TITEM_).Value);
                        return typeof(DataCollection<>).MakeGenericType(itemType);
                    case _DICTIONARY_:
                        Type keyType = Type.GetType(element.Attribute(_TKEY_).Value);
                        Type valueType = Type.GetType(element.Attribute(_TVALUE_).Value);
                        return typeof(DataDictionary<,>).MakeGenericType(keyType, valueType);
                }
            }
            else
            {
                return Type.GetType(type.Value);
            }
            throw new InvalidCastException($"Unknown Type {element.ToString()}");
        }

        // writer

        public static string GetXmlText(object obj, string RootName = "Root")
        {
            StringWriter stringWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true,
                IndentChars = "    ",
            });
            writer.WriteStartElement(RootName);
            WriteXmlRecursive(writer, obj);
            writer.WriteEndElement();
            writer.Flush();
            return stringWriter.ToString();
        }

        public static void WriteXmlRecursive(XmlWriter writer, object obj)
        {
            Type type = obj.GetType();
            if (Attribute.IsDefined(type, typeof(XmlDataPropertyAttribute)))
            {
                writer.WriteAttributeString(_XTYPE_, _DATABASE_);
                writer.WriteAttributeString(_TYPE_, SerializableTypeName(type));
                var ignoreTypes = type.GetCustomAttributes(typeof(IgnoreSerializeTypeAttribute))
                    .Select(att => ((IgnoreSerializeTypeAttribute)att).Type);
                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
                var properties = from property in type.GetProperties(bindingFlags)
                                 where (property.CanWrite 
                                        || Attribute.IsDefined(property.PropertyType, typeof(XmlDataPropertyAttribute))
                                        || Attribute.IsDefined(property.PropertyType, typeof(XmlDataCollectionAttribute))
                                        || Attribute.IsDefined(property.PropertyType, typeof(XmlDataDictionaryAttribute))
                                        )
                                    && !Attribute.IsDefined(property, typeof(XmlIgnoreAttribute))
                                    && !Attribute.IsDefined(property, typeof(ParentAttribute))
                                    && !Attribute.IsDefined(property, typeof(ReferenceAttribute))
                                    && !ignoreTypes.Any(t => t.IsAssignableFrom(property.PropertyType))
                                 select property;
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
            }
            else if (Attribute.IsDefined(type, typeof(XmlDataCollectionAttribute)))
            {
                writer.WriteAttributeString(_XTYPE_, _COLLECTION_);
                if (type.GetGenericTypeDefinition() != typeof(DataCollection<>))
                    writer.WriteAttributeString(_TYPE_, SerializableTypeName(type.GetGenericTypeDefinition()));
                Type genericType = type.GetGenericArguments().First();
                writer.WriteAttributeString(_TITEM_, SerializableTypeName(genericType));
                MethodInfo method = typeof(SerializationHelper).GetMethod(nameof(WriteCollectionXml)).MakeGenericMethod(genericType);
                method.Invoke(null, new object[] { writer, obj });
            }
            else if (Attribute.IsDefined(type, typeof(XmlDataDictionaryAttribute)))
            {
                writer.WriteAttributeString(_XTYPE_, _DICTIONARY_);
                if (type.GetGenericTypeDefinition() != typeof(DataDictionary<,>))
                    writer.WriteAttributeString(_TYPE_, SerializableTypeName(type.GetGenericTypeDefinition()));
                Type TKeyType = type.GetGenericArguments()[0];
                Type TValueType = type.GetGenericArguments()[1];
                writer.WriteAttributeString(_TKEY_, SerializableTypeName(TKeyType));
                writer.WriteAttributeString(_TVALUE_, SerializableTypeName(TValueType));
                MethodInfo method = typeof(SerializationHelper).GetMethod(nameof(WriteDictionaryXml)).MakeGenericMethod(TKeyType, TValueType);
                method.Invoke(null, new object[] { writer, obj });
            }
            else
            {
                var Serializer = new XmlSerializer(type);
                var valueType = obj.GetType();
                writer.WriteAttributeString(_TYPE_, SerializableTypeName(valueType));
                Serializer.Serialize(writer, obj);
            }
        }

        public static void WriteCollectionXml<T>(XmlWriter writer, ICollection<T> dataCollection)
        {
            foreach (var item in dataCollection)
            {
                writer.WriteStartElement(_ITEM_);
                WriteXmlRecursive(writer, item);
                writer.WriteEndElement();
            }
        }

        public static void WriteDictionaryXml<TKey, TValue>(XmlWriter writer, IDictionary<TKey,TValue> dataDictionary)
        {
            foreach (var item in dataDictionary)
            {
                writer.WriteStartElement(_Pair_);
                writer.WriteStartElement(_KEY_);
                WriteXmlRecursive(writer, item.Key);
                writer.WriteEndElement();
                writer.WriteStartElement(_VALUE_);
                WriteXmlRecursive(writer, item.Value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        public static string SerializableTypeName(Type type)
        {
            return type.Assembly.GetName().Name == "mscorlib" ? type.FullName : type.AssemblyQualifiedName;
        }
        
    }
}
