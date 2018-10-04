using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static XJK.C;

namespace XJK
{
    [DebuggerDisplay("{ToStringViewLf()}")]
    public class LogItem
    {
        private const int LEADINGNAMEFORMAT = -20;

        public string ModuleId { get; set; }
        public DateTime DateTime { get; set; }
        public LogLevel Level { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public int Indent { get; set; }
        public string Position { get; set; }

        public LogItem() { }
        public LogItem(DataRow row)
        {
            ModuleId = (string)row[Log.SMODULEID];
            DateTime = (DateTime)row[Log.SDATETIME];
            Level = (LogLevel)row[Log.SLEVEL];
            Category = (string)row[Log.SCATEGORY];
            Message = (string)row[Log.SMSG];
            Indent = (int)row[Log.SINDENT];
            Position = (string)row[Log.SPOSITION];
        }

        public override string ToString()
        {
            return ToFullString();
        }

        public string ToIndentString()
        {
            return $"{new string(' ', Indent * Log.IndentSize)}{Message}";
        }

        public string ToIndentOrFullString()
        {
            return (Indent == 0) ? ToFullString() : ToIndentString();
        }

        public string ToFullString()
        {
            string lel = $"[{Level}]";
            string cat = Category.IsNullOrEmpty() ? "" : $" '{Category}'";
            string pos = (Log.LogLocation && !Position.IsNullOrEmpty()) ? $" ({Position})" : "";
            return $"[{ModuleId}][{DateTime.ToString(DateTimeFormat)}]{lel,9} {Message}{cat}{pos}";
        }
        
        public string ToStringViewLf()
        {
            return $"{Log.ModuleId,LEADINGNAMEFORMAT} = {ModuleId}{LF}" +
                $"{Log.SDATETIME,LEADINGNAMEFORMAT} = {DateTime.ToString(DateTimeFormat)}{LF}" +
                $"{Log.SLEVEL,LEADINGNAMEFORMAT} = {Level}{LF}" +
                $"{Log.SCATEGORY,LEADINGNAMEFORMAT} = {Category}{LF}" +
                $"{Log.SMSG,LEADINGNAMEFORMAT} = {Message}{LF}" +
                $"{Log.SINDENT,LEADINGNAMEFORMAT} = {Indent}{LF}" +
                $"{Log.SPOSITION,LEADINGNAMEFORMAT} = {Position}{LF}";
        }
    }

}
