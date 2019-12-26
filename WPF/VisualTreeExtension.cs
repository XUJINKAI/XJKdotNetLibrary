using System;
using System.Windows;
using System.Windows.Media;

namespace XJK.WPF
{
    public static class VisualTreeExtension
    {
        public static DependencyObject GetParent(this DependencyObject element)
        {
            return VisualTreeHelper.GetParent(element);
        }

        public static DependencyObject FindVisualTreeParent(this DependencyObject element, Type findType, DependencyObject stopElement = null)
        {
            if (element == null) return null;

            var parent = element.GetParent();
            if (parent == null)
            {
                if (((FrameworkElement)element).Parent is DependencyObject)
                {
                    parent = ((FrameworkElement)element).Parent;
                }
            }
            if (parent == null) return null;

            if (parent.GetType() == findType || parent.GetType().IsSubclassOf(findType))
                return parent;
            if (parent == stopElement)
                return null;
            return FindVisualTreeParent(parent, findType, stopElement);
        }
    }
}
