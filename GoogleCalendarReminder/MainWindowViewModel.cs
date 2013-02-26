using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using GoogleCalendarReminder.Properties;
using System.ComponentModel;
using GoogleCalendarReminder.Utility;

namespace GoogleCalendarReminder
{
    public class MainWindowViewModel
    {
        public ReadOnlyObservableCollection<CalendarEvent> CalendarEventCollection
        {
            get { return GoogleCalendarReminder.CalendarEventManager.Instance.CalendarEventCollection; }
        }

        private readonly int[] _baseSnoozeList = new[]{ 5, 10, 15, 20, 25, 30, 45, 60 };
        public ObservableCollection<int> SnoozeCollection { get; set; }

        public int SnoozeValue { get; set; }

        public MainWindowViewModel()
        {
            SnoozeCollection = new ObservableCollection<int>(_baseSnoozeList);
        }
        
        public void UpdateSnoozeOptions(CalendarEvent calendarEvent)
        {
            if (calendarEvent == null) return;

            SnoozeCollection.Remove(-5);
            SnoozeCollection.Remove(-10);

            if (calendarEvent.StartTime.Subtract(TimeUtility.GetTimeSpan(10)) > DateTime.Now)
            {
                SnoozeCollection.Insert(0, -10);
            }

            if (calendarEvent.StartTime.Subtract(TimeUtility.GetTimeSpan(5)) > DateTime.Now)
            {
                SnoozeCollection.Insert(0, -5);
            }
        }
    }
}
