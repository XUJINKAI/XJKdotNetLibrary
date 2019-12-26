using System.Windows;

namespace XJK.WPF.Extensions
{
    public static class BoolExtension
    {
        public static Visibility ToVisibility(this bool bln)
        {
            return bln ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
