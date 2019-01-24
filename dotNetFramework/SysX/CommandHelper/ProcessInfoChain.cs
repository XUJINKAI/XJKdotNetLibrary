using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XJK.Logger;
using static XJK.SysX.CommandHelper.CommandLineHelper;

namespace XJK.SysX.CommandHelper
{
    /*
     * WindowStyle --> UI
     * UseShellExecute --> default (new console) (NO Redirect)
     * !UseShellExecute --> in parent's console window (NO Admin)
     * !UseShellExecute && CreateNoWindow --> No Window (NO Admin)
     * RunAs (Admin) --> UseShellExecute = true
     * Redirect --> UseShellExecute = false
     */

    public enum Privilege
    {
        NotSet = 0,
        Invoker,
        Limited, // (change command or not)
        Admin, // UseShellExecute && RunAs
    }

    public enum LaunchType
    {
        NotSet = 0,
        Normal,
        CmdStart, // (change command)
    }

    public enum WindowType
    {
        NotSet = 0,
        Normal,
        Hidden,
        Minimized,
        Maximized,
        ConsoleInParent, // !UseShellExecute (NO Admin)
        ConsoleNoWindow, // !UseShellExecute && CreateNoWindow (NO Admin)
    }

    public class ProcessInfoChain
    {
        public ProcessStartInfo ProcessStartInfo { get; private set; }
        public Process Process { get; private set; }
        public bool ThrowConflict { get; set; } = true;

        private const int ERROR_CANCELLED = 1223;
        private Privilege _privilege = Privilege.NotSet;
        private LaunchType _launchType = LaunchType.NotSet;
        private WindowType _windowType = WindowType.NotSet;

        public Privilege Privilege
        {
            get => _privilege; set
            {
                switch (value)
                {
                    case Privilege.Admin:
                        ProcessStartInfo.UseShellExecute = true;
                        ProcessStartInfo.Verb = "RunAs";
                        break;
                    default:
                        ProcessStartInfo.Verb = "";
                        break;
                }
                _privilege = value;
                AutoDefineWindowStyle();
            }
        }

        public LaunchType LaunchType
        {
            get => _launchType; set
            {
                _launchType = value;
                AutoDefineWindowStyle();
            }
        }

        public WindowType WindowType
        {
            get => _windowType; set
            {
                switch (value)
                {
                    case WindowType.Normal:
                        ProcessStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        break;
                    case WindowType.Hidden:
                        ProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        break;
                    case WindowType.Minimized:
                        ProcessStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                        break;
                    case WindowType.Maximized:
                        ProcessStartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        break;
                    case WindowType.ConsoleInParent:
                        ProcessStartInfo.UseShellExecute = false;
                        break;
                    case WindowType.ConsoleNoWindow:
                        ProcessStartInfo.UseShellExecute = false;
                        ProcessStartInfo.CreateNoWindow = true;
                        break;
                }
                _windowType = value;
            }
        }

        public string Command
        {
            get => ProcessStartInfo.FileName;
            set => ProcessStartInfo.FileName = value;
        }
        public string Args
        {
            get => ProcessStartInfo.Arguments;
            set => ProcessStartInfo.Arguments = value;
        }

        private void AutoDefineWindowStyle()
        {
            // if change cmd/arg
            if (Privilege == Privilege.Limited && ENV.IsAdministrator()
                || LaunchType == LaunchType.CmdStart)
            {
                // if not conflict
                if (Privilege != Privilege.Admin && WindowType == WindowType.NotSet)
                {
                    WindowType = WindowType.ConsoleNoWindow;
                }
            }
        }

