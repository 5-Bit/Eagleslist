using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Eagleslist
{
    public class OwnerIdToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var testId = value as int?;

            if (testId.HasValue && testId.Value > 0)
            {
                var id = CredentialManager.GetCurrentUser()?.ID;

                if (id.HasValue)
                {
                    return id.Value == testId.Value ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
