using Microsoft.Win32;
using MunicipalityApp.Data;
using MunicipalityApp.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MunicipalityApp.Pages
{
    public partial class ReportIssuePage : Page
    {
        private string attachedFilePath = "";

        public ReportIssuePage()
        {
            InitializeComponent();

            // Update progress bar when user interacts with input fields.
            txtTitle.TextChanged += (s, e) => UpdateProgressBar();
            txtDescription.TextChanged += (s, e) => UpdateProgressBar();
            cmbCategory.SelectionChanged += (s, e) => UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            double progress = 0;
            int totalFields = 4; // Title, Category, Description, File
            int completedFields = 0;

            if (!string.IsNullOrWhiteSpace(txtTitle.Text)) completedFields++;
            if (cmbCategory.SelectedItem != null) completedFields++;
            if (!string.IsNullOrWhiteSpace(txtDescription.Text)) completedFields++;
            if (!string.IsNullOrWhiteSpace(attachedFilePath)) completedFields++;

            progress = (completedFields / (double)totalFields) * 100;

            DoubleAnimation animation = new DoubleAnimation
            {
                To = progress,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            progressBar.BeginAnimation(System.Windows.Controls.Primitives.RangeBase.ValueProperty, animation);

            if (progress <= 33)
            {
                progressBar.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (progress <= 66)
            {
                progressBar.Foreground = new SolidColorBrush(Colors.Orange);
            }
            else
            {
                progressBar.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        private void btnAttachMedia_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*";
            if (dlg.ShowDialog() == true)
            {
                attachedFilePath = dlg.FileName;
                lblFilePath.Text = $"Attached: {attachedFilePath}";
                UpdateProgressBar();
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            ToastNotification toast;

            if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbCategory.SelectedItem == null)
            {
                toast = new ToastNotification("Please fill in all required fields.");
                toast.Show();
                return;
            }

            var issue = new UserIssue
            {
                Title = txtTitle.Text,
                Category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty,
                Description = txtDescription.Text,
                FilePath = attachedFilePath,
                Status = "Pending"
            };

            IssueRepository.AddIssue(issue);
            UserRepository.CurrentUser.AddPoints(10);

            toast = new ToastNotification(
                $"Issue submitted successfully! 🎉\n" +
                $"Request ID: {issue.Id}\n" +
                $"You now have {UserRepository.CurrentUser.Points} points.\n" +
                $"Your badge: {UserRepository.CurrentUser.Badge}"
            );
            toast.Show();

            txtTitle.Clear();
            txtDescription.Clear();
            cmbCategory.SelectedIndex = -1;
            attachedFilePath = "";
            lblFilePath.Text = "No file selected.";
            UpdateProgressBar();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main home page in the hosting Frame
            this.NavigationService?.Navigate(new MainHomePage());
        }
    }
}