        public Tuple<string, string> ExcuteCommandArgs
        {
            get
            {
                string cmd = Command;
                string arg = Args;
                if (Privilege == Privilege.Limited)
                {
                    if (ENV.IsAdministrator())
                    {
                        string combine = ShouldQuote(cmd) ? QuoteMark(cmd, QuoteRepalce.DoubleQuote) : cmd;
                        combine += arg.IsNullOrEmpty() ? "" : (" " + arg);
                        cmd = "RunAs";
                        arg = $"/trustlevel:0x20000 {QuoteMark(combine, QuoteRepalce.BackSlashQuote)}";
                    }
                }
                if (LaunchType == LaunchType.CmdStart)
                {
                    string combine = ShouldQuote(cmd) ? QuoteMark(cmd, QuoteRepalce.DoubleQuote) : cmd;
                    combine += arg.IsNullOrEmpty() ? "" : (" " + arg);
                    cmd = "cmd";
                    arg = $"/c start \"\" {combine}";
                }
                return Tuple.Create(cmd, arg);
            }
        }
        

        #region 构造函数
        public ProcessInfoChain()
        {
            ProcessStartInfo = new ProcessStartInfo();
        }

        public ProcessInfoChain(ProcessStartInfo processStartInfo)
        {
            ProcessStartInfo = processStartInfo;
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

        #region Set Property

        public ProcessInfoChain DumpInfo(Action<string> action)
        {
            action(this.ToString());
            return this;
        }
        
        public ProcessInfoChain SetCommand(string command, string args)
        {
            Command = command;
            Args = args;
            return this;
        }

        public ProcessInfoChain Set(Action<ProcessStartInfo> action)
        {
            action(ProcessStartInfo);
            return this;
        }

        public ProcessInfoChain RunAs(Privilege Privilege)
        {
            this.Privilege = Privilege;
            return this;
        }

        public ProcessInfoChain LaunchBy(LaunchType LaunchType)
        {
            this.LaunchType = LaunchType;
            return this;
        }

        public ProcessInfoChain SetWindow(WindowType WindowType)
        {
            this.WindowType = WindowType;
            return this;
        }

        #endregion

        public ProcessInfoChain CheckConflict()
        {
            string conflict = "";
            if (Privilege == Privilege.Admin
                && (WindowType == WindowType.ConsoleInParent || WindowType == WindowType.ConsoleNoWindow)
                )
            {
                conflict += "RunAs Admin confict with ConsoleInParent/ConsoleNoWindow WindowType" + Environment.NewLine;
            }

            if (conflict != "")
            {
                conflict = "ProcessInfoChain Conflict:" + Environment.NewLine + conflict;
                if (ThrowConflict)
                {
                    throw new Exception(conflict);
                }
                else
                {
                    Log.Warning(conflict);
                }
            }
            return this;
        }

        public ExcuteResult Excute(bool ThrowExp = true)
        {
            ExcuteResult excuteResult = new ExcuteResult()
            {
                InfoChain = this,
            };

            Log.DEBUG(this);
            CheckConflict();
            var ActualCmdArg = ExcuteCommandArgs;
            ProcessStartInfo.FileName = ActualCmdArg.Item1;
            ProcessStartInfo.Arguments = ActualCmdArg.Item2;

            try
            {
                Process = Process.Start(ProcessStartInfo);
                excuteResult.ExcuteStatus = ExcuteStatus.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                if (ex is Win32Exception win32Exception && win32Exception.NativeErrorCode == ERROR_CANCELLED)
                {
                    excuteResult.ExcuteStatus = ExcuteStatus.UserCancel;
                }
                else
                {
                    excuteResult.ExcuteStatus = ExcuteStatus.Exception;
                    excuteResult.Exception = ex;

                    if (ThrowExp)
                    {
                        throw ex;
                    }
                }
            }
            return excuteResult;
        }

        public override string ToString()
        {
            return  $"<ProcessInfoChain>{Environment.NewLine}" +
                $"Command : {Command}{Environment.NewLine}" +
                $"Argument: {Args}{Environment.NewLine}" +
                $"ExcuteCommandArgs: {ExcuteCommandArgs.Item1} {ExcuteCommandArgs.Item2}{Environment.NewLine}" +
                $"{Privilege}, {LaunchType}, {WindowType}";
        }
    }
}
