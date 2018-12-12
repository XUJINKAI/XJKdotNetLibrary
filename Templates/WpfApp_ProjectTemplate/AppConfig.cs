using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace $safeprojectname$
{
    public static class AppConfig
    {
        public static string LF => Environment.NewLine;
        public static string EntryLocation => System.Reflection.Assembly.GetEntryAssembly().Location;
        public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

        // Project Enviroment

        private static readonly bool _isDesignTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
        public static bool IsDesignTime => _isDesignTime;

#if DEBUG
        private const bool _debug = true;
#else
        private const bool _debug = false;
#endif
        public static bool DEBUG => _debug;
        public static bool RELEASE => !_debug;
    }
}
