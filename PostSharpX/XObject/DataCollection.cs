using PostSharp.Patterns.Collections;
using PostSharp.Patterns.Model;
using PostSharp.Patterns.Recording;
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
using XJK.XObject.DefaultProperty;
using XJK.XObject.NotifyProperty;
using XJK.XObject.Serializers;

namespace XJK.XObject
{
    /// <summary>
    /// Aggregatable, Observable, Serializable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [IExXmlSerialization(ExXmlType.Collection)]
    [ImplementIExXmlSerializable]
    public class DataCollection<T> : AdvisableCollection<T>, INotifyPropertyChanged, IXmlSerializable, IExXmlSerializable, IDefaultProperty
        ,ICollection<T>, IList<T>, ICollection, IList
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        [XmlIgnore] [IgnoreAutoChangeNotification] public virtual string ParseError => throw new NotImplementedException();

        [Reference] private readonly Dictionary<object, PropertyChangedEventHandler> HandlerDict = new Dictionary<object, PropertyChangedEventHandler>();

        // Fix Clear bug
        public new void Clear() { while (this.Count > 0) this.RemoveAt(0); }
        void ICollection<T>.Clear() { Clear(); }
        void IList.Clear() { Clear(); }
        
        public DataCollection()
        {
            this.CollectionChanged += DataCollection_CollectionChanged;
        }

        public DataCollection(params T[] items) : this()
        {
            this.AddRange(items);
        }

        protected void DataCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int addCount = e.NewItems?.Count ?? 0;
            int removeCount = e.OldItems?.Count ?? 0;
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                removeCount = HandlerDict.Count;
                HandlerDict.Clear();
            }
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
            OnPropertyChanged(new PropertyChangedEventArgs($"({(addCount > 0 ? $"+{addCount}" : "")}{(removeCount > 0 ? $"-{removeCount}" : "")})"));
        }

        public virtual XmlSchema GetSchema() => throw new NotImplementedException();
        public virtual string GetXmlData() => throw new NotImplementedException();
        public virtual void ReadXml(XmlReader reader) => throw new NotImplementedException();
        public virtual void SetByXml(string xml) => throw new NotImplementedException();
        public virtual void WriteXml(XmlWriter writer) => throw new NotImplementedException();
        public object GetPropertyDefaultValue(string PropertyName, out ValueDefaultType valueDefaultType) => throw new NotImplementedException();
        public object ResetPropertyDefaultValue(string PropertyName, out ValueDefaultType valueDefaultType) => throw new NotImplementedException();
        public void ResetAllPropertiesDefaultValue(ValueDefaultType filterType = (ValueDefaultType)(-1)) => Clear();

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        private PropertyChangedEventHandler MakeNestedPropertyChangedHandler(long id) => (sender, e) => OnPropertyChanged(new NestedPropertyChangedEventArgs($"[{id}]", e));
    }
}
