using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace XJK.NotifyPropertyChanged
{
    /// <summary>
    /// provide ItemsPropertyChanged event to notify item property change
    /// </summary>
    /// <typeparam name="T">must be INotifyPropertyChanged</typeparam>
    [Serializable]
    public class ObservableCollectionEx<T> : ObservableCollection<T>, INotifyPropertyChangedEx
        where T : INotifyPropertyChanged
    {
        public bool PropagationNotificationEx { get; set; } = true;
        public event PropertyChangedEventHandlerEx PropertyChangedEx;

        private readonly string DataPropertyName = "Items";

        protected void OnPropertyChangedEx(PropertyChangedEventArgsEx e)
        {
            PropertyChangedEx?.Invoke(this, e);
        }

        public ObservableCollectionEx()
        {
            PropertyChanged += ObservableCollectionX_PropertyChanged;
            CollectionChanged += ObservableCollectionX_CollectionChanged;
        }

        private void ObservableCollectionX_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChangedEx(PropertyChangedEventArgsEx.NewPropertyChange(e.PropertyName));
        }

        protected void ObservableCollectionX_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += CollectionItem_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= CollectionItem_PropertyChanged;
                }
            }
            OnPropertyChangedEx(PropertyChangedEventArgsEx.NewCollectionChange(DataPropertyName, e.NewItems, e.OldItems));
            if (PropagationNotificationEx) OnPropertyChanged(new PropertyChangedEventArgs(DataPropertyName));
        }

        protected void CollectionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChangedEx(PropertyChangedEventArgsEx.NewCollectionItemPropertyChange(DataPropertyName, sender, e.PropertyName));
            if (PropagationNotificationEx) OnPropertyChanged(new PropertyChangedEventArgs(DataPropertyName));
        }
    }
}
