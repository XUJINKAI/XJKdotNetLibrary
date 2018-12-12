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


        [Conditional("DEBUG")]
        public static void Tag(string tag = "", [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            System.Diagnostics.Debug.WriteLine($"{tag} # {CallerLineNumber,-3} {Path.GetFileName(CallerFilePath)} ({CallerMemberName})");
        }


        public static void WriteLine(LogLevel LogLevel, string Message, bool? WriteCodePosition = null, 
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            if (LogLevel > OutputLevel) return;
            string lel = $"[{LogLevel}]";
            string pos = "";
            if ((WriteCodePosition ?? OutputCodePosition) && CallerLineNumber > 0)
                pos = $"# {CallerLineNumber,-3}, {Path.GetFileName(CallerFilePath)}, {CallerMemberName}";
            Trace.WriteLine($"{lel} {Message}{pos}");
        }


        public static void Error(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            WriteLine(LogLevel.Error, GetMessage(obj), WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
        }

        public static void Warning(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            WriteLine(LogLevel.Warn, GetMessage(obj), WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
        }

        public static void Info(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            WriteLine(LogLevel.Info, GetMessage(obj), WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
        }

        public static void Verbose(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            WriteLine(LogLevel.Verbose, GetMessage(obj), WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
        }

        [Conditional("DEBUG")]
        public static void Debug(object obj, bool? WriteCodePosition = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            WriteLine(LogLevel.Debug, GetMessage(obj), WriteCodePosition, CallerMemberName, CallerFilePath, CallerLineNumber);
        }

        public static string GetMessage(object obj)
        {
            if (obj is null)
            {
                return "<null>";
            }
            else if (obj is Exception ex)
            {
                return ex.GetFullMessage();
            }
            else
            {
                return obj.ToString();
            }
        }
    }

}
