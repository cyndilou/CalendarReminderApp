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
        private readonly DispatcherTimer _updateTimer;

        private ObservableCollection<CalendarEvent> _calendarEventCollection;
        public ObservableCollection<CalendarEvent> CalendarEventCollection
        {
            get { return _calendarEventCollection ?? (_calendarEventCollection = new ObservableCollection<CalendarEvent>()); }
            set { _calendarEventCollection = value; }
        }

        private readonly int[] _baseSnoozeList = new[]{ 5, 10, 15, 20, 25, 30, 45, 60 };
        public ObservableCollection<int> SnoozeCollection { get; set; }

        public int SnoozeValue { get; set; }

        public MainWindowViewModel()
        {
            SnoozeCollection = new ObservableCollection<int>(_baseSnoozeList);

            _updateTimer = new DispatcherTimer
                               {
                                   Interval = TimeUtility.GetTimeSpan(Settings.Default.RefreshRate)
                               };

            _updateTimer.Tick += UpdateTimerTick;
            _updateTimer.Start();
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

        private void UpdateTimerTick(object sender, EventArgs e)
        {
            RefreshCalendarEntries();
        }

        public void RefreshCalendarEntries()
        {
            if (String.IsNullOrEmpty(Account.Default.Username))
            {
                return;
            }

            var worker = new BackgroundWorker();
            worker.DoWork += BackgroundWorkerDoWork;
            worker.RunWorkerCompleted += BackgroundWorkerWorkCompleted;

            worker.RunWorkerAsync();

            //foreach (var calendar in Account.Default.CalendarCollection.Where(c => c.IsSelected))
            //{
            //    List<CalendarEvent> eventList = GoogleCalendarService.GetCalendarEvents(calendar) ?? new List<CalendarEvent>();

            //    foreach (var calendarEvent in eventList)
            //    {
            //        CalendarEvent @event = calendarEvent;
            //        if (CalendarEventCollection.FirstOrDefault(o => o.ID == @event.ID) == null)
            //        {
            //            CalendarEventCollection.Add(calendarEvent);
            //        }
            //    }
            //}
        }

        void BackgroundWorkerWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var eventList = e.Result as List<CalendarEvent>;
            if (eventList == null) return;

            // First, remove all past events that have been dismissed as well as all future events
            // for which the reminder has yet to go off.
            // The future events will be re-populated with the results from the calendar service query.
            var itemsToRemove = CalendarEventCollection.Where(o => 
                                                                (((o.Status == EventStatus.Dismissed) && (o.EndTime < DateTime.Now))
                                                                //|| (o.Status == EventStatus.Future))).ToList();
                                                                || ((o.Status == EventStatus.Future) && (o.ReminderTime > DateTime.Now)))).ToList();
            foreach (var item in itemsToRemove)
            {
                CalendarEventCollection.Remove(item);
            }

            // Now, loop through the events from the calendar service and either update the entries
            // already in the collection or add them
            foreach (var calendarEvent in eventList)
            {
                CalendarEvent @newEvent = calendarEvent;
                var oldEvent = CalendarEventCollection.FirstOrDefault(o => o.ID == @newEvent.ID);
                if (oldEvent == null)
                {
                    CalendarEventCollection.Add(calendarEvent);
                }
                else
                {
                    oldEvent.Update(calendarEvent);
                }
            }

            //foreach (var calendarEvent in eventList)
            //{
            //    CalendarEvent @event = calendarEvent;
            //    if (CalendarEventCollection.FirstOrDefault(o => o.ID == @event.ID) == null)
            //    {
            //        CalendarEventCollection.Add(calendarEvent);
            //    }
            //}
        }

        static void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var eventList = new List<CalendarEvent>();

            foreach (var calendar in Account.Default.CalendarCollection.Where(c => c.IsSelected))
            {
                eventList.AddRange(GoogleCalendarService.GetCalendarEvents(calendar) ?? new List<CalendarEvent>());
            }

            e.Result = eventList;
        }
    }
}
