using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace GoogleCalendarReminder.Converters
{
    [ValueConversion(typeof(Color), typeof(LinearGradientBrush))]
    public class GradientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var brush = new LinearGradientBrush();
            string color = (string)value;

            brush.StartPoint = new Point(0.5, 0);
            brush.EndPoint = new Point(0.5, 1);
            brush.Opacity = 0.3;

            brush.GradientStops.Add(new GradientStop(Colors.Transparent, 0));
            brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(color), 1));

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

