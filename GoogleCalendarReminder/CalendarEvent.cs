using System;
using System.Collections.Generic;
using System.Linq;
using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.Extensions;

namespace GoogleCalendarReminder
{
    public enum EventStatus
    {
        Future = 0
        , Snoozed
        , Dismissed
    }

    public enum ParticipantStatus
    {
        Accepted = 0
        , Tentative
        , Declined
    }
    
    public class CalendarEvent
    {
        #region Public Properties

        public AtomId ID { get; set; }
        public string Subject { get; set; }
        
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime ReminderTime { get; set; }
        public string Url { get; set; }
        public string Color { get; set; }

        public TimeSpan Duration { get { return EndTime - StartTime; } }

        public EventStatus Status { get; set; }
        public ParticipantStatus MyStatus { get; set; }

        //public bool IsActive { get; set; }

        public bool IsDue { get { return Status != EventStatus.Dismissed && ReminderTime <= DateTime.Now; } }

        #endregion

        #region Constructor

        public CalendarEvent(EventEntry entry)
        {
            ID = entry.Id;
            Subject = entry.Title.Text;
            Location = entry.Locations.Count > 0 ? entry.Locations.FirstOrDefault().ValueString : String.Empty;
            Url = entry.Links.Count > 0 ? entry.Links[0].HRef.Content : String.Empty;

            StartTime = entry.Times.Count > 0 ? entry.Times.FirstOrDefault().StartTime : DateTime.Now;
            EndTime = entry.Times.Count > 0 ? entry.Times.FirstOrDefault().EndTime : StartTime.AddMinutes(1);

            // Default reminder to 30 minutes before
            ReminderTime = StartTime.Subtract(new TimeSpan(0, 30, 0));

            // Now find the real reminder
            foreach (var t in entry.Reminders.Where(t => t.Method == Google.GData.Extensions.Reminder.ReminderMethod.alert))
            {
                ReminderTime = StartTime.Subtract(new TimeSpan(t.Days, t.Hours, t.Minutes, 0));
                break;
            }

            SetMyStatus(entry.Participants);

            Status = (MyStatus == ParticipantStatus.Declined) ? EventStatus.Dismissed : EventStatus.Future;
        }

        public void Update(CalendarEvent calendarEvent)
        {
            ID = calendarEvent.ID;
            Subject = calendarEvent.Subject;
            Location = calendarEvent.Location;
            Url = calendarEvent.Url;
            
            if (calendarEvent.StartTime != StartTime)
            {
                StartTime = calendarEvent.StartTime;
                EndTime = calendarEvent.EndTime;
                ReminderTime = calendarEvent.ReminderTime;
                Status = EventStatus.Future;
            }
        }

        private void SetMyStatus(IEnumerable<Who> participants)
        {
            MyStatus = ParticipantStatus.Accepted;

            var me = participants.FirstOrDefault(participant => participant.Email.StartsWith(Account.Default.Username));
            if (me == null || me.Attendee_Status == null) { return; }

            switch (me.Attendee_Status.Value)
            {
                case Who.AttendeeStatus.EVENT_ACCEPTED:
                    MyStatus = ParticipantStatus.Accepted;
                    break;
                case Who.AttendeeStatus.EVENT_DECLINED:
                    MyStatus = ParticipantStatus.Declined;
                    break;
                case Who.AttendeeStatus.EVENT_TENTATIVE:
                    MyStatus = ParticipantStatus.Tentative;
                    break;
                default:
                    MyStatus = ParticipantStatus.Accepted;
                    break;
            }
        }

        #endregion
    }
}
