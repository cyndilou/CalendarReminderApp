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
        public List<CalendarEvent> CalendarEventCollection { get; set; }

        public UpcomingEvents(List<CalendarEvent> calendarEventCollection)
        {
            InitializeComponent();

            CalendarEventCollection = calendarEventCollection;
            DataContext = this;

            CalendarEventList.Items.SortDescriptions.Add(new SortDescription("StartTime", ListSortDirection.Ascending));
            CalendarEventList.Items.Refresh();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(CalendarEventList.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("StartTime.Date"));

            view.Filter = null;
            view.Filter = new Predicate<object>(o => ((CalendarEvent)o).Status != EventStatus.Dismissed);
        }

        private void OnOpenItem(object sender, RoutedEventArgs e)
        {
            var calendarEvent = CalendarEventList.SelectedItem as CalendarEvent;
            if (calendarEvent == null) return;

            OpenItem(calendarEvent);
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var calendarEvent = ((ListViewItem)sender).Content as CalendarEvent;

            OpenItem(calendarEvent);
        }

        private void OpenItem(CalendarEvent calendarEvent)
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
    }
}
