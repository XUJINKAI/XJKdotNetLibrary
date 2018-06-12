using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XJK.SysX
{
    static class Reg
    {
        public static bool IsAutorun(string ProductName, string PathFullName, bool SetIfHasProductName = false)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            var path = key.GetValue(ProductName);
            if (path == null)
            {
                return false;
            }
            else
            {
                if((string)path == PathFullName)
                {
                    return true;
                }
                else
                {
                    if (SetIfHasProductName)
                    {
                        SetAutorun(ProductName, PathFullName, true);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static void SetAutorun(string ProductName, string PathFullName, bool Autorun)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (Autorun)
                {
                    key.SetValue(ProductName, PathFullName);
                }
                else
                {
                    key.DeleteValue(ProductName);
                }
            }
            catch { }
        }
    }
}
