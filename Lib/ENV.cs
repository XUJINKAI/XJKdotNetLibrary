using System;

namespace XJK
{
    public static class ENV
    {

#if DEBUG
        private const bool _debug = true;
#else
        private const bool _debug = false;
#endif

        public static string? EntryLocation => System.Reflection.Assembly.GetEntryAssembly()?.Location;

        public static string? BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public static bool DEBUG => _debug;

        public static bool RELEASE => !_debug;

    }
}
