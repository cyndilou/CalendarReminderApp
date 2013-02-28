using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using GoogleCalendarReminder.Properties;
using GoogleCalendarReminder.Utility;
using System.Collections.ObjectModel;

namespace GoogleCalendarReminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Public Properties

        public MainWindowViewModel ViewModel 
        { 
            get { return DataContext as MainWindowViewModel; }
            set { DataContext = value; }
        }
        
        public CalendarEvent SelectedCalendarEvent
        {
            get { return (CalendarEvent)GetValue(SelectedCalendarEventProperty); }
            set { SetValue(SelectedCalendarEventProperty, value); }
        }

        #endregion

        #region Dependency Properties

        // Using a DependencyProperty as the backing store for SelectedCalendarEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedCalendarEventProperty =
            DependencyProperty.Register("SelectedCalendarEvent", typeof(CalendarEvent), typeof(MainWindow), new PropertyMetadata(OnSelectedEventChanged));

        private static void OnSelectedEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainWindow = d as MainWindow;
            if (mainWindow == null) return;

            mainWindow.ViewModel.UpdateSnoozeOptions(mainWindow.SelectedCalendarEvent);
            mainWindow.SnoozeCombo.SelectedIndex = 0;
        }

        #endregion

        #region Singleton

        private static readonly MainWindow _instance = new MainWindow();
        public static MainWindow Instance { get { return _instance; } }

        private MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();
        }

        private void OnMainWindowControlClosing(object sender, CancelEventArgs e)
        {
            // Since this window is a singleton, prevent it from actually closing, otherwise it won't be able to be opened again
            Hide();
            e.Cancel = true;

            var view = CollectionViewSource.GetDefaultView(CalendarEventList.ItemsSource);

            if (view != null)
            {
                view.Filter = null;
            }
        }

        #endregion

        #region Public Methods

        public void Show(bool isTopmost)
        {
            Topmost = isTopmost;
            
            Show();

            if (!Topmost)
            {
                Activate();
            }

            FilterView();
        }

        #endregion

        #region Window Events

        private void OnMainWindowControlActivated(object sender, EventArgs e)
        {
            FilterView();
        }

        #endregion

        #region Event Handlers

        private void SnoozeClicked(object sender, RoutedEventArgs e)
        {
            var calendarEvent = CalendarEventList.SelectedItem as CalendarEvent;
            if (calendarEvent == null) return;

            calendarEvent.Status = EventStatus.Snoozed;
            calendarEvent.ReminderTime = ViewModel.SnoozeValue < 0 
                                        ? calendarEvent.StartTime.Subtract(TimeUtility.GetTimeSpan(-1 * ViewModel.SnoozeValue)) 
                                        : DateTime.Now.Add(TimeUtility.GetTimeSpan(ViewModel.SnoozeValue));

            FilterView();
        }

        private void DismissAllClicked(object sender, RoutedEventArgs e)
        {
            foreach (var calendarEvent in
                from object item in CalendarEventList.Items select item as CalendarEvent)
            {
                calendarEvent.Status = EventStatus.Dismissed;
            }

            FilterView();
        }

        private void DismissClicked(object sender, RoutedEventArgs e)
        {
            var calendarEvent = CalendarEventList.SelectedItem as CalendarEvent;
            if (calendarEvent == null) return;

            calendarEvent.Status = EventStatus.Dismissed;
            
            FilterView();
        }

        private void OpenItemClicked(object sender, RoutedEventArgs e)
        {
            var calendarEvent = CalendarEventList.SelectedItem as CalendarEvent;
            if (calendarEvent == null) return;

            try
            {
                System.Diagnostics.Process.Start(calendarEvent.Url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var calendarEvent = ((ListViewItem)sender).Content as CalendarEvent;

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

        #region Private Methods

        private void FilterView()
        {
            var selectedItem = CalendarEventList.SelectedIndex;

            var view = CollectionViewSource.GetDefaultView(CalendarEventList.ItemsSource);
            view.Filter = null;
            view.Filter = new Predicate<object>(FilterTestEvents);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("StartTime", ListSortDirection.Ascending));

            CalendarEventList.SelectedIndex = selectedItem >= CalendarEventList.Items.Count ? (CalendarEventList.Items.Count - 1) : selectedItem;

            if (CalendarEventList.SelectedIndex == -1)
            {
                CalendarEventList.SelectedIndex = 0;
            }

            if (CalendarEventList.Items.Count <= 0)
            {
                Close();
            }
        }

        private static bool FilterTestEvents(object obj)
        {
            var item = obj as CalendarEvent;
            if (item == null) return false;

            return item.IsDue;
        }

        #endregion

    }
}
