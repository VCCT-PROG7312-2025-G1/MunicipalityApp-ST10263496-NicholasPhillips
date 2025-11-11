using MunicipalityApp.Data;
using MunicipalityApp.Models;
using MunicipalityApp.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MunicipalityApp.Pages
{
    public partial class StatusReportPage : Page
    {
    private ServiceRequestIndex? _index;

        public StatusReportPage()
        {
            InitializeComponent();
            BuildIndexes();
            LoadIssues();
        }

        private void LoadIssues()
        {
            var issues = IssueRepository.GetIssuesNewestFirst().ToArray();
            lstIssues.ItemsSource = issues;
        }

        private void BuildIndexes()
        {
            _index = new ServiceRequestIndex(IssueRepository.Issues);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new MainHomePage());
        }

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
                lstIssues.SelectedItem = issue;
                lstIssues.ScrollIntoView(issue);
            }
            else
            {
                MessageBox.Show("No service request found for the specified ID.", "Find Request", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            BuildIndexes();
            LoadIssues();
            MessageBox.Show("Indexes rebuilt and list refreshed.", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
