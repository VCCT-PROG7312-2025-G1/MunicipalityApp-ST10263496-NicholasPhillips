using Microsoft.Win32;
using MunicipalityApp.Data;
using MunicipalityApp.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
// Normalization is centralized in IssueRepository

namespace MunicipalityApp
{

    // Interaction logic for ReportIssueWindow.
    // Allows users to report municipal issues, attach media, and submit reports.

    public partial class ReportIssueWindow : Window
    {
        // Stores the file path of the attached media.
        private string attachedFilePath = "";


        // Initializes the window and sets up event handlers for progress tracking.
        public ReportIssueWindow()
        {
            InitializeComponent();

            // Update progress bar when user interacts with input fields.
            txtTitle.TextChanged += (s, e) => UpdateProgressBar();
            txtDescription.TextChanged += (s, e) => UpdateProgressBar();
            cmbCategory.SelectionChanged += (s, e) => UpdateProgressBar();
            cmbLocation.SelectionChanged += (s, e) => UpdateProgressBar();
            cmbLocation.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new TextChangedEventHandler((s,e) => UpdateProgressBar()));
        }


        // Updates the progress bar based on completed input fields.
        // Animates the progress and changes color according to completion.
        private void UpdateProgressBar()
        {
            double progress = 0;
            int totalFields = 5; // Title, Category, Description, Location, File
            int completedFields = 0;

            // Check each field for completion.
            if (!string.IsNullOrWhiteSpace(txtTitle.Text)) completedFields++;
            if (cmbCategory.SelectedItem != null) completedFields++;
            if (!string.IsNullOrWhiteSpace(txtDescription.Text)) completedFields++;
            if (!string.IsNullOrWhiteSpace(cmbLocation?.Text)) completedFields++;
            if (!string.IsNullOrWhiteSpace(attachedFilePath)) completedFields++;

            progress = (completedFields / (double)totalFields) * 100;

            // Animate progress bar value.
            DoubleAnimation animation = new DoubleAnimation
            {
                To = progress,
                Duration = TimeSpan.FromMilliseconds(400), // 0.4 second animation
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            progressBar.BeginAnimation(System.Windows.Controls.Primitives.RangeBase.ValueProperty, animation);

            // Change progress bar color based on progress.
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


        // Handles the Attach Media button click event.
        // Opens a file dialog for the user to select a file and updates the UI.

        private void btnAttachMedia_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*";
            if (dlg.ShowDialog() == true)
            {
                attachedFilePath = dlg.FileName;
                lblFilePath.Text = $"Attached: {attachedFilePath}";
                UpdateProgressBar(); // animate after file attach
            }
        }


        // Handles the Submit Report button click event.
        // Validates input, creates a new issue, saves it, awards points, and resets the form.

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Declare a single toast variable for the method.
            ToastNotification toast;

            // Validate required fields.
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbCategory.SelectedItem == null)
            {
                toast = new ToastNotification("Please fill in all required fields.");
                toast.Show();
                return;
            }

            // Create new issue object. Repository will perform canonical normalization.
            var issue = new UserIssue
            {
                Title = txtTitle.Text,
                Category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty,
                Description = txtDescription.Text,
                FilePath = attachedFilePath,
                Location = cmbLocation?.Text ?? string.Empty,
                Status = "Pending"
            };

            // Save issue in repository.
            IssueRepository.AddIssue(issue);

            // Award points to the user.
            UserRepository.CurrentUser.AddPoints(10);

            // Show success toast notification
            toast = new ToastNotification(
                $"Issue submitted successfully! 🎉\n" +
                $"Request ID: {issue.Id}\n" +
                $"You now have {UserRepository.CurrentUser.Points} points.\n" +
                $"Your badge: {UserRepository.CurrentUser.Badge}"
            );
            toast.Show();

            // Reset form fields.
            txtTitle.Clear();
            txtDescription.Clear();
            cmbCategory.SelectedIndex = -1;
            attachedFilePath = "";
            lblFilePath.Text = "No file selected.";
            UpdateProgressBar(); // reset progress
        }


        // Handles the Back button click event.
        // Returns to the main window and closes the current window.
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MunicipalityApp.Services.NavigationManager.ShowMainWindow();
            this.Close();
        }
    }
}
