﻿using System;

namespace GoogleCalendarReminder.Utility
{
    public static class TimeUtility
    {
        private const bool Debug = false;
        public static TimeSpan GetTimeSpan(int value)
        {
#if DEBUG
            return new TimeSpan(0, 0, value);
#else
            return new TimeSpan(0, value, 0);
#endif
            //return new TimeSpan(0, !Debug ? value : 0, Debug ? value : 0);
        }
    }
}
