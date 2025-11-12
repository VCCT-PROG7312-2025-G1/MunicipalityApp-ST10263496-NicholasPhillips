using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bogus;
using MunicipalityApp.Models;
using MunicipalityApp.Data;

// PART TWO
namespace MunicipalityApp
{

    // Displays municipal announcements, supports search and location filtering,
    // and surfaces recommendations using queues, stacks, and a priority queue.
    public partial class AnnouncementsWindow : Window
    {
        
         // Manages municipal announcements and recommendations.
         // Provides location filtering, and prioritized suggestions.
        
        private SortedDictionary<DateTime, List<Event>> eventsByDate = new(); // Sorted by date for chronological operations
        private HashSet<string> categories = new(); // Unique set of categories
        private HashSet<string> locations = new(); // Unique set of locations
        private Queue<Event> upcomingEvents = new(); // FIFO of upcoming events (populated during load)
        private Stack<Event> recentlyViewed = new(); // LIFO of user interactions for exclusion in recommendations
        private List<string> searchHistory = new(); // Raw search terms history
        private PriorityQueue<Event, int> priorityEvents = new(); // Global queue of events by priority


        // Initializes the window, loads sample data, prepares filters, and wires UI events.
        public AnnouncementsWindow()
        {
            InitializeComponent();
            // Use SampleEventService to generate sample data instead of inlining Bogus in the window.
            var sampleSvc = new MunicipalityApp.Services.SampleEventService();
            LoadSampleEvents(sampleSvc.GenerateSampleEvents(50));
            // Also load persisted user-submitted reports so they appear in the announcements list
            // when the window is opened after reports were submitted.
            LoadPersistedUserIssues();
            PopulateLocationFilter();
            DisplayEvents();

            // Subscribe to new user issues so location filters can include reported locations
            IssueRepository.IssueAdded += OnUserIssueAdded;
            this.Closed += (s, e) => IssueRepository.IssueAdded -= OnUserIssueAdded;

            // Hook up live search
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Hook up ComboBox selection changed
            cmbLocationFilter.SelectionChanged += cmbLocationFilter_SelectionChanged;
        }

        // Load persisted user-submitted issues from IssueRepository and convert them to Event entries
        // so that reports saved to disk are visible in the Announcements window.
        private void LoadPersistedUserIssues()
        {
            try
            {
                foreach (var issue in MunicipalityApp.Data.IssueRepository.Issues)
                {
                    var ev = new Event
                    {
                        Title = issue.Title,
                        Description = issue.Description,
                        Date = issue.ReportedDate,
                        Location = issue.Location,
                        Category = issue.Category,
                        Priority = 1
                    };

                    var key = ev.Date.Date;
                    lock (eventsByDate)
                    {
                        if (!eventsByDate.ContainsKey(key)) eventsByDate[key] = new List<Event>();
                        eventsByDate[key].Add(ev);
                    }

                    if (!string.IsNullOrWhiteSpace(ev.Category)) categories.Add(ev.Category);
                    if (!string.IsNullOrWhiteSpace(ev.Location)) locations.Add(ev.Location);
                    upcomingEvents.Enqueue(ev);
                    priorityEvents.Enqueue(ev, ev.Priority);
                }
            }
            catch { }
        }

        private void OnUserIssueAdded(UserIssue issue)
        {
            // Add new location if present and refresh the filter
            if (!string.IsNullOrWhiteSpace(issue.Location))
            {
                if (!locations.Contains(issue.Location))
                {
                    locations.Add(issue.Location);
                    this.Dispatcher?.Invoke(() => PopulateLocationFilter());
                }
            }
            // Also add the submitted issue as a new announcement/event so users can see recent reports
            try
            {
                var ev = new Event
                {
                    Title = issue.Title,
                    Description = issue.Description,
                    Date = issue.ReportedDate,
                    Location = issue.Location,
                    Category = issue.Category,
                    Priority = 1
                };

                // Insert into eventsByDate under the appropriate date
                var key = ev.Date.Date;
                lock (eventsByDate)
                {
                    if (!eventsByDate.ContainsKey(key)) eventsByDate[key] = new List<Event>();
                    eventsByDate[key].Add(ev);
                }

                // Add to auxiliary structures and refresh display on UI thread
                if (!string.IsNullOrWhiteSpace(ev.Category)) categories.Add(ev.Category);
                if (!string.IsNullOrWhiteSpace(ev.Location)) locations.Add(ev.Location);
                upcomingEvents.Enqueue(ev);
                priorityEvents.Enqueue(ev, ev.Priority);

                this.Dispatcher?.BeginInvoke(new Action(() =>
                {
                    // Insert the new announcement at the top of the list for visibility
                    lstAnnouncements.Items.Insert(0, ev);
                }));
            }
            catch { }
        }


