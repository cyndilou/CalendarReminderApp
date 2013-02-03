using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleCalendarReminder
{
    public class GoogleCalendarAuthToken
    {
        private DateTime TokenAquiredTime { get; set; }

        private string _authToken;
        public string AuthToken
        {
            get { return _authToken; }
            set
            {
                _authToken = value;
                TokenAquiredTime = DateTime.Now;
            }
        }

        public bool IsExpired()
        {
            // Tokens are good for 24 hours
            return TokenAquiredTime < (DateTime.Now - new TimeSpan(23, 0, 0));
        }
    }
}
