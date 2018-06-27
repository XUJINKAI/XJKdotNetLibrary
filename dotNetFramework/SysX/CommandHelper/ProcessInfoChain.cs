using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static XJK.SysX.CommandLineHelper;

namespace XJK.SysX
{
    public enum RunType
    {
        Invoker,
        Admin,
        Limited,
        CmdStart,
        CmdStartAdmin,
    }

    public enum WindowType
    {
        Show,
        Hide,
        NoWin,
        NoWinHide,
    }

    public class ProcessInfoChain
    {
        public ProcessStartInfo ProcessStartInfo
        {
            get
            {
                Apply();
                return _processStartInfo;
            }
        }
        private readonly ProcessStartInfo _processStartInfo;
        private Process _process = null;
        
        private event Action<string> RedirectOutputAction;
        private event Action<string> RedirectErrorAction;
        private event Action<Exception> CatchExceptionAction;
        
        public string Command { get; set; } = "";
        public string Args { get; set; } = "";
        public string CommandApplied { get; private set; } = "";
        public string ArgsApplied { get; private set; } = "";
        public RunType RunType { get; set; } = RunType.Invoker;
        public WindowType WindowType { get; set; } = WindowType.Show;

        #region 构造函数
        public ProcessInfoChain()
        {
            _processStartInfo = new ProcessStartInfo();
        }

        public ProcessInfoChain(ProcessStartInfo processStartInfo)
        {
            _processStartInfo = processStartInfo;
        }

        public static ProcessInfoChain New()
        {
            return new ProcessInfoChain();
        }

        public static ProcessInfoChain New(string Command, string Args)
        {
            return new ProcessInfoChain()
            {
                Command = Command,
                Args = Args
            };
        }

        public static ProcessInfoChain New(ProcessStartInfo processStartInfo)
        {
            return new ProcessInfoChain(processStartInfo);
        }
        #endregion

        public ProcessInfoChain Set(Action<ProcessStartInfo> action)
        {
            action(ProcessStartInfo);
            return this;
        }

        public ProcessInfoChain RunAs(RunType runType)
        {
            RunType = runType;
            return this;
        }

        public ProcessInfoChain Window(WindowType window)
        {
            WindowType = window;
            return this;
        }

        public ProcessInfoChain CmdLine(string command, string args)
        {
            Command = command;
            Args = args;
            return this;
        }
        
        public ProcessInfoChain RedirectOutput(Action<string> action)
        {
            RedirectOutputAction += action;
            return this;
        }

        public ProcessInfoChain RedirectError(Action<string> action)
        {
            RedirectErrorAction += action;
            return this;
        }

        public ProcessInfoChain Catch(Action<Exception> action)
        {
            CatchExceptionAction += action;
            return this;
        }

        public ProcessInfoChain Apply()
        {
            string argarg;
            switch (RunType)
            {
                case RunType.Invoker:
                case RunType.Admin:
                    CommandApplied = Command;
                    ArgsApplied = Args;
                    break;
                case RunType.Limited:
                    argarg = ShouldQuote(Command) ? QuoteMark(Command, QuoteRepalce.DoubleQuote) : Command;
                    if (!Args.IsNullOrEmpty())
                    {
                        argarg += " " + Args;
                    }
                    CommandApplied = "RunAs";
                    ArgsApplied = $"/trustlevel:0x20000 {QuoteMark(argarg, QuoteRepalce.BackSlashQuote)}";
                    break;
                case RunType.CmdStart:
                case RunType.CmdStartAdmin:
                    argarg = ShouldQuote(Command) ? QuoteMark(Command, QuoteRepalce.DoubleQuote) : Command;
                    if (!Args.IsNullOrEmpty())
                    {
                        argarg += " " + Args;
                    }
                    CommandApplied = "cmd";
                    ArgsApplied = $"/c start \"\" {argarg}";
                    break;
                default:
                    throw new Exception("ProcessInfoChain unknown RunAs Type");
            }
            bool as_admin = RunType == RunType.Admin || RunType == RunType.CmdStartAdmin;
            if (as_admin)
            {
                _processStartInfo.UseShellExecute = true;
                _processStartInfo.Verb = "RunAs";
            }
            _processStartInfo.FileName = CommandApplied;
            _processStartInfo.Arguments = ArgsApplied;
            switch (WindowType)
            {
                case WindowType.Show:
                    _processStartInfo.CreateNoWindow = false;
                    _processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    break;
                case WindowType.Hide:
                    _processStartInfo.CreateNoWindow = false;
                    _processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    break;
                case WindowType.NoWin:
                    _processStartInfo.CreateNoWindow = true;
                    _processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    break;
                case WindowType.NoWinHide:
                    _processStartInfo.CreateNoWindow = true;
                    _processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    break;
            }
            bool has_redirect = RedirectOutputAction != null || RedirectErrorAction != null;
            if (has_redirect)
            {
                if (as_admin)
                {
                    Log.Warning("[ProcessInfoChain.Apply] AsAdmin but HasRedirect, UseShellExecute conflict");
                }
                _processStartInfo.UseShellExecute = false;
            }
            _processStartInfo.RedirectStandardOutput = RedirectOutputAction != null;
            _processStartInfo.RedirectStandardError = RedirectErrorAction != null;
            return this;
        }

        public ProcessInfoChain LogDetail()
        {
            Apply();
            Log.Debug($"Apply Process Chain{C.LF}Command: {_processStartInfo.FileName}{C.LF}Args: {_processStartInfo.Arguments}{C.LF}" +
                $"Verb: {_processStartInfo.Verb}{C.LF}WindowStyle: {_processStartInfo.WindowStyle}, {_processStartInfo.CreateNoWindow}");
            return this;
        }

        const int ERROR_CANCELLED = 1223;

        public ProcessInfoChain Start()
        {
            Apply();
            try
            {
                _process = Process.Start(_processStartInfo);
                if (RedirectOutputAction != null)
                {
                    Task.Run(async () =>
                    {
                        var x = await _process.StandardOutput.ReadToEndAsync();
                        Log.Debug($"Process Result: {x}");
                        RedirectOutputAction(x);
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                if (ex is Win32Exception win32Exception)
                {
                    if (win32Exception.NativeErrorCode == ERROR_CANCELLED)
                    {
                        Log.Trace($"[User Canceled Run] {_processStartInfo.FileName} {_processStartInfo.Arguments}");
                        return this;
                    }
                }
                if (RedirectErrorAction != null)
                {
                    Task.Run(async () =>
                    {
                        var x = await _process.StandardError.ReadToEndAsync();
                        Log.Debug($"Process Error: {x}");
                        RedirectErrorAction(x);
                    });
                }
                if (CatchExceptionAction != null)
                {
                    CatchExceptionAction(ex);
                }
                else
                {
                    throw ex;
                }
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
