using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace XJKdotNetLibrary
{
    public enum LogLevel
    {
        Info,
        Verbose,
        Error,
        Debug,
        Trace,
    }

    public class LogItem
    {
        private const int LEADINGNAMEFORMAT = -20;

        public string Prefix { get; set; }
        public DateTime DateTime { get; set; }
        public LogLevel Level { get; set; }
        public int Indent { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string Trace { get; set; }

        public LogItem() { }
        public LogItem(DataRow row)
        {
            Prefix = (string)row["Prefix"];
            DateTime = (DateTime)row["DateTime"];
            Level = (LogLevel)row["Level"];
            Indent = (int)row["Indent"];
            Message = (string)row["Message"];
            Source = (string)row["Source"];
            Trace = (string)row["Trace"];
        }

        public override string ToString()
        {
            return this.ToString(true);
        }

        public string ToString(bool MetaInfo = true)
        {
            string pre = string.IsNullOrEmpty(Prefix) ? "" : Prefix;
            return MetaInfo ? $"{pre}[{DateTime.ToString(C.DateTimeFormat)}][{Level}] {Message}" : $"{new String(' ', Indent * Log.IndentSize)}{Message}";
        }

        public string ToLongStringLf()
        {
            return $"{0,LEADINGNAMEFORMAT:Prefix}{Prefix}{C.LF}" +
                $"{0,LEADINGNAMEFORMAT:DateTime}{DateTime.ToString(C.DateTimeFormat)}{C.LF}" +
                $"{0,LEADINGNAMEFORMAT:Level}{Level}{C.LF}" +
                $"{0,LEADINGNAMEFORMAT:Message}{Message}{C.LF}" +
                $"{0,LEADINGNAMEFORMAT:Indent}{Indent}{C.LF}" +
                $"{0,LEADINGNAMEFORMAT:Source}{Source}{C.LF}" +
                $"{0,LEADINGNAMEFORMAT:Trace}{Trace}{C.LF}";
        }
    }

    public static class Log
    {
        public static event Action<string> AutoStringListener;
        public static event Action<IEnumerable<LogItem>> LogItemListener;

        public static string Prefix { get; set; } = "";
        public static int MaxRowsLimit { get; set; } = 100;
        public static int IndentLevel { get; set; } = 0;
        public static int IndentSize { get; set; } = 4;
        private static readonly DataTable DB;

        public static int CountAll => DB.Rows.Count;
        public static int CountNew => DB.GetChanges().Rows.Count;

        public static bool DebugConsoleOutput { get; set; } = false;

        private static bool s_autoFlush = false;

        public static bool AutoFlush
        {
            get => s_autoFlush; set
            {
                s_autoFlush = value;
                if (value) Flush();
            }
        }
        
        static Log()
        {
            DB = new DataTable();
            DB.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Prefix"),
                new DataColumn("DateTime", typeof(DateTime)),
                new DataColumn("Source"),
                new DataColumn("Trace"),
                new DataColumn("Level", typeof(LogLevel)),
                new DataColumn("Indent", typeof(int)),
                new DataColumn("Message"),
            });
            DB.RowChanged += DB_RowChanged;
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.Add(new LogTraceListener());
            System.Diagnostics.Trace.AutoFlush = true;
        }
        

        private static readonly Status RaiseRowChanged = new Status(true);

        private static void DB_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (RaiseRowChanged)
            {
                if (AutoFlush)
                {
                    Flush();
                }
            }
        }

        public static void Flush()
        {
            var db = DB.GetChanges(DataRowState.Added);
            // !null or >0
            if (db?.Rows.Count > 0)
            {
                if (AutoStringListener != null)
                {
                    string str = "";
                    foreach (DataRow row in db.Rows)
                    {
                        var item = new LogItem(row);
                        str += $"{item.ToString(item.Indent == 0)}{C.LF}";
                    }
                    AutoStringListener.Invoke(str);
                }
                if (LogItemListener != null)
                {
                    List<LogItem> list = new List<LogItem>();
                    foreach (DataRow row in db.Rows)
                    {
                        list.Add(new LogItem(row));
                    }
                    LogItemListener.Invoke(list);
                }
                RaiseRowChanged.InChanging(() =>
                {
                    DB.AcceptChanges();
                });
            }
        }

        public static IEnumerable<LogItem> AllCaches()
        {
            List<LogItem> list = new List<LogItem>();
            foreach (DataRow row in DB.Rows)
            {
                list.Add(new LogItem(row));
            }
            return list;
        }

        public static string AllCachesText
        {
            get
            {
                return AllCaches().Aggregate("", (sum, next) => $"{sum}{next}{C.LF}");
            }
        }

        public static void Indent()
        {
            IndentLevel++;
        }

        public static void Unindent()
        {
            IndentLevel = Math.Max(0, IndentLevel - 1);
        }

        public static void ResetIndent()
        {
            IndentLevel = 0;
        }


        private static void Add(object obj, LogLevel level, string source = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            var row = DB.NewRow();
            row["Prefix"] = Prefix;
            row["DateTime"] = DateTime.Now.ToString(C.DateTimeFormat);
            row["Source"] = source ?? CallerMemberName;
            row["Trace"] = $"{CallerLineNumber} in {CallerFilePath}, line {CallerLineNumber}";
            row["Level"] = level;
            row["Indent"] = IndentLevel;
            row["Message"] = obj.ToString();
            DB.Rows.Add(row);
            while (DB.Rows.Count > MaxRowsLimit)
            {
                DB.Rows.RemoveAt(0);
            }
            DebugWriteToConsole(row);
        }

        [Conditional("DEBUG")]
        private static void DebugWriteToConsole(DataRow row)
        {
            if (DebugConsoleOutput)
            {
                System.Diagnostics.Debug.Write(new LogItem(row).ToString() + C.LF);
            }
        }

        public static void Info(object obj, string source = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            Add(obj, LogLevel.Info, source, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
        public static void Verbose(object obj, string source = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            Add(obj, LogLevel.Verbose, source, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
        public static void Error(object obj, Exception ex = null, string source = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            Add((obj == null ? "" : obj.ToString() + C.LF) + (ex == null ? "" : (ex.ToStringLong())),
                LogLevel.Error, source, CallerMemberName, CallerFilePath, CallerLineNumber);
        }

        [Conditional("DEBUG")]
        public static void Debug(object obj, string source = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            Add(obj, LogLevel.Debug, source, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
        [Conditional("TRACE")]
        public static void Trace(object obj, string source = null,
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            Add(obj, LogLevel.Trace, source, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
    }

    public class LogTraceListener : TraceListener
    {
        public static StringBuilder TraceCache = new StringBuilder();

        public override void Write(string message)
        {
            TraceCache.Append(message);
        }

        public override void WriteLine(string message)
        {
            TraceCache.Append(message);
            Log.Trace(TraceCache.ToString(), "SystemTrace");
            TraceCache.Clear();
        }
    }
}
