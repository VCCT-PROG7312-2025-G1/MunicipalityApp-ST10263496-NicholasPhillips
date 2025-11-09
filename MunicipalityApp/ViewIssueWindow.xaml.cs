using MunicipalityApp.Data;
using MunicipalityApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MunicipalityApp
{

    // Displays reported issues in a grid with search by ID and index refresh.
    public partial class ViewIssueWindow : Window
    {
        private ServiceRequestIndex _index;
        public ViewIssueWindow()
        {
            InitializeComponent();
            BuildIndexes();
            LoadIssues();
        }


        // Populates the DataGrid with the latest issues (newest first).

        private void LoadIssues()
        {
            dgIssues.ItemsSource = IssueRepository.GetIssuesNewestFirst().ToArray();
        }


        // Builds advanced indices over the repository to support fast lookups.
        private void BuildIndexes()
        {
            _index = new ServiceRequestIndex(IssueRepository.Issues);
        }


        // Searches for a request by GUID and selects it in the grid if found.
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
                dgIssues.SelectedItem = issue;
                dgIssues.ScrollIntoView(issue);
            }
            else
            {
                MessageBox.Show("No service request found for the specified ID.", "Find Request", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Rebuilds indexes and refreshes grid data.
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            BuildIndexes();
            LoadIssues();
            MessageBox.Show("Indexes rebuilt and grid refreshed.", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
