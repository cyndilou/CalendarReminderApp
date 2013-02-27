using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace GoogleCalendarReminder.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) { return Visibility.Collapsed; }

            bool bCompareTo = true;
            if (parameter != null)
            {
                bool.TryParse(parameter.ToString(), out bCompareTo);
            }

            bool bValue = (bool)value;

            return bValue == bCompareTo ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
