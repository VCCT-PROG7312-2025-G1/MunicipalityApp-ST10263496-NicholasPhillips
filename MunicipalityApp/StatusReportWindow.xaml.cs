using MunicipalityApp.Data;
using MunicipalityApp.Models;
using System.Windows;

namespace MunicipalityApp
{
    /// <summary>
    /// Interaction logic for StatusReportWindow.
    /// Displays a list of reported issues and their statuses.
    /// </summary>
    public partial class StatusReportWindow : Window
    {
        /// <summary>
        /// Initializes the StatusReportWindow and loads the reported issues.
        /// </summary>
        public StatusReportWindow()
        {
            InitializeComponent();
            LoadIssues();
        }

        /// <summary>
        /// Loads all reported issues from the IssueRepository and displays them in the list.
        /// If no issues exist, shows a placeholder message.
        /// </summary>
        private void LoadIssues()
        {
            lstIssues.Items.Clear();

            var issues = IssueRepository.Issues;
            
            if (!issues.Any())
            {
                lstIssues.Items.Add("No issues reported yet.");
                return;
            }

            // Get issues in reverse chronological order (newest first)
            foreach (var issue in IssueRepository.GetIssuesNewestFirst())
            {
                lstIssues.Items.Add($"{issue.Title} ({issue.Category}) – Status: {issue.Status} [Reported: {issue.ReportedDate:g}]");
            }
        }

        /// <summary>
        /// Handles the Back button click event.
        /// Returns to the main window and closes the current window.
        /// </summary>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
