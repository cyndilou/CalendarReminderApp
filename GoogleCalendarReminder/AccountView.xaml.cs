using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GoogleCalendarReminder.Utility;

namespace GoogleCalendarReminder
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class AccountView
    {
        #region Public Properties

        public AccountViewModel ViewModel 
        { 
            get { return DataContext as AccountViewModel; }
            set { DataContext = value;  }
        }

        #endregion

        #region Constructor

        public AccountView()
        {
            InitializeComponent();

            ViewModel = new AccountViewModel();
        }

        #endregion

        #region Events

        private void OnOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Account.Default.CalendarCollection = ViewModel.CalendarCollection.ToList();

            ViewModel.SaveAccountSettings();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.Password = PasswordEdit.SecurePassword;
        }

        private void ConnectClicked(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void Connect()
        {
            if (String.IsNullOrEmpty(ViewModel.Username)
                || ViewModel.Password.Length == 0)
            {
                return;
            }

            Cursor = Cursors.Wait;

            LoadCalendars();

            Cursor = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PasswordEdit.Password = SecureStringUtility.SecureStringToString(ViewModel.Password);

            var view = CollectionViewSource.GetDefaultView(CalendarsList.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("IsMine"));

            Connect();
        }

        #endregion

        #region Private Methods

        private void LoadCalendars()
        {
            ViewModel.CalendarCollection.Clear();

            List<Calendar> calendarList = GoogleCalendarService.GetCalendarList() ?? new List<Calendar>();

            foreach (var calendar in calendarList)
            {
                calendar.IsSelected = Account.Default.IsCalendarSelected(calendar);
                ViewModel.CalendarCollection.Add(calendar);
            }
        }
            
        #endregion
    }
}
