using System;

namespace XJK.FileSystem
{
    public interface IObjectReference<T>
    {
        T GetObject();
        void SetObject(T obj);
        event ObjectChangedHandler ObjectChanged;
    }

    public delegate void ObjectChangedHandler(object sender, ObjectChangedEventArgs e);

    public class ObjectChangedEventArgs : EventArgs
    {
        public object OldObject { get; private set; }

        public ObjectChangedEventArgs(object old)
        {
            OldObject = old;
        }
    }
}
