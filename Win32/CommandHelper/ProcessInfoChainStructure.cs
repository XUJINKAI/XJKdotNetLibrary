using System;
using System.Diagnostics;

namespace XJK.Win32.CommandHelper
{
    public enum ExcuteStatus
    {
        NotRun = 0,
        Success,
        UserCancel,
        Exception,
    }

    public class ExcuteResult
    {
        public ProcessInfoChain InfoChain { get; set; } = null;
        public ExcuteStatus ExcuteStatus { get; set; } = ExcuteStatus.NotRun;
        public Exception Exception { get; set; } = null;

        private void NeedAfterRun(string action)
        {
            if (ExcuteStatus == ExcuteStatus.NotRun)
            {
                throw new Exception($"[ExcuteResult] ProcessInfoChain Need Excute before '{action}'");
            }
        }

        public ExcuteResult Catch(Action<Exception> action)
        {
            NeedAfterRun("Catch");
            if (ExcuteStatus == ExcuteStatus.Exception)
            {
                action(Exception);
            }
            return this;
        }

        public ExcuteResult Finally(Action<ExcuteResult> action)
        {
            NeedAfterRun("Finally");
            if (ExcuteStatus != ExcuteStatus.NotRun)
            {
                action(this);
            }
            return this;
        }
    }

    public static class ProcessInfoChainExtension
    {
        public static ProcessInfoChain ToChain(this ProcessStartInfo startInfo)
        {
            return new ProcessInfoChain(startInfo);
        }
    }
}
