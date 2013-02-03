using System;
using System.Windows.Data;

namespace GoogleCalendarReminder.Converters
{
    public class DateGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return String.Empty;

            var time = (DateTime)value;
            if (time.Date == DateTime.Today)
            {
                return "Today";
            }

            return time.Date == DateTime.Today.AddDays(1) ? "Tomorrow" : time.ToString("D");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
