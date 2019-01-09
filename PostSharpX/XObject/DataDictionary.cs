using PostSharp.Patterns.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.XObject.DefaultProperty;
using XJK.XObject.NotifyProperty;
using XJK.XObject.Serializers;

namespace XJK.XObject
{
    /// <summary>
    /// Auto Notify, XML Serializable
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    [IExXmlSerialization(ExXmlType.Dictionary)]
    [ImplementIExXmlSerializable]
    public class DataDictionary<TKey, TValue> : 
        IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey,TValue>>
        , ICollection,IDictionary, IEnumerable
        , INotifyPropertyChanged, INotifyCollectionChanged
        , IXmlSerializable, IExXmlSerializable, IDefaultProperty
    {
        [XmlIgnore] private readonly Dictionary<TKey, TValue> Content = new Dictionary<TKey, TValue>();
        [XmlIgnore] private readonly Dictionary<TKey, PropertyChangedEventHandler> HandlerDict = new Dictionary<TKey, PropertyChangedEventHandler>();
        [XmlIgnore] public virtual string ParseError => throw new NotImplementedException();

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ICollection<TKey> Keys => Content.Keys;
        public ICollection<TValue> Values => Content.Values;
        public int Count => Content.Count;
        public bool IsReadOnly => false;
        public bool IsSynchronized => true;
        public object SyncRoot => Content;
        public bool IsFixedSize => false;
        ICollection IDictionary.Keys => Content.Keys;
        ICollection IDictionary.Values => Content.Values;
        public object this[object key]
        {
            get => Content[(TKey)key]; set
            {
                var tkey = (TKey)key;
                var tvalue = (TValue)value;
                if (ContainsKey(tkey))
                {
                    Remove(tkey);
                }
                Add(tkey, tvalue);
            }
        }
        public TValue this[TKey key]
        {
            get => Content[key]; set
            {
                if (ContainsKey(key))
                {
                    Remove(key);
                }
                Add(key, value);
            }
        }

        private void NotifyCollectionAddItems(List<KeyValuePair<TKey,TValue>> items)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"(+{items.Count})"));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }
        private void NotifyCollectionRemoveItems(List<KeyValuePair<TKey, TValue>> items)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"(-{items.Count})"));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
        }
        private void NotifyCollectionReplace(KeyValuePair<TKey, TValue> newitem, KeyValuePair<TKey, TValue> olditem)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"[{newitem.Key}"));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace
                , new List<KeyValuePair<TKey, TValue>>() { newitem }, new List<KeyValuePair<TKey, TValue>>() { olditem }));
        }
        private void NotifyCollectionReset()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"(clear)"));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        private PropertyChangedEventHandler MakeNestedPropertyChangedHandler(TKey key)
        {
            return (sender, e) =>
            {
                OnPropertyChanged(new NestedPropertyChangedEventArgs($"[{key}]", e));
            };
        }
        protected void ItemInstallNotifyHandler(TKey key, TValue value)
        {
            if (value is INotifyPropertyChanged notify)
            {
                var handler = MakeNestedPropertyChangedHandler(key);
                notify.PropertyChanged += handler;
                HandlerDict.Add(key, handler);
            }
        }
        protected void ItemRemoveNotifyHandler(TKey key, TValue value)
        {
            if (value is INotifyPropertyChanged notify)
            {
                var handler = HandlerDict[key];
                notify.PropertyChanged -= handler;
                HandlerDict.Remove(key);
            }
        }

        protected void OnAddItem(TKey key, TValue value)
        {
            Content.Add(key, value);
            ItemInstallNotifyHandler(key, value);
            NotifyCollectionAddItems(new List<KeyValuePair<TKey, TValue>>() { new KeyValuePair<TKey, TValue>(key, value) });
        }
        protected void OnAddItems(List<KeyValuePair<TKey, TValue>> items)
        {
            items.ForEach(item =>
            {
                Content.Add(item.Key, item.Value);
                ItemInstallNotifyHandler(item.Key, item.Value);
            });
            NotifyCollectionAddItems(items);
        }
        protected bool OnRemoveItem(TKey key)
        {
            if(Content.TryGetValue(key, out var value))
            {
                Content.Remove(key);
                ItemRemoveNotifyHandler(key, value);
                NotifyCollectionRemoveItems(new List<KeyValuePair<TKey, TValue>>() { new KeyValuePair<TKey, TValue>(key, value) });
                return true;
            }
            return false;
        }
        protected void OnResetCollection()
        {
            foreach(var item in Content)
            {
                ItemRemoveNotifyHandler(item.Key, item.Value);
            }
            Content.Clear();
            NotifyCollectionReset();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Content.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Content.GetEnumerator();
        public bool ContainsKey(TKey key) => Content.ContainsKey(key);
        public bool Contains(KeyValuePair<TKey, TValue> item) => Content.ContainsKey(item.Key) && Content[item.Key].Equals(item.Value);
        public void Add(TKey key, TValue value) => OnAddItem(key, value);
        public void Add(KeyValuePair<TKey, TValue> item) => OnAddItem(item.Key, item.Value);
        public bool Remove(TKey key) => OnRemoveItem(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => OnRemoveItem(item.Key);
        public void Clear() => OnResetCollection();
        public bool TryGetValue(TKey key, out TValue value) => Content.TryGetValue(key, out value);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (!(array is KeyValuePair<TKey, TValue>[] kvArray))
                throw new ArgumentException();
            ((ICollection<KeyValuePair<TKey, TValue>>)this).CopyTo(kvArray, arrayIndex);
        }
        

        public virtual XmlSchema GetSchema() => throw new NotImplementedException();
        public virtual string GetXmlData() => throw new NotImplementedException();
        public virtual void ReadXml(XmlReader reader) => throw new NotImplementedException();
        public virtual void SetByXml(string xml) => throw new NotImplementedException();
        public virtual void WriteXml(XmlWriter writer) => throw new NotImplementedException();
        public object GetPropertyDefaultValue(string PropertyName, out ValueDefaultType valueDefaultType) => throw new NotImplementedException();
        public object ResetPropertyDefaultValue(string PropertyName, out ValueDefaultType valueDefaultType) => throw new NotImplementedException();
        public void ResetAllPropertiesDefaultValue(ValueDefaultType filterType = (ValueDefaultType)(-1)) => Clear();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            throw new NotImplementedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }
    }
}
