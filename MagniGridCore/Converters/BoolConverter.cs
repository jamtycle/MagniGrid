using System;
using System.Globalization;
using System.Windows.Data;

namespace MagniGrid.Core.Converters
{
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) { return null; }

            if (value is string)
                return !(string.IsNullOrEmpty((string)value) || string.IsNullOrWhiteSpace((string)value));
            if (value is bool) 
                return (bool)value; 

            throw new InvalidOperationException($"Unsupported type [{value.GetType()}]");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
