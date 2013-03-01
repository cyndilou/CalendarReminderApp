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
    public class NotificationService
    {
        #region Private Members

        private readonly DispatcherTimer _updateTimer;
        private System.Windows.Forms.NotifyIcon _notifyIcon;

        private readonly string _projectPageUrl = "http://cyndilou.github.com/CalendarReminderApp/";
        private readonly string _issueLogUrl = "https://github.com/cyndilou/CalendarReminderApp/issues";
        
        #endregion

        #region Singleton

        private static readonly NotificationService _instance = new NotificationService();
        public static NotificationService Instance { get { return _instance; } }

        private NotificationService()
        {
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeUtility.GetTimeSpan(1)
            };

            _updateTimer.Tick += UpdateTimerTick;
            _updateTimer.Start();

            CreateNotificationIcon();
        }

        #endregion

        #region Event Handlers

        private void UpdateTimerTick(object sender, EventArgs e)
        {
            // Show main window if there are any due events
            if (GoogleCalendarReminder.CalendarEventManager.Instance.AreEventsDue())
            {
                ShowMainWindow();
            }
        }

        private void NotifyIconClick(object sender, EventArgs e)
        {
            // If there are any due events, show the main window, otherwise show the upcoming events
            if (GoogleCalendarReminder.CalendarEventManager.Instance.AreEventsDue())
            {
                ShowMainWindow();
            }
            else
            {
                ShowUpcomingEvents();
            }
        }

        #endregion

        #region Content Menu Item Handlers

        private void OnSettings(object sender, EventArgs e)
        {
            ShowSettings(false);
        }

        private void OnLaunchProjectPage(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(_projectPageUrl);
        }

        private void OnViewLog(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(_issueLogUrl);
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            GoogleCalendarReminder.CalendarEventManager.Instance.RefreshCalendarEntries();
        }

        private static void OnAbout(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void OnExit(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void OnUpcomingEvents(object sender, EventArgs e)
        {
            ShowUpcomingEvents();
        }

        #endregion

        private void CreateNotificationIcon()
        {
            DestroyNotificationIcon();

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
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("View Project Page...", OnLaunchProjectPage));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("View Issue Log...", OnViewLog));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("About...", OnAbout));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
            _notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Exit", OnExit));

            _notifyIcon.Visible = true;
        }

        #region Private Methods

        private void DestroyNotificationIcon()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        }

        private void ShowSettings(bool closeOnCancel)
        {
            var settingsWindow = new AccountView();
            if (((settingsWindow.ShowDialog() == false) && closeOnCancel)
                || String.IsNullOrEmpty(Account.Default.Username))
            {
                Shutdown();
                return;
            }

            GoogleCalendarReminder.CalendarEventManager.Instance.Reset();
            GoogleCalendarReminder.CalendarEventManager.Instance.RefreshCalendarEntries();
        }

        private void ShowMainWindow()
        {
            GoogleCalendarReminder.UpcomingEvents.Instance.Close();

            GoogleCalendarReminder.MainWindow.Instance.Show(Settings.Default.IsAlwaysOnTop);
        }

        private void ShowUpcomingEvents()
        {
            GoogleCalendarReminder.MainWindow.Instance.Close();

            GoogleCalendarReminder.UpcomingEvents.Instance.Topmost = Settings.Default.IsAlwaysOnTop;
            GoogleCalendarReminder.UpcomingEvents.Instance.Show();
            GoogleCalendarReminder.UpcomingEvents.Instance.Activate();
        }

        private void Shutdown()
        {
            DestroyNotificationIcon();

            Application.Current.Shutdown();
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            ShowSettings(true);
        }

        #endregion
    }
}
