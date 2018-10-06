using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XJK.SysX
{
    public static class Clipboards
    {
        public static string GetText() => Clipboard.GetText();
        public static void SetText(string text) => Clipboard.SetDataObject(text);
    }
}
