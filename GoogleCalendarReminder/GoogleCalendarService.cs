using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Calendar;
using System.Windows;
using Google.GData.Client;
using GoogleCalendarReminder.Properties;
using System.Xml;
using GoogleCalendarReminder.Utility;

namespace GoogleCalendarReminder
{
    public static class GoogleCalendarService
    {
        public static List<Calendar> GetCalendarList()
        {
            // Create a CalenderService and authenticate
            var service = new CalendarService("GoogleCalendarReminder");
            service.SetAuthenticationToken(GetAuthToken());

            //service.setUserCredentials(Account.Default.Username, SecureStringUtility.SecureStringToString(Account.Default.Password));

            var query = new CalendarQuery
                            {
                                Uri = new Uri("https://www.google.com/calendar/feeds/default/allcalendars/full")
                            };

            CalendarFeed resultFeed;

            try
            {
                resultFeed = service.Query(query) as CalendarFeed;
            }
            catch (CaptchaRequiredException ex)
            {
                // TODO: CDA - http://code.google.com/googleapps/faq.html
                MessageBox.Show(ex.Message, ex.GetType().ToString());

                //https://www.google.com/accounts/UnlockCaptcha
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
                return null;
            }

            return (from CalendarEntry entry in resultFeed.Entries where !entry.Hidden select new Calendar(entry)).ToList();
        }

        public static List<CalendarEvent> GetCalendarEvents(Calendar calendar)
        {
            List<CalendarEvent> eventList = new List<CalendarEvent>();

            EventQuery query = new EventQuery();
            CalendarService service = new CalendarService("GoogleCalendarReminder");
            service.SetAuthenticationToken(GetAuthToken());

            //service.setUserCredentials(Account.Default.Username
            //                            , SecureStringUtility.SecureStringToString(Account.Default.Password));

            query.Uri = new Uri(calendar.CalendarUri);
            query.SingleEvents = true;
            query.StartTime = DateTime.Now;
            query.EndTime = DateTime.Today.AddDays(Settings.Default.DayRange);

            EventFeed calFeed;
            try
            {
                calFeed = service.Query(query) as EventFeed;
            }
            catch (Exception)
            {
                return null;
            }

            // now populate the calendar
            while (calFeed != null && calFeed.Entries.Count > 0)
            {
                foreach (EventEntry entry in calFeed.Entries)
                {
                    eventList.Add(new CalendarEvent(entry)
                                        {
                                            Color = calendar.Color
                                        });
                }

                // just query the same query again.
                if (calFeed.NextChunk != null)
                {
                    query.Uri = new Uri(calFeed.NextChunk);

                    try
                    {
                        calFeed = service.Query(query) as EventFeed;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                else
                {
                    calFeed = null;
                }
            }

            return eventList;
        }

        public static void WriteToStandardOutput(EventEntry entry)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(Console.Out);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.Indentation = 2;
            entry.SaveToXml(xmlWriter);
            xmlWriter.Flush();
        }


        private static GoogleCalendarAuthToken _googleCalendarAuthToken;
        private static GoogleCalendarAuthToken GoogleCalendarAuthToken
        {
            get { return _googleCalendarAuthToken ?? (_googleCalendarAuthToken = new GoogleCalendarAuthToken()); }
            set { _googleCalendarAuthToken = value; }
        }

        private static string GetAuthToken()
        {
            if (GoogleCalendarAuthToken.IsExpired())
            {
                var service = new CalendarService("GoogleCalendarReminder");

                service.setUserCredentials(Account.Default.Username
                                            , SecureStringUtility.SecureStringToString(Account.Default.Password));

                try
                {
                    GoogleCalendarAuthToken.AuthToken = service.QueryClientLoginToken();
                }
                catch (CaptchaRequiredException ex)
                {
                    // TODO: CDA - http://code.google.com/googleapps/faq.html
                    MessageBox.Show(ex.Message, ex.GetType().ToString());

                    //https://www.google.com/accounts/UnlockCaptcha
                    return null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().ToString());
                    return null;
                }
            }

            return GoogleCalendarAuthToken.AuthToken;
        }
    }
}
