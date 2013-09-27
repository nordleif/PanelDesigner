using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace PanelDesigner
{
    internal static class Extensions
    {
        public static FrameworkElement Clone(this FrameworkElement element)
        {
            var xaml = XamlWriter.Save(element);
            var newElement = (FrameworkElement)XamlReader.Parse(xaml);
            return newElement;
        }

        public static void AddAdorner(this FrameworkElement element, Adorner adorner)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            adornerLayer.Add(adorner);
        }

        public static bool ContainsAdorner<T>(this FrameworkElement element) where T: class
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            var adorners = adornerLayer.GetAdorners(element);
            if (adorners == null)
                return false;
            foreach (var adorner in adorners)
                if (adorner is T)
                    return true;
            return false;
        }

        public static T GetAdorner<T>(this FrameworkElement element) where T : class
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer == null)
                return default(T);
            var adorners = adornerLayer.GetAdorners(element);
            if (adorners == null)
                return default(T);
            return adorners.SingleOrDefault(a => a is T) as T;
        }

        public static void RemoveAdorner(this FrameworkElement element, Adorner adorner)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            adornerLayer.Remove(adorner);
        }

        public static void TrySetValue(this FrameworkElement element, string propertyName, object value)
        {
            try
            {
                var type = element.GetType();
                var property = type.GetProperty(propertyName);
                if (property != null)
                    property.SetValue(element, value, null);
            }
            catch { }
        }

        internal static IEnumerable<FrameworkElement> GetDescendants(this PanelDesigner panelDesigner)
        {
            return (panelDesigner.Content as Panel).GetDescendants();
        }

        internal static IEnumerable<FrameworkElement> GetDescendants(this Panel panel)
        {
            var result = new List<FrameworkElement>();
            if (panel != null)
            {
                foreach (var child in panel.Children)
                {
                    if (child is FrameworkElement)
                        result.Add((FrameworkElement)child);
                    if (child is Panel)
                        result.AddRange(((Panel)child).GetDescendants());
                }
            }
            return result.ToArray();
        }
    }
}