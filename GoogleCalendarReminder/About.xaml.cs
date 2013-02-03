using System.Deployment.Application;
using System.Diagnostics;

namespace GoogleCalendarReminder
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        public string AppName { get; set; }
        public string VersionNumber { get; set; }
        public string Author { get; set; }

        public About()
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            AppName = assembly.GetName().Name;
            VersionNumber = GetVersion();
            Author = fileVersionInfo.CompanyName;

            DataContext = this;
        }

        private static string GetVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }

            var assemblyInfo = System.Reflection.Assembly.GetExecutingAssembly();
            return assemblyInfo.GetName().Version.ToString();
        }
    }
}
