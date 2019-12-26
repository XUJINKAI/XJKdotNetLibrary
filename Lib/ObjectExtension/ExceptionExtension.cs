using System;
using System.Text;
using XJK.ReflectionUtils;

namespace XJK
{
    public static class ExceptionExtension
    {
        public static string GetFullMessage(this Exception ex)
        {
            return GetMessage(ex, true, true, true);
        }

        public static string GetMessage(this Exception ex, bool DumpData = true, bool StackTrace = false, bool OuterException = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<Exception>");
            BuildMessageFunc(true, sb, ex, DumpData, StackTrace, OuterException);
            return sb.ToString();
        }

        private static void BuildMessageFunc(bool IsOuterCall, StringBuilder sb, Exception ex, bool DumpData, bool StackTrace, bool OuterException)
        {
            if (ex.InnerException != null)
            {
                BuildMessageFunc(false, sb, ex.InnerException, DumpData, StackTrace, OuterException);
            }
            if (OuterException || ex.InnerException == null)
            {
                sb.Append("Message: ");
                sb.AppendLine(ex.Message);
                if (DumpData && ex.Data.Count > 0)
                {
                    sb.AppendLine("Data:");
                    string[] dump = ex.Data.Dump().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    bool firstline = true;
                    foreach (var line in dump)
                    {
                        if (!firstline)
                        {
                            sb.AppendLine(line);
                        }
                        firstline = false;
                    }
                }
                if (StackTrace)
                {
                    sb.AppendLine("StackTrace:");
                    sb.AppendLine(ex.StackTrace);
                }
                if (!IsOuterCall)
                {
                    sb.AppendLine("--- InnerException Above ---");
                }
            }
        }
    }
}
