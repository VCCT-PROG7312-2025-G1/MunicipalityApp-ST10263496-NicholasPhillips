using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalityApp.Models
{
    // Represents an issue reported by a user in the municipality application
    public class Issue
    {
        public DateTime ReportedDate { get; private set; } = DateTime.Now;
        // The location where the issue was found or reported
        public string Location { get; set; }

        // The category of the issue (e.g., road, lighting, waste)
        public string Category { get; set; }

        // A detailed description of the issue
        public string Description { get; set; }

        // Points assigned to the issue, used for prioritization or tracking
        public int Points { get; set; } = 10;

        // Current status of the issue (default is "Submitted")
        public string Status { get; set; } = "Submitted";

        // Thread-safe collection of file paths or URLs for attachments related to the issue
        // Using ConcurrentBag for thread-safe unordered collection of attachments
        public ConcurrentBag<string> Attachments { get; private set; } = new ConcurrentBag<string>();
        
        // Stack to track the order of attachment additions (LIFO order)
        private readonly Stack<string> _attachmentStack = new Stack<string>();
        
        // Method to add an attachment to both collections
        public void AddAttachment(string attachmentPath)
        {
            Attachments.Add(attachmentPath);
            lock (_attachmentStack)
            {
                _attachmentStack.Push(attachmentPath);
            }
        }
        
        // Get the most recently added attachment
        public string GetMostRecentAttachment()
        {
            lock (_attachmentStack)
            {
                return _attachmentStack.Count > 0 ? _attachmentStack.Peek() : null;
            }
        }
    }
}