        // Populate the in-memory structures from a provided event set.
        private void LoadSampleEvents(IEnumerable<Event> sampleEvents)
        {
            foreach (var ev in sampleEvents)
            {
                if (!eventsByDate.ContainsKey(ev.Date.Date))
                    eventsByDate[ev.Date.Date] = new List<Event>();

                eventsByDate[ev.Date.Date].Add(ev);
                categories.Add(ev.Category);
                locations.Add(ev.Location);
                upcomingEvents.Enqueue(ev); // seed queue; can be rebuilt from eventsByDate if needed
                priorityEvents.Enqueue(ev, ev.Priority);
            }
        }

        // Populate ComboBox with all locations
        // Populates the location filter with unique locations and selects the default option.
        private void PopulateLocationFilter()
        {
            cmbLocationFilter.Items.Clear();
            cmbLocationFilter.Items.Add("All Locations");
            foreach (var loc in locations.OrderBy(l => l))
                cmbLocationFilter.Items.Add(loc);
            cmbLocationFilter.SelectedIndex = 0; // Default selection
        }


        // Displays the provided events, or all events when null. Results are ordered by date.
        private void DisplayEvents(IEnumerable<Event>? events = null)
        {
            lstAnnouncements.Items.Clear();

            var items = events ?? eventsByDate.SelectMany(e => e.Value);

            foreach (var ev in items.OrderBy(e => e.Date))
            {
                lstAnnouncements.Items.Add(ev);
            }
        }

        // Live search
        // Performs live search as the user types.
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        // Search button click
        // Triggers a search using the current query.
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        // Filter by query and selected location
        // Applies text and location filters, updates the results, and records search history.
        private void RunSearch(string query)
        {
            var selectedLocation = cmbLocationFilter.SelectedItem?.ToString();

            var allEvents = eventsByDate.SelectMany(ev => ev.Value);

            // Apply search filter
            var results = allEvents.Where(ev =>
                (string.IsNullOrWhiteSpace(query) ||
                 ev.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                 ev.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                 ev.Location.Contains(query, StringComparison.OrdinalIgnoreCase))
                &&
                (selectedLocation == "All Locations" || ev.Location.Equals(selectedLocation, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            DisplayEvents(results);

            if (!string.IsNullOrWhiteSpace(query))
            {
                searchHistory.Add(query.ToLower());
                ShowRecommendations(query);
            }
            else
            {
                lstRecommendations.Items.Clear();
            }
        }

        // Handle ComboBox location change
        // Re-runs the search whenever the selected location changes.
        private void cmbLocationFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }


        // Builds and displays recommendations using search history (for preferred categories),
        // excludes recently viewed items using a Stack, and orders by priority then date.
        private void ShowRecommendations(string query)
        {
            // Clear previous recommendations
            lstRecommendations.Items.Clear();

            // Flatten all events into a single list
            var allEvents = eventsByDate.SelectMany(ev => ev.Value);

            // Track user's preferred categories based on search history
            var preferredCategories = searchHistory
                .SelectMany(q => allEvents
                    .Where(ev => ev.Category.Contains(q, StringComparison.OrdinalIgnoreCase))
                    .Select(ev => ev.Category))
                .GroupBy(cat => cat)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .ToList(); // Note: could be a HashSet for O(1) Contains during filtering

            // Build recommendations
            var recommendations = allEvents
                .Where(ev =>
                    // Do not recommend events already viewed
                    !recentlyViewed.Contains(ev) &&

                    // Title does not contain the exact search query to avoid redundancy
                    !ev.Title.Contains(query, StringComparison.OrdinalIgnoreCase) &&

                    // Include events that match query in category OR location OR preferred category
                    (ev.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                     ev.Location.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                     preferredCategories.Contains(ev.Category))
                )
                // Sort by priority first, then by upcoming date
                .OrderByDescending(ev => ev.Priority)
                .ThenBy(ev => ev.Date)
                // Limit the number of recommendations to 5 
                .Take(5)
                .ToList();

            // Populate the ListBox with display 
            foreach (var ev in recommendations)
            {
                lstRecommendations.Items.Add(new
                {
                    ev.Title,
                    ev.Location,
                    Date = ev.Date.ToString("dddd, MMM dd")
                });
            }
        }

        // Back button click event handler
        // Navigates back to the main window and closes this window.
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MunicipalityApp.Services.NavigationManager.ShowMainWindow();
            this.Close();
        }

    }
}