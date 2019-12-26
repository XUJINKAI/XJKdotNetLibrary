using System;
using System.Diagnostics;
using XJK.Win32.PInvoke;

namespace XJK.Win32.SysX
{
    public class WinFinder
    {
        public string ClassName { get; set; }
        public string CaptionName { get; set; }

        public bool IsFindForeground => string.IsNullOrEmpty(ClassName) && string.IsNullOrEmpty(CaptionName);

        public override string ToString()
        {
            if (IsFindForeground) return "<Foreground WinFinder>";
            return $"<WinFinder {ClassName} \"{CaptionName}\">";
        }

        public static Process[] ListProcessed() => Process.GetProcesses();
        public static string[] ListClassNames() => throw new NotImplementedException();
    }

    public static class WinFinderExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="winFinder"></param>
        /// <returns></returns>
        public static IntPtr GetHandle(this WinFinder winFinder)
        {
            if (winFinder == null) return Win.Foreground.Get();
            if (winFinder.IsFindForeground) return Win.Foreground.Get();
            return User32.FindWindow(winFinder.ClassName, winFinder.CaptionName);
        }
    }
}
