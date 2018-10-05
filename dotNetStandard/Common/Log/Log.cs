using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static XJK.C;

namespace XJK
{
    public static class Log
    {
        public static event Action<string> TextListener;
        public static event Action<IEnumerable<LogItem>> LogItemListener;
        
        public static string ModuleId { get; set; } = "";
        public static bool LogLocation { get; set; } = false;
        public static int IndentLevel { get; set; } = 0;
        public static int IndentSize { get; set; } = 4;
        public static int MaxRowsLimit
        {
            get => s_maxRowsLimit;
            set
            {
                s_maxRowsLimit = value;
                RemoveToLimitRows();
            }
        }
        public static bool AutoFlush
        {
            get => s_autoFlush; set
            {
                s_autoFlush = value;
                if (value) Flush();
            }
        }
        private static readonly Status RaiseRowChanged = new Status(true);
        private static readonly StringBuilder LineCache = new StringBuilder();
        private static readonly DataTable DB;
        public static int CountAll => DB.Rows.Count;
        public static int CountNew => DB.GetChanges().Rows.Count;

        private static bool s_autoFlush = true;
        private static int s_maxRowsLimit = 100;
        private static LogTraceListener LogTraceListener;

        public const string SMODULEID = "ModuleId";
        public const string SDATETIME = "DateTime";
        public const string SLEVEL = "Level";
        public const string SCATEGORY = "Category";
        public const string SMSG = "Message";
        public const string SINDENT = "Indent";
        public const string SPOSITION = "CallerPosition";

        public static void ListenSystemDiagnosticsLog()
        {
            if (LogTraceListener == null)
            {
                LogTraceListener = new LogTraceListener();
            }
            System.Diagnostics.Trace.Listeners.Add(LogTraceListener);
        }

        static Log()
        {
            ModuleId = "#" + Helper.RandomString(7);
            DB = new DataTable();
            DB.Columns.AddRange(new DataColumn[]
            {
                new DataColumn(SMODULEID),
                new DataColumn(SDATETIME, typeof(DateTime)),
                new DataColumn(SLEVEL, typeof(LogLevel)),
                new DataColumn(SCATEGORY),
                new DataColumn(SMSG),
                new DataColumn(SINDENT, typeof(int)),
                new DataColumn(SPOSITION),
            });
            DB.RowChanged += DB_RowChanged;
        }

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
                if (TextListener != null)
                {
                    string str = "";
                    foreach (DataRow row in db.Rows)
                    {
                        var item = new LogItem(row);
                        str += $"{item.ToIndentOrFullString()}{LF}";
                    }
                    TextListener.Invoke(str);
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
                return AllCaches().Aggregate("", (sum, next) => $"{sum}{next}{LF}");
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

        public static void RemoveToLimitRows()
        {
            while (DB.Rows.Count > MaxRowsLimit)
            {
                DB.Rows.RemoveAt(0);
            }
        }

        public static void Write(string message)
        {
            LineCache.Append(message);
        }
        

        public static string Tag([CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            if (CallerLineNumber <= 0) return "";
            return $"# {CallerLineNumber,-3}, {Path.GetFileName(CallerFilePath)}, {CallerMemberName}";
        }
        [Conditional("DEBUG")]
        public static void TagDebugger([CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = -1)
        {
            string s = $"{CallerLineNumber,-5}{Path.GetFileName(CallerFilePath),-30}{CallerMemberName}";
            System.Diagnostics.Debug.WriteLine($"### {s}");
        }

        private static void AddRow(LogLevel level, string message, string category, string CallerMemberName, string CallerFilePath, int CallerLineNumber)
        {
            var row = DB.NewRow();
            row[SMODULEID] = ModuleId;
            row[SDATETIME] = DateTime.Now.ToString(DateTimeFormat);
            row[SLEVEL] = level;
            row[SCATEGORY] = category ?? "";
            row[SMSG] = LineCache.ToString() + message;
            row[SINDENT] = IndentLevel;
            row[SPOSITION] = Tag(CallerMemberName, CallerFilePath, CallerLineNumber);
            DB.Rows.Add(row);
            LineCache.Clear();
            RemoveToLimitRows();
        }

        public static void Fatal(object obj, string category = "",
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            AddRow(LogLevel.Fatal, GetMessage(obj), category, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
        public static void Error(object obj, string category = "",
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            AddRow(LogLevel.Error, GetMessage(obj), category, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
        public static void Warning(object obj, string category = "",
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            AddRow(LogLevel.Warn, GetMessage(obj), category, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
        public static void Info(object obj, string category = "",
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            AddRow(LogLevel.Info, GetMessage(obj), category, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
        public static void Verbose(object obj, string category = "",
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            AddRow(LogLevel.Verbose, GetMessage(obj), category, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
        [Conditional("DEBUG")]
        public static void Debug(object obj, string category = "",
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            AddRow(LogLevel.Verbose, GetMessage(obj), category, CallerMemberName, CallerFilePath, CallerLineNumber);
        }
        [Conditional("TRACE")]
        public static void Trace(object obj, string category = "",
            [CallerMemberName] string CallerMemberName = "", [CallerFilePath] string CallerFilePath = "", [CallerLineNumber] int CallerLineNumber = 0)
        {
            AddRow(LogLevel.Verbose, GetMessage(obj), category, CallerMemberName, CallerFilePath, CallerLineNumber);
        }

        private static string GetMessage(object obj)
        {
            if (obj is null)
            {
                return "";
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
