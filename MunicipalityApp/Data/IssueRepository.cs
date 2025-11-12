using MunicipalityApp.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;

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

        // Returns a thread-safe read-only snapshot list of all issues (in insertion order).
        public static IReadOnlyList<UserIssue> GetAllIssues()
        {
            lock (_lock)
            {
                return _issuesList.ToArray();
            }
        }

        // Persistence file location (AppData\MunicipalityApp\issues.json)
        private static readonly string _dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MunicipalityApp");
        private static readonly string _dataFile = Path.Combine(_dataDir, "issues.json");

        // JSON serializer options
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // Static constructor: attempt to load persisted issues on first access.
        static IssueRepository()
        {
            try
            {
                if (!Directory.Exists(_dataDir)) Directory.CreateDirectory(_dataDir);
                if (File.Exists(_dataFile))
                {
                    var json = File.ReadAllText(_dataFile);
                    var list = JsonSerializer.Deserialize<List<UserIssue>>(json, _jsonOptions);
                    if (list != null && list.Count > 0)
                    {
                        lock (_lock)
                        {
                            _issuesList.Clear();
                            _recentIssues.Clear();
                            _processingQueue.Clear();
                            foreach (var issue in list)
                            {
                                _issuesList.AddLast(issue);
                                _recentIssues.Push(issue);
                                _processingQueue.Enqueue(issue);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"IssueRepository: failed to load persisted issues: {ex}");
                // Swallow -- repository will start empty
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
            // Normalize common fields centrally so all insertion paths are consistent
            issue.Title = NormalizeTitle(issue.Title);
            issue.Category = NormalizeCategory(issue.Category);
            issue.Location = NormalizeLocation(issue.Location);

            lock (_lock)
            {
                _issuesList.AddLast(issue);      // Add to linked list
                _recentIssues.Push(issue);       // Add to stack (most recent first)
                _processingQueue.Enqueue(issue);  // Add to queue (FIFO processing)
            }
            // Notify subscribers that a new issue was added (UI can refresh/filter accordingly)
            IssueAdded?.Invoke(issue);

            // Persist to disk (best-effort)
            try
            {
                SaveToFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"IssueRepository: failed to persist issue: {ex}");
            }
        }

        // Normalize a location string: null/whitespace -> empty, collapse internal whitespace, trim, and Title Case.
        private static string NormalizeLocation(string? location)
        {
            if (string.IsNullOrWhiteSpace(location)) return string.Empty;
            // Trim and collapse multiple whitespace characters into a single space
            var collapsed = Regex.Replace(location.Trim(), "\\s+", " ");
            // Use current culture's Title Case for nicer presentation
            try
            {
                var ti = CultureInfo.CurrentCulture.TextInfo;
                return ti.ToTitleCase(collapsed.ToLower());
            }
            catch
            {
                // In unlikely event TitleCase fails, return the collapsed string
                return collapsed;
            }
        }

        // Normalize title: trim and collapse internal whitespace. Preserve original casing.
        private static string NormalizeTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title)) return string.Empty;
            return Regex.Replace(title.Trim(), "\\s+", " ");
        }

        // Normalize category: trim, collapse whitespace and Title Case for display consistency.
        private static string NormalizeCategory(string? category)
        {
            if (string.IsNullOrWhiteSpace(category)) return string.Empty;
            var collapsed = Regex.Replace(category.Trim(), "\\s+", " ");
            try
            {
                var ti = CultureInfo.CurrentCulture.TextInfo;
                return ti.ToTitleCase(collapsed.ToLower());
            }
            catch
            {
                return collapsed;
            }
        }

        // Event raised after an issue is added. UI pages should subscribe to update lists/filters.
        public static event System.Action<UserIssue>? IssueAdded;
        
        // Event raised after an issue is updated (status/progress changes).
        public static event System.Action<UserIssue>? IssueUpdated;

        // Update the status and optional progress of an existing issue by Id.
        public static bool UpdateIssueStatus(System.Guid id, string status, int? progress = null)
        {
            lock (_lock)
            {
                var node = _issuesList.First;
                while (node != null)
                {
                    if (node.Value.Id == id)
                    {
                        node.Value.Status = status ?? node.Value.Status;
                        if (progress.HasValue) node.Value.Progress = progress.Value;
                        // persist
                        try { SaveToFile(); } catch { }
                        // notify
                        IssueUpdated?.Invoke(node.Value);
                        return true;
                    }
                    node = node.Next;
                }
            }
            return false;
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

        // Persist the current issues list to disk atomically.
        private static void SaveToFile()
        {
            // Take a snapshot under lock to ensure consistency.
            UserIssue[] snapshot;
            lock (_lock)
            {
                snapshot = _issuesList.ToArray();
            }

            // Ensure data directory exists
            if (!Directory.Exists(_dataDir)) Directory.CreateDirectory(_dataDir);

            // Serialize
            var json = JsonSerializer.Serialize(snapshot, _jsonOptions);

            // Write atomically: write to temp file then replace
            var tempFile = _dataFile + ".tmp";
            File.WriteAllText(tempFile, json);
            try
            {
                // If target exists, delete then move to avoid exceptions on older runtimes
                if (File.Exists(_dataFile)) File.Delete(_dataFile);
                File.Move(tempFile, _dataFile);
            }
            catch
            {
                // If atomic replace failed, attempt best-effort copy
                if (File.Exists(tempFile))
                {
                    File.Copy(tempFile, _dataFile, true);
                    File.Delete(tempFile);
                }
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
