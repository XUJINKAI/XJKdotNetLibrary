using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XJK.Logger
{
    public class Listener : TraceListener
    {
        public string Id { get; set; }
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public bool Registered
        {
            get => Trace.Listeners.Contains(this);
            set
            {
                if (value && !Trace.Listeners.Contains(this))
                {
                    Trace.Listeners.Add(this);
                }
                else if (!value && Trace.Listeners.Contains(this))
                {
                    Trace.Listeners.Remove(this);
                }
            }
        }

        private readonly Action<string> _writer;

        public Listener(Action<string> Writer)
        {
            _writer = Writer ?? throw new ArgumentNullException("Writer cannot be null.");
        }
        
        private void Add(string content)
        {
            _writer(content);
        }

        private void AddWithMeta(string content)
        {
            var id = string.IsNullOrEmpty(Id) ? "" : $"[{Id}]";
            var lines = $"{id}[{DateTime.Now.ToString(DateTimeFormat)}]{content}";
            if (Trace.IndentLevel > 0)
            {
                string prefix = ('-' + ' '.Dup(Trace.IndentSize - 1)).Dup(Trace.IndentLevel);
                string indent = ' '.Dup(Trace.IndentLevel * Trace.IndentSize);
                if (prefix.Length != indent.Length) Debugger.Break();
                lines = prefix + lines.Replace("\n", "\n" + indent);
            }
            _writer(lines + Environment.NewLine);
        }

        #region Write

        public override void Write(object o)
        {
            Add(Log.GetMessage(o));
        }

        public override void Write(object o, string category)
        {
            Add($"{category}: {Log.GetMessage(o)}");
        }

        public override void Write(string message)
        {
            Add(message);
        }

        public override void Write(string message, string category)
        {
            Add($"{category}: {message}");
        }

        #endregion

        #region WriteLine

        public override void WriteLine(object o)
        {
            AddWithMeta(Log.GetMessage(o));
        }

        public override void WriteLine(object o, string category)
        {
            AddWithMeta($"{category}: {Log.GetMessage(o)}");
        }

        public override void WriteLine(string message)
        {
            AddWithMeta(message);
        }

        public override void WriteLine(string message, string category)
        {
            AddWithMeta($"{category}: {message}");
        }

        #endregion

        #region Fail

        public override void Fail(string message)
        {
            AddWithMeta(message);
        }

        public override void Fail(string message, string detailMessage)
        {
            AddWithMeta($"{message}{Environment.NewLine}{detailMessage}");
        }

        #endregion

        #region TraceEvent

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            throw new NotImplementedException();
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            string arg = args.JoinToString(o => o.ToString(), ", ");
            TraceEvent(eventCache, source, eventType, id, $"Args: {arg}{Environment.NewLine}{format}");
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            switch (eventType)
            {
                case TraceEventType.Error:
                    Log.Error(message, false);
                    break;
                case TraceEventType.Warning:
                    Log.Warning(message, false);
                    break;
                case TraceEventType.Information:
                    Log.Info(message, false);
                    break;
                default:
                    Log.Verbose($"TraceEventType: {eventType}{Environment.NewLine}{message}");
                    break;
            }
        }

        #endregion

    }
}
