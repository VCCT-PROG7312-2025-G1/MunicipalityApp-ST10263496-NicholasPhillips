using MunicipalityApp.Data;
using MunicipalityApp.Models;
using MunicipalityApp.Services;
using System;
using System.Linq;
using System.Windows;

namespace MunicipalityApp
{

    // Interaction logic for StatusReportWindow.
    // Displays a list of reported issues and their statuses.

    public partial class StatusReportWindow : Window
    {
        private ServiceRequestIndex _index;

        // Initializes the StatusReportWindow and loads the reported issues.
        public StatusReportWindow()
        {
            InitializeComponent();
            BuildIndexes();
            LoadIssues();
        }


        // Loads all reported issues from the IssueRepository and displays them in the list.
        // If no issues exist, shows a placeholder message.

        private void LoadIssues()
        {
            var issues = IssueRepository.GetIssuesNewestFirst().ToArray();
            lstIssues.ItemsSource = issues;
        }


        // Builds advanced indices (BST/AVL/RBT/Heap/Graph) over the current repository items.

        private void BuildIndexes()
        {
            _index = new ServiceRequestIndex(IssueRepository.Issues);
        }


        // Handles the Back button click event.
        // Returns to the main window and closes the current window.
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }


        // Finds and selects a service request in the list by its GUID entered in the search box.
        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            var text = txtSearchId.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Please enter a Service Request ID (GUID).", "Find Request", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (!Guid.TryParse(text, out var id))
            {
                MessageBox.Show("Invalid ID format. Please paste a valid GUID.", "Find Request", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_index != null && _index.TryFindById(id, out var issue))
            {
                // Select in list
                lstIssues.SelectedItem = issue;
                lstIssues.ScrollIntoView(issue);
            }
            else
            {
                MessageBox.Show("No service request found for the specified ID.", "Find Request", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        // Rebuilds indexes and refreshes the visible list to reflect the latest repository state.
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            BuildIndexes();
            LoadIssues();
            MessageBox.Show("Indexes rebuilt and list refreshed.", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
