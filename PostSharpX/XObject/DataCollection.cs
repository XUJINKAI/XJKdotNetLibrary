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
    {
        #region Virtual Interface

        [XmlIgnore]
        [IgnoreAutoChangeNotification]
        public virtual string ParseError => throw new NotImplementedException();

        public virtual XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public virtual string GetXmlData()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public virtual void SetByXml(string xml)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

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
            Clear();
        }

        #endregion

        public new void Clear()
        {
            if (Count > 0)
            {
                RemoveRange(0, Count);
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        [Reference] private readonly Dictionary<object, PropertyChangedEventHandler> HandlerDict = new Dictionary<object, PropertyChangedEventHandler>();

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public DataCollection(params T[] items) : this()
        {
            this.AddRange(items);
        }

        public DataCollection()
        {
            this.CollectionChanged += DataCollection_CollectionChanged;
        }

        protected void DataCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Reset)
            {
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
