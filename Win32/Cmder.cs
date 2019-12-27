using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using XJK.Win32.CommandHelper;
using static XJK.Win32.CommandHelper.CmdRunner;

namespace XJK.Win32
{
    public static class Cmder
    {
        public static string[] CommandLineToArgs(string commandLine)
        {
            if (string.IsNullOrEmpty(commandLine))
                return Array.Empty<string>();

            var argv = PInvoke.Shell32.CommandLineToArgvW(commandLine, out int argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();

            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p) ?? "";
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }


        public static string GetRunVerbs(string filename)
        {
            return (new ProcessStartInfo(filename)).Verbs.JoinToString(", ");
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

        public static (string, string) SplitCommandArg(string CommandWithArgs)
        {
            var argArray = CommandLineToArgs(CommandWithArgs);
            string command = argArray[0];
            string arg = CommandWithArgs.Substring(Math.Min(command.Length + 1, CommandWithArgs.Length));
            return (command, arg);
        }

        public static void RunSmart(string CommandWithArgs)
        {
            if (IsUrl(CommandWithArgs))
            {
                RunAsInvoker(CommandWithArgs, "");
            }
            else
            {
                var (command, arg) = SplitCommandArg(CommandWithArgs);
                bool showWindow = command.ToLower().Equals("cmd") && arg.IsNullOrEmpty();
                if (showWindow) RunAsInvoker(command, arg);
                else ProcessInfoChain.New(command, arg).SetWindow(WindowType.ConsoleNoWindow).Excute();
            }
        }

    }
}
