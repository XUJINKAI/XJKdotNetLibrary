using System;
using Microsoft.Win32;
using static XJK.SysX.NativeMethods;

namespace OneQuick.SysX.Device
{
    public static class OS
    {
        public static string ComputerName => Environment.MachineName;
        public static readonly string ProductName;//Windows 10 Pro
        public static readonly string ReleaseId;//1803
        public static readonly string CurrentBuild;//17133
        public static readonly string CurrentVersion;//6.3
        public static readonly string BuildBranch;//rs4_release
        public static readonly string OsArch;
        public static readonly string ProductId;
        public static readonly string OsFullName;

        static OS()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            ProductName = (string)reg.GetValue("ProductName");
            ReleaseId = (string)reg.GetValue("ReleaseId");
            CurrentBuild = (string)reg.GetValue("CurrentBuild");
            CurrentVersion = (string)reg.GetValue("CurrentVersion");
            BuildBranch = (string)reg.GetValue("BuildBranch");
            ProductId = (string)reg.GetValue("ProductId");
            OsArch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            OsFullName = $"{ProductName} {OsArch} {ReleaseId} ({CurrentBuild}.{BuildBranch})";
        }
    }
}
