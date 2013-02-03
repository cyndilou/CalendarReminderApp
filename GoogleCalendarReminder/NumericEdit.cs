using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace GoogleCalendarReminder
{
    public class NumericEdit : TextBox
    {
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(NumericEdit), null);

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericEdit), null);

        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text);

            double value = 0.0;
            if (Double.TryParse(e.Text, out value))
            {
                //e.Handled = Double.IsNaN(MinValue)
                //                ? e.Handled
                //                : (value < MinValue ? true : e.Handled);

                //e.Handled = Double.IsNaN(MaxValue)
                //                ? e.Handled
                //                : (value > MaxValue ? true : e.Handled);
            }

            base.OnPreviewTextInput(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
        }

        private static bool AreAllValidNumericChars(IEnumerable<char> str)
        {
            return str.All(Char.IsNumber);
        }
    }
}
