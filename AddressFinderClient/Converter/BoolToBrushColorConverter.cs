using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AddressFinderClient.Converter
{
    class BoolToBrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = value as bool?;

            if (res is true) return new SolidColorBrush(Colors.Green);

            return new SolidColorBrush(
                res is true
                    ? Colors.Green
                    : res is false
                        ? Colors.Red
                        : Colors.Gray);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
