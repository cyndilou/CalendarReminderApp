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

namespace GoogleCalendarReminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Private Members

        private readonly DispatcherTimer _updateTimer;
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private WindowState _storedWindowState = WindowState.Normal;
        private bool _shutdown = false;
        private const double Version = 0.1;
        private readonly string _feedbackUrl = "https://spreadsheets.google.com/spreadsheet/viewform?hl=en_US&formkey=dDJjekFWRUQxck9idk1OOVdSNklMYXc6MQ#gid=0";
        private readonly string _issueLogUrl = "https://spreadsheets.google.com/spreadsheet/pub?hl=en_US&hl=en_US&key=0AgH-9As5hO3RdDJjekFWRUQxck9idk1OOVdSNklMYXc&output=html";
        
        #endregion

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

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            _updateTimer = new DispatcherTimer
                               {
                                   Interval = TimeUtility.GetTimeSpan(1)
                               };

            _updateTimer.Tick += UpdateTimerTick;
            _updateTimer.Start();

            _notifyIcon = new System.Windows.Forms.NotifyIcon
                              {
                                  BalloonTipTitle = Properties.Resources.NotifierTitle,
                                  Text = Properties.Resources.NotifierTitle
                              };

            var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GoogleCalendarReminder.Resources.calendar.ico");
            if (stream != null)
            {
                _notifyIcon.Icon = new Icon(stream);
            }

            _notifyIcon.DoubleClick += NotifyIconClick;

            _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Refresh Calendars", OnRefresh));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Upcoming Events...", OnUpcomingEvents));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Settings...", new EventHandler(OnSettings)));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Send Feedback...", OnFeedback));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("View Issue Log...", OnViewLog));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("About...", OnAbout));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Exit", OnExit));
            
            _notifyIcon.Visible = true;
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

        private void UpdateTimerTick(object sender, EventArgs e)
        {
            FilterView();
        }

        void OnClose(object sender, CancelEventArgs args)
        {
            if (_shutdown == false)
            {
                args.Cancel = true;
                WindowState = WindowState.Minimized;
                return;
            }

            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            else
            {
                _storedWindowState = WindowState;
            }
        }

        private void NotifyIconClick(object sender, EventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    var preview = new UpcomingEvents(ViewModel.CalendarEventCollection.ToList());

            //    //var x = Mouse.GetPosition(this).X - preview.ActualWidth;
            //    //var y = Mouse.GetPosition(this).Y - preview.ActualHeight - 10;
            //    //if (y < 0)
            //    //    y = Mouse.GetPosition(this).Y;

            //    //preview..SetDesktopLocation(x, y);
            //    preview.Show();
            //}

            if (CalendarEventList.Items.Count > 0)
            {
                Show();
                WindowState = WindowState.Normal;
                //WindowState = _storedWindowState;
            }
            else
            {
                var upcomingEvents = new UpcomingEvents(ViewModel.CalendarEventCollection.ToList());

                //var x = Mouse.GetPosition(this).X - upcomingEvents.Width;
                //var y = Mouse.GetPosition(this).Y - upcomingEvents.Height - 10;
                //if (y < 0)
                //    y = Mouse.GetPosition(this).Y;

                //upcomingEvents.Left = x;
                //upcomingEvents.Top = y;

                upcomingEvents.Show();
            }

            

            //Show();
            //WindowState = m_storedWindowState;
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

        #region Content Menu Item Handlers

        private void OnSettings(object sender, EventArgs e)
        {
            ShowSettings(false);
        }

        private void OnFeedback(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(_feedbackUrl);
        }

        private void OnViewLog(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(_issueLogUrl);
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            ViewModel.RefreshCalendarEntries();
        }

        private static void OnAbout(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void OnExit(object sender, EventArgs e)
        {
            _shutdown = true;
            Close();
        }

        private void OnUpcomingEvents(object sender, EventArgs e)
        {
            var upcomingEvents = new UpcomingEvents(ViewModel.CalendarEventCollection.ToList());
            upcomingEvents.Show();
        }

        #endregion

        #region Private Methods

        private void FilterView()
        {
            var view = CollectionViewSource.GetDefaultView(CalendarEventList.ItemsSource);
            view.Filter = null;
            view.Filter = new Predicate<object>(FilterTestEvents);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("StartTime", ListSortDirection.Ascending));

            if (CalendarEventList.SelectedIndex == -1)
            {
                CalendarEventList.SelectedIndex = 0;
            }

            if (CalendarEventList.Items.Count <= 0)
            {
                WindowState = WindowState.Minimized;
            }
            else
            {
                Show();
                WindowState = WindowState.Normal;
                //WindowState = _storedWindowState;
            }
        }

        private static bool FilterTestEvents(object obj)
        {
            var item = obj as CalendarEvent;
            if (item == null) return false;

            return item.Status != EventStatus.Dismissed && item.ReminderTime <= DateTime.Now;
        }

        private void ShowTrayIcon(bool show)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = show;
            }
        }

        #endregion

        private void MainWindowControlLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            ShowSettings(true);
        }

        private void ShowSettings(bool closeOnCancel)
        {
            var settingsWindow = new AccountView();
            if (((settingsWindow.ShowDialog() == false) && closeOnCancel)
                || String.IsNullOrEmpty(Account.Default.Username))
            {
                _shutdown = true;
                Close();
                return;
            }

            ViewModel.CalendarEventCollection.Clear();
            ViewModel.RefreshCalendarEntries();

            FilterView();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //ShowTrayIcon(!IsVisible);
        }
    }
}
