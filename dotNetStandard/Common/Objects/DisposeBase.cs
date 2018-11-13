using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.Objects
{
    public abstract class DisposeBase : IDisposable
    {
        protected abstract void OnDispose();
        public bool IsDisposed { get; private set; }
        
        public void Dispose()
        {
            if (!IsDisposed)
            {
                GC.SuppressFinalize(this);
                IsDisposed = true;
                OnDispose();
            }
        }
        
        ~DisposeBase()
        {
            Dispose();
        }
    }
}
