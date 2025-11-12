using System;
using System.Windows;
using MunicipalityApp.Data;

namespace MunicipalityApp.Services
{
    // A simple application-wide notification manager.
    // Publishes messages to subscribers (e.g. the Message Center window)
    // and shows a toast notification for each published message.
    public static class NotificationManager
    {
        // Event raised when a new message is published.
        public static event Action<string>? MessagePublished;

        static NotificationManager()
        {
            // Subscribe to repository events as an example call-site.
            // When a new issue is added, publish a user-friendly message.
            try
            {
                IssueRepository.IssueAdded += issue =>
                {
                    // Publish a user-friendly message indicating the report is pending
                    var msg = $"Your report '{issue.Title}' has been submitted and is currently {issue.Status}. (ID: {issue.Id})";
                    Publish(msg);
                };
                // Also listen for updates so we can publish resolved notifications
                IssueRepository.IssueUpdated += issue =>
                {
                    try
                    {
                        if (string.Equals(issue.Status, "Completed", StringComparison.OrdinalIgnoreCase) || string.Equals(issue.Status, "Resolved", StringComparison.OrdinalIgnoreCase))
                        {
                            Publish($"Issue resolved: '{issue.Title}' (ID: {issue.Id}). Status: {issue.Status}");
                        }
                        else
                        {
                            Publish($"Issue updated: '{issue.Title}' (ID: {issue.Id}). Status: {issue.Status}");
                        }
                    }
                    catch { }
                };
            }
            catch
            {
                // swallow - IssueRepository may not be available in some test scenarios
            }
        }

        // Publish a message to subscribers and show a toast.
        public static void Publish(string message)
        {
            // Show toast on UI thread if possible
            try
            {
                var app = Application.Current;
                if (app != null)
                {
                    app.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try { new ToastNotification(message).Show(); } catch { }
                    }));
                }
            }
            catch
            {
                // ignore UI failures
            }

            // Notify subscribers (make callback on UI thread when possible)
            try
            {
                var app = Application.Current;
                if (app != null)
                {
                    app.Dispatcher.BeginInvoke(new Action(() => MessagePublished?.Invoke(message)));
                }
                else
                {
                    MessagePublished?.Invoke(message);
                }
            }
            catch
            {
                // best-effort
                try { MessagePublished?.Invoke(message); } catch { }
            }
        }
    }
}
