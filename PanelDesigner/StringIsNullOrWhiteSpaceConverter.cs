using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PanelDesigner
{
    internal class StringIsNullOrWhiteSpaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterStr = parameter as string;
            if (string.IsNullOrWhiteSpace(parameterStr))
                parameterStr = "<no name>";
            
            var valueStr = value as string;
            if (valueStr == null)
                return parameterStr;

            if (string.IsNullOrWhiteSpace(valueStr))
                return parameterStr;
            else
                return valueStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
