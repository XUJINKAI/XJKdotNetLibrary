using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace XJK
{
    public class LogTraceListener : TraceListener
    {
        #region Write
        public override void Write(object o)
        {
            Log.Write(o.ToString());
        }

        public override void Write(object o, string category)
        {
            Log.Write($"{category}: {o}");
        }

        public override void Write(string message)
        {
            Log.Write(message);
        }

        public override void Write(string message, string category)
        {
            Log.Write($"{category}: {message}");
        }
        #endregion

        #region WriteLine
        public override void WriteLine(object o)
        {
            Log.Verbose(o, "", "", "", -1);
        }

        public override void WriteLine(object o, string category)
        {
            Log.Verbose(o, category, "", "", -1);
        }

        public override void WriteLine(string message)
        {
            Log.Verbose(message, "", "", "", -1);
        }

        public override void WriteLine(string message, string category)
        {
            Log.Verbose(message, category, "", "", -1);
        }
        #endregion

        #region Fail
        public override void Fail(string message)
        {
            Log.Error(message, "", "", "", -1);
        }

        public override void Fail(string message, string detailMessage)
        {
            Log.Error(message + C.LF + detailMessage, "", "", "", -1);
        }
        #endregion

        #region TraceEvent
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            throw new NotImplementedException();
        }
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            throw new NotImplementedException();
        }
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    Log.Error(message, source, "", "", -1);
                    break;
                case TraceEventType.Warning:
                    Log.Warning(message, source, "", "", -1);
                    break;
                case TraceEventType.Information:
                    Log.Info(message, source, "", "", -1);
                    break;
                case TraceEventType.Verbose:
                    Log.Verbose(message, source, "", "", -1);
                    break;
                default:
                    Log.Verbose($"[{eventType}]{message}", source, "", "", -1);
                    break;
            }
        }
        #endregion

    }
}
