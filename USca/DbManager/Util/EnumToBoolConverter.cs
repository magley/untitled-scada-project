using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace USca_DbManager.Util
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Equals(false))
            {
                return DependencyProperty.UnsetValue;

            }
            return parameter;
        }
    }
}
