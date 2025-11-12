using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MunicipalityApp.Models;
using MunicipalityApp.Services;
using MunicipalityApp.Data;

namespace MunicipalityApp.Windows
{
    public partial class ServiceRequestStatusWindow : Window
    {
        private ServiceRequestIndex? _index;

        public ServiceRequestStatusWindow()
        {
            InitializeComponent();
            BuildIndexes();
            LoadAll();
            UpdateStats();
            IssueRepository.IssueAdded += OnIssueAdded;
            this.Closed += (s, e) => IssueRepository.IssueAdded -= OnIssueAdded;
        }

        private void BuildIndexes()
        {
            _index = new ServiceRequestIndex(IssueRepository.Issues);
        }

        private void LoadAll()
        {
            if (_index == null) BuildIndexes();
            var list = _index!.InOrderByReportedDate().Reverse().ToList();
            dgIssues.ItemsSource = list;
        }

        private void UpdateStats()
        {
            int total = IssueRepository.Issues.Count();
            int shown = (dgIssues.ItemsSource as IEnumerable<UserIssue>)?.Count() ?? 0;
            txtStats.Text = $"Total requests: {total}\nDisplayed: {shown}";
        }

        // Persist inline edits to the Status column. When a cell edit is committed, update the repository
        // so changes are saved and IssueUpdated is raised (NotificationManager will handle notifications).
        private void dgIssues_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            // The edited row's data item
            if (e.Row?.Item is not UserIssue edited) return;

            // At this point the binding may already have updated the 'edited.Status' property.
            // Persist the status (and progress if present) to the repository.
            try
            {
                var ok = IssueRepository.UpdateIssueStatus(edited.Id, edited.Status ?? string.Empty, edited.Progress);
                if (ok)
                {
                    // Refresh indexes and UI to reflect persisted changes
                    BuildIndexes();
                    LoadAll();
                    UpdateStats();
                    try
                    {
                        // Show a small non-blocking confirmation toast for the inline edit.
                        new ToastNotification($"Status updated: '{edited.Status}' for '{edited.Title}'").Show();
                    }
                    catch { }
                }
                else
                {
                    MessageBox.Show("Failed to update issue status.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            BuildIndexes();
            LoadAll();
            UpdateStats();
            MessageBox.Show("Indexes rebuilt and list refreshed.", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnIssueAdded(UserIssue issue)
        {
            this.Dispatcher?.Invoke(() =>
            {
                BuildIndexes();
                LoadAll();
                UpdateStats();
            });
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            var text = txtSearchId.Text?.Trim();
            if (string.IsNullOrEmpty(text)) { MessageBox.Show("Please enter a GUID or search text."); return; }
            if (Guid.TryParse(text, out var id))
            {
                if (_index != null && _index.TryFindById(id, out var issue))
                {
                    dgIssues.SelectedItem = issue;
                    dgIssues.ScrollIntoView(issue);
                    MessageBox.Show($"Found: {issue.Title}\nStatus: {issue.Status}\nProgress: {issue.Progress}%", "Issue Found");
                }
                else MessageBox.Show("Issue not found.", "Lookup");
            }
            else
            {
                // Partial title search
                var matches = (_index?.SmallestProgressFirst(int.MaxValue) ?? Enumerable.Empty<UserIssue>())
                              .Where(u => !string.IsNullOrEmpty(u.Title) && u.Title.Contains(text, StringComparison.OrdinalIgnoreCase)).ToArray();
                if (matches.Length == 0) MessageBox.Show("No matches for the provided text.", "Lookup");
                else
                {
                    dgIssues.ItemsSource = matches;
                    MessageBox.Show($"Found {matches.Length} matches - showing in grid.", "Lookup");
                }
            }
            UpdateStats();
        }

        private void BtnNextPriority_Click(object sender, RoutedEventArgs e)
        {
            if (_index == null) { MessageBox.Show("No index available."); return; }
            var list = _index.SmallestProgressFirst(1).ToArray();
            if (list.Length == 0) MessageBox.Show("No items in priority queue.");
            else
            {
                var next = list[0];
                MessageBox.Show($"Next by priority: {next.Title}\nProgress: {next.Progress}%\nID: {next.Id}");
            }
        }

        private void BtnShowMst_Click(object sender, RoutedEventArgs e)
        {
            if (_index == null) { MessageBox.Show("No index available."); return; }
            lstMst.Items.Clear();
            var edges = _index.CategoryMst().ToArray();
            if (!edges.Any()) { lstMst.Items.Add("No category MST available."); return; }
            foreach (var edge in edges)
            {
                lstMst.Items.Add($"{edge.u} ⇄ {edge.v} (w={edge.w})");
            }
        }

        private void BtnShowBfs_Click(object sender, RoutedEventArgs e)
        {
            if (_index == null) { MessageBox.Show("No index available."); return; }
            lstMst.Items.Clear();
            var nodes = _index.CategoryBfs().ToArray();
            if (!nodes.Any()) { lstMst.Items.Add("No category graph nodes."); return; }
            foreach (var n in nodes) lstMst.Items.Add(n);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Close this window — NavigationService registers a handler to show the main window again.
            this.Close();
        }
    }
}
