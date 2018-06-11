using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OneQuick.SysX
{
    static class Cmd
    {
        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            var argv = CommandLineToArgvW(commandLine, out int argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }
        
        public static void RunSmart(string CommandWithArgs, bool ShowWindow = true)
        {
            if (IsUrl(CommandWithArgs))
            {
                OpenLink(CommandWithArgs);
            }
            else
            {
                var args = CommandLineToArgs(CommandWithArgs);
                string command = args[0];
                string arg = CommandWithArgs.Substring(Math.Min(command.Length + 1, CommandWithArgs.Length));
                Run(command, arg, ShowWindow);
            }
        }
        public static void Run(string Command, string Args, bool ShowWindow = true)
        {
            if (ShowWindow)
            {
                System.Diagnostics.Process.Start(Command, Args);
            }
            else
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                        FileName = Command,
                        Arguments = Args
                    }
                };
                process.Start();
            }
        }

        public static bool IsUrl(string str)
        {
            string t = str.ToLower();
            return t.StartsWith("http://") || t.StartsWith("https://");
        }

        public static void OpenLink(string url)
        {
            if (IsUrl(url))
            {
                Run(url, "", true);
            }
            else
            {
                throw new Exception("Not URL.");
            }
        }

        public static void Explorer(string path)
        {
            if (global::System.IO.File.Exists(path))
            {
                Run("explorer", "/select, " + path);
            }
            else if (global::System.IO.Directory.Exists(path))
            {
                Run("explorer", path);
            }
            else
            {
                RunSmart("explorer");
            }
        }
    }
}
