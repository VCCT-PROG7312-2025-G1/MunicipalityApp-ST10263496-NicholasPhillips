using System.Windows;

namespace MunicipalityApp
{

    // Interaction logic for MainWindow.
    // Serves as the main menu for the MunicipalityApp, allowing navigation to other windows.

    public partial class MainWindow : Window
    {

        // Initializes the MainWindow and its components.
        public MainWindow()
        {
            InitializeComponent();
            // Register this instance with the global navigation manager so other windows
            // can return to the same MainWindow instance instead of creating new ones.
            MunicipalityApp.Services.NavigationManager.RegisterMainWindow(this);
        }

        // Handles the Report Issues button click event.
        // Opens the ReportIssueWindow and hides the main window.

        private void btnReportIssues_Click(object sender, RoutedEventArgs e)
        {
            // Navigate the main Frame to the ReportIssuePage
            MainFrame.Navigate(new Pages.ReportIssuePage());
            var toast = new ToastNotification("Opening Report Issue form...");
            toast.Show();
        }


        // Handles the Announcements button click event.
        // Opens the AnnouncementsWindow and hides the main window.
        private void btnAnnouncements_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.AnnouncementsPage());
            var toast = new ToastNotification("Opening Announcements...");
            toast.Show();
        }


        // Handles the Status Report button click event.
        // Opens the StatusReportWindow and hides the main window.

        private void btnStatusReport_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.StatusReportPage());
            var toast = new ToastNotification("Opening Status Report...");
            toast.Show();
        }


        // Handles the Home button click event.
        // No action needed, already on the home page.

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the internal home page
            MainFrame.Navigate(new Pages.MainHomePage());
            var toast = new ToastNotification("You're already on the home page");
            toast.Show();
        }


        // Handles the Exit button click event.
        // Shuts down the application.
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
