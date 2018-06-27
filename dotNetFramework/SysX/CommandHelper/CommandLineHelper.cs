using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XJK.SysX
{
    public static class CommandLineHelper
    {
        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            if (commandLine == "") return new string[] { };
            var argv = CommandLineToArgvW(commandLine, out int argc);
            if (argv == IntPtr.Zero)
                throw new Win32Exception();
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

        public static bool ShouldQuote(string command)
        {
            bool head_tail_quote = command.StartsWith("\"") && command.EndsWith("\"");
            bool contains_char = command.Contains(' ') || command.Contains("\"");
            return !head_tail_quote && contains_char;
        }

        public enum QuoteRepalce
        {
            DoubleQuote,
            BackSlashQuote,
        }

        public static string QuoteMark(string cmd, QuoteRepalce QuoteType)
        {
            switch (QuoteType)
            {
                case QuoteRepalce.DoubleQuote:
                    cmd = cmd.Replace("\"", "\"\"");
                    break;
                case QuoteRepalce.BackSlashQuote:
                    cmd = cmd.Replace("\"", "\\\"");
                    break;
            }
            cmd = "\"" + cmd + "\"";
            return cmd;
        }

        public static string JoinArgs(string[] Args)
        {
            for (int i = 0; i < Args.Length; i++)
            {
                Args[i] = QuoteMark(Args[i], QuoteRepalce.DoubleQuote);
            }
            return Args.Join(" ");
        }

    }
}
