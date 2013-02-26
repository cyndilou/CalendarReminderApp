using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using GoogleCalendarReminder.Properties;

namespace GoogleCalendarReminder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

            GoogleCalendarReminder.NotificationService.Instance.Start();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Account.Default.Dispose();
            Settings.Default.Save();
        }
    }
}
