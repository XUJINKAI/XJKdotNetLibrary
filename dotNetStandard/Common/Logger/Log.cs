using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace XJK.Logger
{
    public static class Log
    {
        public static LogLevel OutputLevel { get; set; } = LogLevel.Info;
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
            System.Diagnostics.Debug.WriteLine($"{tag} # {CallerLineNumber,-3} {Path.GetFileName(CallerFilePath)} ({CallerMemberName})");
        }
        

        public static void WriteLine(LogLevel LogLevel, string Message)
        {
            if (LogLevel > OutputLevel) return;
            string lel = $"[{LogLevel}]";
            Trace.WriteLine($"{lel} {Message}");
        }

        public static string GetPositionString(bool? WriteCodePosition, string CallerMemberName, string CallerFilePath, int CallerLineNumber)
        {
            if ((WriteCodePosition ?? OutputCodePosition) && CallerLineNumber > 0)
                return $" # {CallerLineNumber,-3}, {Path.GetFileName(CallerFilePath)}, {CallerMemberName}";
            return "";
        }
        

        public static void Error(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            Trace.TraceError(LogHelper.GetMessage(obj) + pos);
        }

        public static void Warning(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            Trace.TraceWarning(LogHelper.GetMessage(obj) + pos);
        }

        public static void Info(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            Trace.TraceInformation(LogHelper.GetMessage(obj) + pos);
        }

        public static void Verbose(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            WriteLine(LogLevel.Verbose, LogHelper.GetMessage(obj) + pos);
        }

        [Conditional("DEBUG")]
        public static void Debug(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string pos = GetPositionString(WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
            WriteLine(LogLevel.Debug, LogHelper.GetMessage(obj) + pos);
        }

    }

}
