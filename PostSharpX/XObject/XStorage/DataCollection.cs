using PostSharp.Patterns.Collections;
using PostSharp.Patterns.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.NotifyProperty;
using XJK.Serializers;
using XJK.XSerializers;

namespace XJK.XStorage
{
    /// <summary>
    /// Aggregatable, Observable, Serializable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [XmlDataCollection]
    [Serializable]
    public class DataCollection<T> : AdvisableCollection<T>, IXmlSerializable, INotifyPropertyChanged, IXmlParseData
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        private readonly Dictionary<object, PropertyChangedEventHandler> HandlerDict = new Dictionary<object, PropertyChangedEventHandler>();

        public virtual XmlSchema GetSchema() { return null; }
        public virtual void ReadXml(XmlReader reader) { }
        public virtual void WriteXml(XmlWriter writer) { }
        [XmlIgnore]
        public virtual string ParseError { get; }
        public virtual string GetXmlData() { return null; }
        public virtual void SetByXml(string xml) { }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public DataCollection()
        {
            this.CollectionChanged += DataCollection_CollectionChanged;
        }

        protected void DataCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged notify)
                    {
                        var id = IndexOf((T)item);
                        var handler = MakeNestedPropertyChangedHandler(id);
                        notify.PropertyChanged += handler;
                        HandlerDict.Add(item, handler);
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged notify)
                    {
                        var handler = HandlerDict[item];
                        notify.PropertyChanged -= handler;
                        HandlerDict.Remove(item);
                    }
                }
            }
            OnPropertyChanged(new PropertyChangedEventArgs($"(+{e.NewItems?.Count ?? 0},-{e.OldItems?.Count ?? 0})"));
        }

        private PropertyChangedEventHandler MakeNestedPropertyChangedHandler(long id)
        {
            return (sender, e) =>
            {
                OnPropertyChanged(new NestedPropertyChangedEventArgs($"[{id}]", e));
            };
        }
        
        // enhance

        public void ForEach(Action<T> action)
        {
            foreach (var x in this)
            {
                action(x);
            }
        }
    }
}
