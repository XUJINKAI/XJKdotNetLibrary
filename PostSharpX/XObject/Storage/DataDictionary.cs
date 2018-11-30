using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.NotifyPropertyChanged;

namespace XJK.Storage
{
    public class DataDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyPropertyChangedEx, IXmlSerializable
    {
        public List<ParseError> ParseErrors = new List<ParseError>();
        public event PropertyChangedEventHandlerEx PropertyChangedEx;

        private readonly Dictionary<TKey, TValue> Content = new Dictionary<TKey, TValue>();
        private readonly string _KeyValuePairs_ = "KeyValuePairs";

        protected void OnPropertyChangedEx(PropertyChangedEventArgsEx e)
        {
            PropertyChangedEx?.Invoke(this, e);
        }
        protected void OnCollectionChangedEx(List<KeyValuePair<TKey, TValue>> newItems, List<KeyValuePair<TKey, TValue>> oldItems)
        {
            OnPropertyChangedEx(PropertyChangedEventArgsEx.NewCollectionChange(_KeyValuePairs_, newItems, oldItems));
        }

        protected void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChangedEx(PropertyChangedEventArgsEx.NewCollectionItemPropertyChange(_KeyValuePairs_, sender, e.PropertyName));
        }

        #region Override

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

        public ICollection<TKey> Keys => Content.Keys;
        public ICollection<TValue> Values => Content.Values;
        public int Count => Content.Count;
        public bool IsReadOnly => false;
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Content.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Content.GetEnumerator();
        public bool ContainsKey(TKey key) => Content.ContainsKey(key);
        public bool Contains(KeyValuePair<TKey, TValue> item) => Content.ContainsKey(item.Key) && Content[item.Key].Equals(item.Value);

        //for reflection GetMethod
        public void AddKeyValue(TKey key, TValue value) => Add(key, value);

        public void Add(TKey key, TValue value)
        {
            Content.Add(key, value);
            if(value is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += Value_PropertyChanged;
            }
            OnCollectionChangedEx(new List<KeyValuePair<TKey, TValue>>() { new KeyValuePair<TKey, TValue>(key, value) }, null);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Content.Add(item.Key, item.Value);
            if (item.Value is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += Value_PropertyChanged;
            }
            OnCollectionChangedEx(new List<KeyValuePair<TKey, TValue>>() { item }, null);
        }

        public bool Remove(TKey key)
        {
            if (!ContainsKey(key)) return false;
            var value = this[key];
            if (value is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged -= Value_PropertyChanged;
            }
            Content.Remove(key);
            OnCollectionChangedEx(null, new List<KeyValuePair<TKey, TValue>>() { new KeyValuePair<TKey, TValue>(key, value) });
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!ContainsKey(item.Key)) return false;
            if (!this[item.Key].Equals(item.Value)) return false;
            var value = this[item.Key];
            if (value is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged -= Value_PropertyChanged;
            }
            Content.Remove(item.Key);
            OnCollectionChangedEx(null, new List<KeyValuePair<TKey, TValue>>() { item });
            return true;
        }

        public void Clear()
        {
            var oldItems = new List<KeyValuePair<TKey, TValue>>();
            foreach(var item in this)
            {
                oldItems.Add(item);
            }
            Content.Clear();
            OnCollectionChangedEx(null, oldItems);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (this.ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (!(array is KeyValuePair<TKey, TValue>[] kvArray))
                throw new ArgumentException();
            ((ICollection<KeyValuePair<TKey, TValue>>)this).CopyTo(kvArray, arrayIndex);
        }

        #endregion

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            SerializationHelper.DictionaryAddRange(this, reader, ref ParseErrors);
        }

        public void WriteXml(XmlWriter writer)
        {
            SerializationHelper.WriteXmlRecursive(writer, this);
        }
    }
}
