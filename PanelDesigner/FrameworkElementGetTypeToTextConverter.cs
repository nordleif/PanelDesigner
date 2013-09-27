using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PanelDesigner
{
    internal class FrameworkElementGetTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var element = value as FrameworkElement;
            if (element == null)
                return value;

            return value.GetType().Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
