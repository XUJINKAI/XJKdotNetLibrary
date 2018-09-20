using System;
using System.Collections;

namespace XJK.NotifyPropertyChanged
{
    public enum PropertyChangeType
    {
        NoSet = 0,
        Property, // o.Property
        ItemProperty, // o.Object.Property
        Collection, // o.List[i].Add/Remove
        CollectionItemsProperty, // o.List[i].Property
    }

    public class PropertyChangedEventArgsEx : EventArgs
    {
        private PropertyChangedEventArgsEx() { }

        public PropertyChangeType Type { get; private set; }
        public string PropertyName { get; private set; }

        public object Item { get; private set; }
        public string ItemPropertyName { get; private set; }

        public IList CollectionNewItems { get; private set; }
        public IList CollectionOldItems { get; private set; }

        public object TryGetItemPropertyValue()
        {
            return Item?.GetType().GetProperty(ItemPropertyName)?.GetValue(Item);
        }

        public override string ToString()
        {
            switch (Type)
            {
                case PropertyChangeType.Property:
                    return $"{PropertyName}";
                case PropertyChangeType.ItemProperty:
                    return $"{PropertyName}.{ItemPropertyName}";
                case PropertyChangeType.Collection:
                    return $"{PropertyName} New:{CollectionNewItems?.Count ?? 0}, Old:{CollectionOldItems?.Count ?? 0}";
                case PropertyChangeType.CollectionItemsProperty:
                    return $"{PropertyName}.{ItemPropertyName}";
                default:
                    return "Unknown PropertyChangedEventArgsEx Type";
            }
        }
        
        public static PropertyChangedEventArgsEx NewPropertyChange(string propertyName)
        {
            return new PropertyChangedEventArgsEx()
            {
                Type = PropertyChangeType.Property,
                PropertyName = propertyName,
            };
        }
        
        public static PropertyChangedEventArgsEx NewItemPropertyChange(string propertyName, object item, string itemPropertyName)
        {
            return new PropertyChangedEventArgsEx()
            {
                Type = PropertyChangeType.ItemProperty,
                PropertyName = propertyName,
                ItemPropertyName = itemPropertyName,
                Item = item,
            };
        }

        public static PropertyChangedEventArgsEx NewCollectionItemPropertyChange(string propertyName, object item, string itemPropertyName)
        {
            return new PropertyChangedEventArgsEx()
            {
                Type = PropertyChangeType.CollectionItemsProperty,
                PropertyName = propertyName,
                ItemPropertyName = itemPropertyName,
                Item = item,
            };
        }

        public static PropertyChangedEventArgsEx NewCollectionChange(string propertyName, IList newItems, IList oldItems)
        {
            return new PropertyChangedEventArgsEx()
            {
                Type = PropertyChangeType.Collection,
                PropertyName = propertyName,
                CollectionNewItems = newItems,
                CollectionOldItems = oldItems,
            };
        }
    }
}
