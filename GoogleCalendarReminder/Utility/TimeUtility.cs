using System;

namespace GoogleCalendarReminder.Utility
{
    public static class TimeUtility
    {
        private const bool Debug = false;
        public static TimeSpan GetTimeSpan(int value)
        {
            return new TimeSpan(0, !Debug ? value : 0, Debug ? value : 0);
        }
    }
}
