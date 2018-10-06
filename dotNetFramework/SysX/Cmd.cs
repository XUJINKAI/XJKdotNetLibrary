using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XJK;
using XJK.SysX.CommandHelper;
using static XJK.SysX.CommandHelper.CommandLineHelper;

namespace XJK.SysX
{
    public static class Cmd
    {
        public static string GetRunVerbs(string filename)
        {
            return (new ProcessStartInfo(filename)).Verbs.Join(", ");
        }

        public static bool IsUrl(string str)
        {
            string t = str.ToLower();
            return t.StartsWith("http://") || t.StartsWith("https://");
        }
        
        public static void Explorer(string path)
        {
            if (File.Exists(path))
            {
                RunAsInvoker("explorer", "/select, " + path);
            }
            else if (Directory.Exists(path))
            {
                RunAsInvoker("explorer", path);
            }
            else
            {
                RunSmart("explorer");
            }
        }

        public static Tuple<string, string> SplitCommandArg(string CommandWithArgs)
        {
            var argArray = CommandLineToArgs(CommandWithArgs);
            string command = argArray[0];
            string arg = CommandWithArgs.Substring(Math.Min(command.Length + 1, CommandWithArgs.Length));
            return Tuple.Create(command, arg);
        }

        public static void RunSmart(string CommandWithArgs)
        {
            if (IsUrl(CommandWithArgs))
            {
                RunAsInvoker(CommandWithArgs, "");
            }
            else
            {
                var tuple = SplitCommandArg(CommandWithArgs);
                string command = tuple.Item1;
                string arg = tuple.Item2;
                bool showWindow = command.ToLower().Equals("cmd") && arg.IsNullOrEmpty();
                if(showWindow) RunAsInvoker(command, arg);
                else ProcessInfoChain.New(command, arg).SetWindow(WindowType.ConsoleNoWindow).Excute();
            }
        }

        #region Process Run

        public static ExcuteResult RunAsInvoker(string Command, string Args)
        {
             return ProcessInfoChain.New(Command, Args).Excute();
        }

        public static ExcuteResult RunAsAdmin(string Command, string Args)
        {
            return ProcessInfoChain.New(Command, Args).RunAs(Privilege.Admin).Excute();
        }
        
        public static ExcuteResult RunWithCmdStart(string Command, string Args)
        {
            return ProcessInfoChain.New(Command, Args).LaunchBy(LaunchType.CmdStart).Excute();
        }
        
        #endregion

    }

}
