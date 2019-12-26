using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace XJK.Logger
{
    public static class Log
    {
        private static LogLevel s_outputLevel = LogLevel.Info;

        public static LogLevel OutputLevel
        {
            get => s_outputLevel; set
            {
                if (s_outputLevel != value)
                    Trace.WriteLine($"--- Log Level : {value} ---");
                s_outputLevel = value;
            }
        }

        public static bool OutputCodePosition { get; set; } = false;

        /// <summary>
        /// DEBUG only
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="CallerMemberName"></param>
        /// <param name="CallerFilePath"></param>
        /// <param name="CallerLineNumber"></param>
        [Conditional("DEBUG")]
        public static void Tag(string tag = "", [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            Debug.WriteLine($"{tag} # {CallerLineNumber,-3} {Path.GetFileName(CallerFilePath)} ({CallerMemberName})");
        }


        public static void WriteLine(LogLevel LogLevel, string Message)
        {
            if (LogLevel > OutputLevel) return;
            Trace.WriteLine(LogHelper.ImplementMessage(LogLevel, Message));
        }

        public static string GetPositionString(bool? WriteCodePosition, string CallerMemberName, string CallerFilePath, int CallerLineNumber)
        {
            if ((WriteCodePosition ?? OutputCodePosition) && CallerLineNumber > 0)
                return $"  #!{CallerMemberName}, L{CallerLineNumber}, {Path.GetFileName(CallerFilePath)}";
            return "";
        }


        public static void Error(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            WriteLine(LogLevel.Error, LogHelper.GetMessage(obj) + pos);
        }

        public static void Warning(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            WriteLine(LogLevel.Warn, LogHelper.GetMessage(obj) + pos);
        }

        public static void Info(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            WriteLine(LogLevel.Info, LogHelper.GetMessage(obj) + pos);
        }

        public static void Verbose(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            WriteLine(LogLevel.Verbose, LogHelper.GetMessage(obj) + pos);
        }

        /// <summary>
        /// #if DEBUG
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="WriteCodePosition"></param>
        /// <param name="CallerMemberName"></param>
        /// <param name="CallerFilePath"></param>
        /// <param name="CallerLineNumber"></param>
        [Conditional("DEBUG")]
        public static void DEBUG(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            WriteLine(LogLevel.DEBUG, LogHelper.GetMessage(obj) + pos);
        }

    }

}
