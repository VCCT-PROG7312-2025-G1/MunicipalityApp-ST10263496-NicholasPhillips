using MunicipalityApp.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;

namespace MunicipalityApp.Data
{

    // Provides thread-safe storage and management for user-reported issues.
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


        // Gets a thread-safe snapshot of all issues in insertion order.
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
        

        // Gets a thread-safe snapshot of recently added issues (most recent first).
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


        // Adds a new issue to all internal collections.
        public static void AddIssue(UserIssue issue)
        {
            lock (_lock)
            {
                _issuesList.AddLast(issue);      // Add to linked list
                _recentIssues.Push(issue);       // Add to stack (most recent first)
                _processingQueue.Enqueue(issue);  // Add to queue (FIFO processing)
            }
        }


        // Attempts to get the next issue for processing in FIFO order.
        public static bool TryGetNextIssue(out UserIssue? issue)
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
        
        // Gets issues in reverse chronological order (newest first).
        public static IEnumerable<UserIssue> GetIssuesNewestFirst()
        {
            lock (_lock)
            {
                return _issuesList.Reverse().ToArray();
            }
        }
    }
}
