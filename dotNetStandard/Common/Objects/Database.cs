using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.Serializers;

namespace XJK.Objects
{
    public class Database
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        public Database() { }

        public Database(string xmlText)
        {
            XmlText = xmlText;
        }

        private readonly DatabaseContent Content = new DatabaseContent();

        public string XmlText
        {
            get => Content.ToXmlText();
            set { ClearContent(); LoadInXmlText(value); }
        }

        public void SaveToFile(string filePath)
        {
            File.WriteAllText(filePath, XmlText);
        }

        public void LoadInXmlText(string xml)
        {
            var db = XmlSerialization.FromXmlText<DatabaseContent>(xml);
            foreach ((string key, object value) in db.Tuples())
            {
                this[key] = value;
            }
        }

        public object this[string key]
        {
            get => Content[key];
            set => Content[key] = value;
        }

        public bool HasKey(string key) => Content.ContainsKey(key);

        public void ClearContent()
        {
            Content.Clear();
        }

        public T Get<T>(string key, T defValue)
        {
            if (Content.TryGetValue(key, out object value))
            {
                if (value is T transValue)
                {
                    return transValue;
                }
            }
            return defValue;
        }

        public void Set(string key, object value)
        {
            this[key] = value;
        }


        public static Database FromText(string xmlText)
        {
            return new Database(xmlText);
        }

        public static T FromText<T>(string xmlText) where T : Database, new()
        {
            return new T { XmlText = xmlText };
        }

        public static Database FromFile(string filePath)
        {
            return new Database(File.ReadAllText(filePath));
        }

        public static T FromFile<T>(string filePath) where T : Database, new()
        {
            return new T() { XmlText = File.ReadAllText(filePath) };
        }
    }
}
