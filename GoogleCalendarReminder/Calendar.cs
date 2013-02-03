using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Calendar;
using System.ComponentModel;

namespace GoogleCalendarReminder
{
    public class Calendar : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Color { get; set; }
        public string CalendarUri { get; set; }
        public bool IsMine { get; set; }

        public bool IsSelected { get; set; }

        public Calendar(CalendarEntry calendar)
        {
            Title = calendar.Title.Text;

            //System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(calendar.Color);
            //color.A = 225;

            //Color = color;

            Color = calendar.Color;
            CalendarUri = calendar.Content.AbsoluteUri;

            IsMine = IsSelected = String.Compare(calendar.AccessLevel, "owner") == 0;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("IsMine"));
            }

            
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
