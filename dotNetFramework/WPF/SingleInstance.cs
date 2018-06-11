using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XJK.WPF
{
    public static class SingleInstance
    {
        private static Mutex mutex;
        private static bool isNewInstance;

        public static bool IsNewInstance(string AppId)
        {
            if (mutex == null)
            {
                mutex = new Mutex(true, AppId, out isNewInstance);
                return isNewInstance;
            }
            else
            {
                return isNewInstance;
            }
        }
    }
}
