using System;
using System.Windows.Data;

namespace GoogleCalendarReminder.Converters
{
    public class DueInConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return String.Empty;

            var startTime = (DateTime) value;

            if (startTime < DateTime.Now)
            {
                return "Overdue " + GetString(DateTime.Now.Subtract(startTime));
            }

            return GetString(startTime.Subtract(DateTime.Now));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string GetString(TimeSpan timeSpan)
        {
            if (Math.Round(timeSpan.TotalDays) > 0) { return Math.Round(timeSpan.TotalDays) + " days"; }
            if (Math.Round(timeSpan.TotalHours) > 0) { return Math.Round(timeSpan.TotalHours) + " hours"; }

            return Math.Round(timeSpan.TotalMinutes) + " minutes";
        }
    }
}
