using System.Windows;

namespace XJK.Win32.SysX
{
    public static class Clipboards
    {
        public static string GetText() => Clipboard.GetText();
        public static void SetText(string text) => Clipboard.SetDataObject(text);
    }
}
