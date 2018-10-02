using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace XJK.SysX
{
    public static class Clips
    {
        public static string GetText() => Clipboard.GetText();
        public static void SetText(string text) => Clipboard.SetDataObject(text);
    }
}
