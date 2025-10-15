using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bogus;
using MunicipalityApp.Models;

// PART TWO
namespace MunicipalityApp
{
    public partial class AnnouncementsWindow : Window
    {
        /*
         * Manages municipal announcements and recommendations.
         * Provides search, location filtering, and prioritized suggestions
         * for upcoming events/issues.
         */
        private SortedDictionary<DateTime, List<Event>> eventsByDate = new();
        private HashSet<string> categories = new();
        private HashSet<string> locations = new(); //  Store all locations
        private Queue<Event> upcomingEvents = new();
        private Stack<Event> recentlyViewed = new();
        private List<string> searchHistory = new();
        private PriorityQueue<Event, int> priorityEvents = new();

        /// <summary>
        /// Initializes the window, loads sample data, sets up filters and UI event handlers.
        /// </summary>
        public AnnouncementsWindow()
        {
            InitializeComponent();
            LoadSampleEvents();
            PopulateLocationFilter();
            DisplayEvents();

            // Hook up live search
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Hook up ComboBox selection changed
            cmbLocationFilter.SelectionChanged += cmbLocationFilter_SelectionChanged;

            // Track recently viewed announcements
            lstAnnouncements.SelectionChanged += lstAnnouncements_SelectionChanged;
        }

        /// <summary>
        /// Generates sample events using Bogus and loads them into in-memory data structures.
        /// </summary>
        private void LoadSampleEvents() // Generate sample events using Bogus
        {
            var categoriesList = new[] // Categories of municipal issues
            {
                "Water", "Electricity", "Transport", "Public Safety", "Community"
            };

            var issueTypes = new[] // Types of issues for event titles
            {
                "Water Leak", "Burst Pipe", "Scheduled Maintenance", "Car Accident",
                "Power Outage", "Load Shedding", "Road Repair", "Traffic Lights Faulty",
                "Public Gathering", "Storm Damage", "Tree Removal", "Waste Collection Delay"
            };

            var locationList = new[] // Locations within the municipality
            {
                "Cape Town", "Durban", "Johannesburg", "Pretoria",
                "Port Elizabeth", "Bloemfontein", "East London"
            };

            var faker = new Faker<Event>() // Define rules for generating fake events
                .RuleFor(e => e.Title, f =>
                {
                    var issue = f.PickRandom(issueTypes);
                    var street = f.Address.StreetName();
                    return $"{issue} - {street}";
                })
                .RuleFor(e => e.Category, f => f.PickRandom(categoriesList))
                .RuleFor(e => e.Location, f => f.PickRandom(locationList))
                .RuleFor(e => e.Date, f => f.Date.Between(DateTime.Today, DateTime.Today.AddDays(30)))
                .RuleFor(e => e.Priority, f => f.Random.Int(1, 5));

            var sampleEvents = faker.Generate(50);

            foreach (var ev in sampleEvents)
            {
                if (!eventsByDate.ContainsKey(ev.Date.Date))
                    eventsByDate[ev.Date.Date] = new List<Event>();

                eventsByDate[ev.Date.Date].Add(ev);
                categories.Add(ev.Category);
                locations.Add(ev.Location);
                priorityEvents.Enqueue(ev, ev.Priority);
            }

            // Build the upcoming events queue in chronological order
            RebuildUpcomingQueue();
        }

        // Populate ComboBox with all locations
        /// <summary>
        /// Populates the location ComboBox with all unique locations and selects the default option.
        /// </summary>
        private void PopulateLocationFilter()
        {
            cmbLocationFilter.Items.Clear();
            cmbLocationFilter.Items.Add("All Locations");
            foreach (var loc in locations.OrderBy(l => l))
                cmbLocationFilter.Items.Add(loc);
            cmbLocationFilter.SelectedIndex = 0; // Default selection
        }

        /// <summary>
        /// Displays the provided events in the announcements list. If null, displays all events.
        /// </summary>
        private void DisplayEvents(IEnumerable<Event>? events = null)
        {
            lstAnnouncements.Items.Clear();

            // Use the SortedDictionary order to iterate by ascending date
            var items = events ?? eventsByDate
                .Where(kvp => kvp.Key >= DateTime.Today)
                .SelectMany(kvp => kvp.Value);

            foreach (var ev in items.OrderBy(e => e.Date))
            {
                lstAnnouncements.Items.Add(ev);
            }

            // Keep the upcoming events queue in sync with all events
            RebuildUpcomingQueue();
        }

        // Live search
        /// <summary>
        /// Triggers a search when the user types in the search box.
        /// </summary>
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        // Search button click
        /// <summary>
        /// Executes a search when the Search button is clicked.
        /// </summary>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        // Filter by query and selected location
        /// <summary>
        /// Filters events by the query and selected location, updates the display, and shows recommendations.
        /// </summary>
        private void RunSearch(string query)
        {
            var selectedLocation = cmbLocationFilter.SelectedItem?.ToString();

            // Flatten events using the natural sort order of the SortedDictionary
            var allEvents = eventsByDate
                .Where(kvp => kvp.Key >= DateTime.Today)
                .SelectMany(ev => ev.Value);

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
        /// <summary>
        /// Re-runs the search when the selected location changes.
        /// </summary>
        private void cmbLocationFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        /// <summary>
        /// Builds and displays prioritized recommendations based on the query, search history,
        /// and event priority, excluding items already viewed.
        /// </summary>
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
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Build recommendations
            var filtered = allEvents
                .Where(ev =>
                    // Do not recommend events already viewed
                    !recentlyViewed.Contains(ev) &&

                    // Title does not contain the exact search query to avoid redundancy
                    !ev.Title.Contains(query, StringComparison.OrdinalIgnoreCase) &&

                    // Include events that match query in category OR location OR preferred category
                    (ev.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                     ev.Location.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                     preferredCategories.Contains(ev.Category))
                );

            // Use a temporary PriorityQueue to pick top-priority items efficiently (higher Priority first)
            var tempQueue = new PriorityQueue<Event, int>();
            foreach (var ev in filtered)
            {
                // Negate priority to convert default min-heap into max-priority (5 highest)
                tempQueue.Enqueue(ev, -ev.Priority);
            }

            var top = new List<Event>(capacity: 5);
            while (top.Count < 5 && tempQueue.Count > 0)
            {
                top.Add(tempQueue.Dequeue());
            }

            // Order the selected top items by date for display consistency
            var recommendations = top
                .OrderByDescending(ev => ev.Priority)
                .ThenBy(ev => ev.Date)
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

        private void lstAnnouncements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstAnnouncements.SelectedItem is Event ev)
            {
                // Push the selected event onto the recently viewed stack
                recentlyViewed.Push(ev);
            }
        }

        private void RebuildUpcomingQueue()
        {
            // Rebuild the queue with events in chronological order from the SortedDictionary
            upcomingEvents.Clear();
            foreach (var kvp in eventsByDate)
            {
                foreach (var ev in kvp.Value.OrderBy(e => e.Date))
                {
                    upcomingEvents.Enqueue(ev);
                }
            }
        }

        // Back button click event handler
        /// <summary>
        /// Navigates back to the main window and closes the current window.
        /// </summary>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Open the main window
            MainWindow main = new MainWindow();
            main.Show();

            // Close this AnnouncementsWindow
            this.Close();
        }

    }
}