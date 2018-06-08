using System;

namespace XJKdotNetLibrary
{
    public class Status
    {
        public bool DefaultState { get; private set; }
        public bool State => Counter == 0 ? DefaultState : !DefaultState;
        public bool IsChanged => State != DefaultState;
        public event Action<bool> StatusChanged;

        private int Counter = 0;

        public Status(bool defaultState)
        {
            DefaultState = defaultState;
        }

        public void InChanging(Action action)
        {
            Counter++;
            if (Counter == 1) StatusChanged?.Invoke(false);
            action?.Invoke();
            Counter--;
            if (Counter == 0) StatusChanged?.Invoke(true);
        }

        public static bool operator true(Status val) => val.State;
        public static bool operator false(Status val) => !val.State;
        public static bool operator !(Status val) => !val.State;
    }
}
