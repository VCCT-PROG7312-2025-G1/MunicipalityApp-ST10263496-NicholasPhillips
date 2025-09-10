using System.Windows;

namespace MunicipalityApp
{
    /// <summary>
    /// Interaction logic for MainWindow.
    /// Serves as the main menu for the MunicipalityApp, allowing navigation to other windows.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes the MainWindow and its components.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Report Issues button click event.
        /// Opens the ReportIssueWindow and hides the main window.
        /// </summary>
        private void btnReportIssues_Click(object sender, RoutedEventArgs e)
        {
            ReportIssueWindow reportWindow = new ReportIssueWindow();
            reportWindow.Show();
            this.Hide();
            
            var toast = new ToastNotification("Opening Report Issue form...");
            toast.Show();
        }

        /// <summary>
        /// Handles the Announcements button click event.
        /// Opens the AnnouncementsWindow and hides the main window.
        /// </summary>
        private void btnAnnouncements_Click(object sender, RoutedEventArgs e)
        {
            AnnouncementsWindow announcementsWindow = new AnnouncementsWindow();
            announcementsWindow.Show();
            this.Hide();
            
            var toast = new ToastNotification("Opening Announcements...");
            toast.Show();
        }

        /// <summary>
        /// Handles the Status Report button click event.
        /// Opens the StatusReportWindow and hides the main window.
        /// </summary>
        private void btnStatusReport_Click(object sender, RoutedEventArgs e)
        {
            StatusReportWindow statusWindow = new StatusReportWindow();
            statusWindow.Show();
            this.Hide();
            
            var toast = new ToastNotification("Opening Status Report...");
            toast.Show();
        }

        /// <summary>
        /// Handles the Home button click event.
        /// No action needed, already on the home page.
        /// </summary>
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            // Already on the home page, no action needed
            var toast = new ToastNotification("You're already on the home page");
            toast.Show();
        }

        /// <summary>
        /// Handles the Exit button click event.
        /// Shuts down the application.
        /// </summary>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
