using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace EcoManager.Shared.Tools
{
    public static class VisualHelp
    {
        public static DependencyObject FindVisualParentRoot(FrameworkElement child)
        {
            if (child == null)
                return null;

            if (child.Parent == null)
                return child as DependencyObject;

            return FindVisualParentRoot(child.Parent as FrameworkElement);
        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(FrameworkElement.NameProperty) as string;

                if (controlName == name)
                    return child as T;

                T result = FindVisualChildByName<T>(child, name);

                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
