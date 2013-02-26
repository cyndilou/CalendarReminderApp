using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using GoogleCalendarReminder.Properties;
using System.ComponentModel;
using GoogleCalendarReminder.Utility;
using System.Windows;

namespace GoogleCalendarReminder
{
    public sealed class CalendarEventManager
    {
        #region Private Members

        private readonly DispatcherTimer _updateTimer;

        #endregion

        #region Public Properties

        private static readonly CalendarEventManager _instance = new CalendarEventManager();
        public static CalendarEventManager Instance { get { return _instance; } }

        private ObservableCollection<CalendarEvent> _calendarEventCollection;
        private ReadOnlyObservableCollection<CalendarEvent> _calendarEventCollectionReadOnly;

        public ReadOnlyObservableCollection<CalendarEvent> CalendarEventCollection
        {
            get { return _calendarEventCollectionReadOnly; }
        }

        #endregion

        #region c'tor

        private CalendarEventManager()
        {
            _calendarEventCollection = new ObservableCollection<CalendarEvent>();
            _calendarEventCollectionReadOnly = new ReadOnlyObservableCollection<CalendarEvent>(_calendarEventCollection);

            _updateTimer = new DispatcherTimer
            {
                Interval = TimeUtility.GetTimeSpan(Settings.Default.RefreshRate)
            };

            _updateTimer.Tick += UpdateTimerTick;
            _updateTimer.Start();
        }

        #endregion

        #region Private Methods

        private void UpdateTimerTick(object sender, EventArgs e)
        {
            RefreshCalendarEntries();
        }

        private void BackgroundWorkerWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                _calendarEventCollection.Remove(item);
            }

            // Now, loop through the events from the calendar service and either update the entries
            // already in the collection or add them
            foreach (var calendarEvent in eventList)
            {
                CalendarEvent @newEvent = calendarEvent;
                var oldEvent = CalendarEventCollection.FirstOrDefault(o => o.ID == @newEvent.ID);
                if (oldEvent == null)
                {
                    _calendarEventCollection.Add(calendarEvent);
                }
                else
                {
                    oldEvent.Update(calendarEvent);
                }
            }
        }

        private static void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var eventList = new List<CalendarEvent>();

            foreach (var calendar in Account.Default.CalendarCollection.Where(c => c.IsSelected))
            {
                eventList.AddRange(GoogleCalendarService.GetCalendarEvents(calendar) ?? new List<CalendarEvent>());
            }

            e.Result = eventList;
        }

        #endregion

        #region Public Methods

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
        }

        public void Reset()
        {
            _calendarEventCollection.Clear();
        }

        public bool AreEventsDue()
        {
            foreach (var entry in CalendarEventCollection)
            {
                if (entry.IsDue)
                {
                    return true;
                }
            }

            return false;
        }

        public void OpenItem(CalendarEvent calendarEvent)
        {
            try
            {
                System.Diagnostics.Process.Start(calendarEvent.Url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

    }
}
