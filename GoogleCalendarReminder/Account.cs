using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Security;
using GoogleCalendarReminder.Properties;
using GoogleCalendarReminder.Utility;

namespace GoogleCalendarReminder
{
    public class Account : IDisposable
    {
        private static readonly Account DefaultInstance = new Account();
        public static Account Default { get { return DefaultInstance; } }

        public string Username { get; set; }
        public SecureString Password { get; set; }
        public bool RememberPassword { get; set; }

        private List<Calendar> _calendarCollection;
        public List<Calendar> CalendarCollection
        {
            get { return _calendarCollection ?? (_calendarCollection = new List<Calendar>()); }
            set { _calendarCollection = value; }
        }

        private StringCollection _selectedCalendars;
        private StringCollection SelectedCalendars
        {
            get { return _selectedCalendars ?? (_selectedCalendars = new StringCollection()); }
            set { _selectedCalendars = value; }
        }

        public Account()
        {
            Username = AccountSettings.Default.Username;
            Password = SecureStringUtility.DecryptString(AccountSettings.Default.Password);
            RememberPassword = AccountSettings.Default.RememberPassword;

            if (AccountSettings.Default.SelectedCalendars == null)
            {
                AccountSettings.Default.SelectedCalendars = new StringCollection();
            }

            SelectedCalendars = AccountSettings.Default.SelectedCalendars;
        }

        public void Dispose()
        {
            SaveSettings();
        }

        public bool IsCalendarSelected(Calendar calendar)
        {
            return SelectedCalendars.Count == 0 ? calendar.IsSelected : SelectedCalendars.Contains(calendar.CalendarUri);
        }

        public void SaveSettings()
        {
            AccountSettings.Default.Username = Username;
            AccountSettings.Default.RememberPassword = RememberPassword;
            AccountSettings.Default.Password = RememberPassword
                                                   ? SecureStringUtility.EncryptString(Password)
                                                   : "";

            AccountSettings.Default.SelectedCalendars.Clear();
            foreach (var calendar in CalendarCollection.Where(c => c.IsSelected))
            {
                AccountSettings.Default.SelectedCalendars.Add(calendar.CalendarUri);
            }

            AccountSettings.Default.Save();
        }
    }
}
