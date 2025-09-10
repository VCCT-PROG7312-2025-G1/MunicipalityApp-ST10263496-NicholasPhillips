using MunicipalityApp.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;

namespace MunicipalityApp.Data
{
    /// <summary>
    /// Provides thread-safe storage and management for user-reported issues.
    /// Maintains multiple collections for different access patterns (insertion order, recent, processing).
    /// </summary>
    public static class IssueRepository
    {
        // LinkedList to maintain insertion order while allowing O(1) insertions/deletions.
        private static readonly LinkedList<UserIssue> _issuesList = new LinkedList<UserIssue>();
        
        // Lock object for thread safety when accessing collections.
        private static readonly object _lock = new object();
        
        // Stack for recently added issues (LIFO order, newest on top).
        private static readonly Stack<UserIssue> _recentIssues = new Stack<UserIssue>();
        
        // Queue for processing issues (FIFO order).
        private static readonly Queue<UserIssue> _processingQueue = new Queue<UserIssue>();

        /// <summary>
        /// Gets a thread-safe snapshot of all issues in insertion order.
        /// </summary>
        public static IEnumerable<UserIssue> Issues 
        { 
            get 
            {
                lock (_lock)
                {
                    return _issuesList.ToArray();
                }
            } 
        }
        
        /// <summary>
        /// Gets a thread-safe snapshot of recently added issues (most recent first).
        /// </summary>
        public static IEnumerable<UserIssue> RecentIssues
        {
            get
            {
                lock (_lock)
                {
                    return _recentIssues.ToArray();
                }
            }
        }

        /// <summary>
        /// Adds a new issue to all internal collections.
        /// </summary>
        /// <param name="issue">The issue to add.</param>
        public static void AddIssue(UserIssue issue)
        {
            lock (_lock)
            {
                _issuesList.AddLast(issue);      // Add to linked list
                _recentIssues.Push(issue);       // Add to stack (most recent first)
                _processingQueue.Enqueue(issue);  // Add to queue (FIFO processing)
            }
        }

        /// <summary>
        /// Attempts to get the next issue for processing in FIFO order.
        /// </summary>
        /// <param name="issue">The next issue, if available.</param>
        /// <returns>True if an issue was dequeued; otherwise, false.</returns>
        public static bool TryGetNextIssue(out UserIssue issue)
        {
            lock (_lock)
            {
                if (_processingQueue.Count > 0)
                {
                    issue = _processingQueue.Dequeue();
                    return true;
                }
                issue = null;
                return false;
            }
        }
        
        /// <summary>
        /// Gets issues in reverse chronological order (newest first).
        /// </summary>
        /// <returns>An enumerable of issues, newest first.</returns>
        public static IEnumerable<UserIssue> GetIssuesNewestFirst()
        {
            lock (_lock)
            {
                return _issuesList.Reverse().ToArray();
            }
        }
    }
}
