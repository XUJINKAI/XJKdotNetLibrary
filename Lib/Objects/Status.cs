using System;
using System.ComponentModel;

namespace XJK
{
    public class Status : INotifyPropertyChanged
    {
        public bool DefaultState { get; private set; }
        public bool State => Counter == 0 ? DefaultState : !DefaultState;
        public bool IsChanged => State != DefaultState;
        public event Action<bool> StatusChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private int Counter = 0;

        public Status(bool defaultState)
        {
            DefaultState = defaultState;
        }

        public void InChanging(Action action)
        {
            Counter++;
            if (Counter == 1)
            {
                StatusChanged?.Invoke(false);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            }
            action?.Invoke();
            Counter--;
            if (Counter == 0)
            {
                StatusChanged?.Invoke(true);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            }
        }

        public static bool operator true(Status val) => val.State;
        public static bool operator false(Status val) => !val.State;
        public static bool operator !(Status val) => !val.State;
        public static implicit operator bool(Status status) => status.State;
    }
}
