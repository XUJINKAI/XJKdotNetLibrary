using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace XJKdotNetLibrary
{
    /// <summary>
    /// provide ItemsPropertyChanged event to notify item property change
    /// </summary>
    /// <typeparam name="T">must be INotifyPropertyChanged</typeparam>
    [Serializable]
    public class ObservableCollectionX<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public delegate void ItemsPropertyChangedEventHandler(object sender, object item, string PropertyName);
        public event ItemsPropertyChangedEventHandler ItemsPropertyChanged;

        public ObservableCollectionX()
        {
            CollectionChanged += ObservableCollectionX_CollectionChanged;
        }

        private void ObservableCollectionX_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += Item_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= Item_PropertyChanged;
                }
            }
        }
        
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemsPropertyChanged?.Invoke(this, sender, e.PropertyName);
        }
    }
}
