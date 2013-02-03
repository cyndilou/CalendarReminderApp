using System;
using System.Windows.Data;

namespace GoogleCalendarReminder.Converters
{
    public class MyCalendarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return String.Empty;

            var isMine = (bool)value;
            return isMine ? "My Calendars" : "Other Calendars";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
