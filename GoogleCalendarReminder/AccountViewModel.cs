using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Security;
using System.Configuration;
using GoogleCalendarReminder.Properties;

namespace GoogleCalendarReminder
{
    public class AccountViewModel
    {
        #region Public Properties

        public string Username 
        {
            get { return Account.Default.Username; }
            set { Account.Default.Username = value; }
        }

        public SecureString Password 
        {
            get { return Account.Default.Password; }
            set { Account.Default.Password = value; }
        }

        public bool RememberPassword
        {
            get { return Account.Default.RememberPassword; }
            set { Account.Default.RememberPassword = value; }
        }

        public ObservableCollection<Calendar> CalendarCollection { get; set; }

        public int RefreshRate
        {
            get { return Settings.Default.RefreshRate; }
            set { Settings.Default.RefreshRate = value; }
        }

        public int DayRange
        {
            get { return Settings.Default.DayRange; }
            set { Settings.Default.DayRange = value; }
        }

        public bool LaunchOnStartup
        {
            get { return Settings.Default.LaunchOnStartup; }
            set { Settings.Default.LaunchOnStartup = value; }
        }

        #endregion Public Properties

        #region Constructor

        public AccountViewModel()
        {
            CalendarCollection = new ObservableCollection<Calendar>(Account.Default.CalendarCollection);
        }

        #endregion Constructor

        #region Public Methods

        public void SaveAccountSettings()
        {
            Account.Default.SaveSettings();
            Settings.Default.Save();
        }

        #endregion Public Methods
    }
}
