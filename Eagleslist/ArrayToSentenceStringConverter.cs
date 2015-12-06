using System;
using System.Globalization;
using System.Windows.Data;
using System.Collections.Generic;

namespace Eagleslist
{
    public class ArrayToSentenceStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var array = value as List<string>;

            if (array == null)
            {
                return "";
            }

            if (array.Count == 0)
            {
                return "";
            }
            else if (array.Count == 1)
            {
                return array[0];
            }
            else
            {
                var builder = array[0];
                for (var i = 1; i < array.Count; i++)
                {
                    if (i == array.Count - 1)
                    {
                        builder += " and ";
                    }
                    else
                    {
                        builder += ", ";
                    }

                    builder += array[i];
                }

                return builder;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
