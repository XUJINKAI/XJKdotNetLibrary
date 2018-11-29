using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.NotifyPropertyChanged;
using XJK.Serializers;

namespace XJK.Storage
{
    /// <summary>
    /// Observable and Serializable Collection
    /// </summary>
    /// <typeparam name="T">must be INotifyPropertyChanged</typeparam>
    [Serializable]
    public class DataCollection<T> : ObservableCollection<T>, INotifyPropertyChangedEx, IXmlSerializable
    {
        public List<ParseError> ParseErrors = new List<ParseError>();
        public event PropertyChangedEventHandlerEx PropertyChangedEx;

        private readonly string DataPropertyName = "Items";

        protected void OnPropertyChangedEx(PropertyChangedEventArgsEx e)
        {
            PropertyChangedEx?.Invoke(this, e);
        }

        public DataCollection()
        {
            CollectionChanged += ObservableCollectionEx_CollectionChanged;
        }

        protected void ObservableCollectionEx_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged notify)
                    {
                        notify.PropertyChanged += CollectionItem_PropertyChanged;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged notify)
                    {
                        notify.PropertyChanged -= CollectionItem_PropertyChanged;
                    }
                }
            }
            OnPropertyChangedEx(PropertyChangedEventArgsEx.NewCollectionChange(DataPropertyName, e.NewItems, e.OldItems));
        }

        protected void CollectionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChangedEx(PropertyChangedEventArgsEx.NewCollectionItemPropertyChange(DataPropertyName, sender, e.PropertyName));
        }

        // enhance

        public void ForEach(Action<T> action)
        {
            foreach (var x in this)
            {
                action(x);
            }
        }

        // Xml
        
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            SerializationHelper.CollectionAddRange(this, reader, ref ParseErrors);
        }

        public void WriteXml(XmlWriter writer)
        {
            SerializationHelper.WriteXmlRecursive(writer, this);
        }
    }
}
