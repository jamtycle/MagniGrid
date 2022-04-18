using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MagniGrid.Core.Converters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null; 

            if (value is Color)
                return new SolidColorBrush((Color)value);
            else if (value is string)
                if (value.ToString()[0] == '#')
                    return new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString((string)value));
                else
                    return Colors.White;
            else
            {
                Console.WriteLine($"The media Type is unrecognizable {value.GetType()}");
                return Colors.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
