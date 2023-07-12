using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using USca_DbManager.Tags;

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
    public class BoolInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                return !val;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                return !val;
            }
            return value;
        }
    }

    public class TagDTOToCustomStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TagDTO tag)
            {
                return TagDTO.SimpleString(tag);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
