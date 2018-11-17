using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK.PInvoke;

namespace XJK.SysX
{
    public class WinFinder
    {
        public string ClassName { get; set; }
        public string CaptionName { get; set; }

        public bool IsFindForeground => string.IsNullOrEmpty(ClassName) && string.IsNullOrEmpty(CaptionName);

        public Win GetWin() => GetHandle();

        public IntPtr GetHandle()
        {
            if (IsFindForeground) return Win.Foreground.Get();
            return User32.FindWindow(ClassName, CaptionName);
        }

        public override string ToString()
        {
            if (IsFindForeground) return "<Foreground WinFinder>";
            return $"<WinFinder {ClassName} \"{CaptionName}\">";
        }

        public static Process[] ListProcessed() => Process.GetProcesses();
        public static string[] ListClassNames() => throw new NotImplementedException();
    }
}
