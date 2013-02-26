using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace GoogleCalendarReminder
{
    /// <summary>
    /// Interaction logic for UpcomingEvents.xaml
    /// </summary>
    public partial class UpcomingEvents : Window
    {
        public ReadOnlyObservableCollection<CalendarEvent> CalendarEventCollection { get { return GoogleCalendarReminder.CalendarEventManager.Instance.CalendarEventCollection; } }

        #region Singleton

        private static readonly UpcomingEvents _instance = new UpcomingEvents();
        public static UpcomingEvents Instance { get { return _instance; } }

        private UpcomingEvents()
        {
            InitializeComponent();

            DataContext = this;

            CalendarEventList.Items.SortDescriptions.Add(new SortDescription("StartTime", ListSortDirection.Ascending));
            CalendarEventList.Items.Refresh();
        }

        private void OnUpcomingEventsClosing(object sender, CancelEventArgs e)
        {
            // Since this window is a singleton, prevent it from actually closing, otherwise it won't be able to be opened again
            Hide();
            e.Cancel = true;
        }

        #endregion

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(CalendarEventList.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("StartTime.Date"));

            view.Filter = null;
            view.Filter = new Predicate<object>(o => ((CalendarEvent)o).Status != EventStatus.Dismissed);
        }

        

        #endregion

        private void OnOpenItem(object sender, RoutedEventArgs e)
        {
            var calendarEvent = CalendarEventList.SelectedItem as CalendarEvent;
            if (calendarEvent == null) return;

            GoogleCalendarReminder.CalendarEventManager.Instance.OpenItem(calendarEvent);
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var calendarEvent = ((ListViewItem)sender).Content as CalendarEvent;

            GoogleCalendarReminder.CalendarEventManager.Instance.OpenItem(calendarEvent);
        }
    }
}
