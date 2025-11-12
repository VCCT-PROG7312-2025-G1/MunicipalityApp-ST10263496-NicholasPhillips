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
            // Open the ReportIssueWindow and hide the main window.
            MunicipalityApp.Services.NavigationManager.NavigateTo(new ReportIssueWindow());
            var toast = new ToastNotification("Opening Report Issue form...");
            toast.Show();
        }


        // Handles the Announcements button click event.
        // Opens the AnnouncementsWindow and hides the main window.
        private void btnAnnouncements_Click(object sender, RoutedEventArgs e)
        {
            MunicipalityApp.Services.NavigationManager.NavigateTo(new AnnouncementsWindow());
            var toast = new ToastNotification("Opening Announcements...");
            toast.Show();
        }


        // Handles the Status Report button click event.
        // Opens the StatusReportWindow and hides the main window.

        private void btnStatusReport_Click(object sender, RoutedEventArgs e)
        {
            // Open the ServiceRequestStatusWindow (window-based UI) and hide the main window.
            MunicipalityApp.Services.NavigationManager.NavigateTo(new Windows.ServiceRequestStatusWindow());
            var toast = new ToastNotification("Opening Service Request Status window...");
            toast.Show();
        }

        // Notifications tab removed from navigation bar. Message Center can still be opened
        // programmatically if needed elsewhere in the app.


        // Handles the Home button click event.
        // No action needed, already on the home page.

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            // Already on the main window; show a friendly toast and ensure focus
            var toast = new ToastNotification("Showing Home...");
            toast.Show();
            this.Activate();
        }


        // Handles the Exit button click event.
        // Shuts down the application.
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
