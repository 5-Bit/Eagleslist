using System;
using System.Globalization;
using System.Windows.Data;
using Humanizer;

namespace Eagleslist
{
    public class HumanizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;

            if (date.HasValue && date.Value != null)
            {
                return date.Value.Humanize(false);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
