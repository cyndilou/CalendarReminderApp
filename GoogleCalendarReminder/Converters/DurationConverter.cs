using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace GoogleCalendarReminder.Converters
{
    class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return String.Empty;

            var duration = (TimeSpan)value;

            double durationValue = 0.0;
            string durationUnit;

            if (duration.Days > 0)
            {
                durationValue = Math.Round(duration.TotalDays, 2);
                durationUnit = " day";
            }
            else if (duration.Hours > 0)
            {
                durationValue = Math.Round(duration.TotalHours, 2);
                durationUnit = " hour";
            }
            else
            {
                durationValue = duration.Minutes;
                durationUnit = " minutes";
            }

            if (durationValue > 1)
            {
                durationUnit += "s";
            }

            return durationValue + durationUnit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
