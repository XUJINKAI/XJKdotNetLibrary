using PostSharp.Patterns.Collections;
using PostSharp.Patterns.Collections.Advices;
using PostSharp.Patterns.DynamicAdvising;
using PostSharp.Patterns.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.NotifyProperty;
using XJK.XSerializers;

namespace XJK.XStorage
{
    /// <summary>
    /// Aggregatable, Observable, Serializable
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    [XmlDataDictionary]
    public class DataDictionary<TKey, TValue> : NotifyXmlObject, IQueryInterface, IAggregatable, IAttachable
        , IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        public event EventHandler ParentChanged;
        public event EventHandler<AncestorChangedEventArgs> AncestorChanged;
        public RelationshipKind ParentRelationship => RelationshipKind.ParentSurrogate;
        [XmlIgnore]
        [IgnoreAutoChangeNotification]
        public object Parent
        {
            get => _parent;
            set
            {
                if (_parent == value) return;
                if (_parent is IAggregatable oldaggregatable)
                {
                    oldaggregatable.ParentChanged -= Aggregatable_ParentChanged;
                    oldaggregatable.AncestorChanged -= Aggregatable_AncestorChanged;
                }
                _parent = value;
                if (_parent is IAggregatable aggregatable)
                {
                    aggregatable.ParentChanged += Aggregatable_ParentChanged;
                    aggregatable.AncestorChanged += Aggregatable_AncestorChanged;
                }
                ParentChanged?.Invoke(this, new EventArgs());
            }
        }

        private object _parent;
        private readonly AdvisableDictionary<TKey, TValue> Content = new AdvisableDictionary<TKey, TValue>();
        private readonly Dictionary<TKey, PropertyChangedEventHandler> HandlerDict = new Dictionary<TKey, PropertyChangedEventHandler>();

        private void Aggregatable_ParentChanged(object sender, EventArgs e)
        {
            AncestorChanged?.Invoke(this, new AncestorChangedEventArgs((_parent as IAggregatable).Parent));
        }

        private void Aggregatable_AncestorChanged(object sender, AncestorChangedEventArgs e)
        {
            AncestorChanged?.Invoke(this, e);
        }

        private PropertyChangedEventHandler MakeNestedPropertyChangedHandler(TKey key)
        {
            return (sender, e) =>
            {
                OnPropertyChanged(new NestedPropertyChangedEventArgs($"[{key}]", e));
            };
        }
        private void OnCollectionChanged(List<TKey> newKeys, List<TKey> oldKeys)
        {
            OnPropertyChanged(new PropertyChangedEventArgs($"(+{newKeys?.Count ?? 0},-{oldKeys?.Count ?? 0})"));
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

        [SafeForDependencyAnalysis] public ICollection<TKey> Keys => Content.Keys;
        [SafeForDependencyAnalysis] public ICollection<TValue> Values => Content.Values;
        [SafeForDependencyAnalysis] public int Count => Content.Count;
        public bool IsReadOnly => false;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Content.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Content.GetEnumerator();
        public bool ContainsKey(TKey key) => Content.ContainsKey(key);
        public bool Contains(KeyValuePair<TKey, TValue> item) => Content.ContainsKey(item.Key) && Content[item.Key].Equals(item.Value);

        public void Add(TKey key, TValue value)
        {
            Content.Add(key, value);
            if (value is INotifyPropertyChanged notifyPropertyChanged)
            {
                var handler = MakeNestedPropertyChangedHandler(key);
                notifyPropertyChanged.PropertyChanged += handler;
                HandlerDict.Add(key, handler);
            }
            OnCollectionChanged(new List<TKey>() { key }, null);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Content.Add(item.Key, item.Value);
            if (item.Value is INotifyPropertyChanged notifyPropertyChanged)
            {
                var handler = MakeNestedPropertyChangedHandler(item.Key);
                notifyPropertyChanged.PropertyChanged += handler;
                HandlerDict.Add(item.Key, handler);
            }
            OnCollectionChanged(new List<TKey>() { item.Key }, null);
        }

        public bool Remove(TKey key)
        {
            if (!ContainsKey(key)) return false;
            var value = this[key];
            if (value is INotifyPropertyChanged notifyPropertyChanged)
            {
                var handler = HandlerDict[key];
                notifyPropertyChanged.PropertyChanged -= handler;
                HandlerDict.Remove(key);
            }
            Content.Remove(key);
            OnCollectionChanged(null, new List<TKey>() { key });
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!ContainsKey(item.Key)) return false;
            if (!this[item.Key].Equals(item.Value)) return false;
            var value = this[item.Key];
            if (value is INotifyPropertyChanged notifyPropertyChanged)
            {
                var handler = HandlerDict[item.Key];
                notifyPropertyChanged.PropertyChanged -= handler;
                HandlerDict.Remove(item.Key);
            }
            Content.Remove(item.Key);
            OnCollectionChanged(null, new List<TKey>() { item.Key });
            return true;
        }

        public void Clear()
        {
            var keys = Content.Keys.ToList();
            Content.Clear();
            HandlerDict.Clear();
            OnCollectionChanged(null, keys);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (this.ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            value = default;
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

        public T QueryInterface<T>() where T : class
        {
            var t = this as T;
            return t;
        }

        public bool AttachToParent(RelationshipInfo relationshipInfo, IAttacher attacher, object attacherState)
        {
            Parent = (attacherState as AggregatableAttribute)?.Instance;
            foreach (var item in Content)
            {
                attacher.AttachChild(attacherState, item, new ChildInfo("Item", item.GetType(), relationshipInfo));
            }
            return true;
        }

        public bool DetachFromParent()
        {
            Parent = null;
            return true;
        }

        public void OnDeserialization(object caller)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitChildren(ChildVisitor visitor, ChildVisitorOptions options = ChildVisitorOptions.None, object state = null)
        {
            foreach (var item in Content)
            {
                visitor(item, new ChildInfo("Item", item.GetType(), new RelationshipInfo(RelationshipKind.ChildOrParentSurrogate)), state);
            }
            return true;
        }
    }
}
