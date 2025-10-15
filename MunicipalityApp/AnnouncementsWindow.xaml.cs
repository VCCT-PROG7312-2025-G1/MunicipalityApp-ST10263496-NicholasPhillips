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
        private SortedDictionary<DateTime, List<Event>> eventsByDate = new();
        private HashSet<string> categories = new();
        private HashSet<string> locations = new(); //  Store all locations
        private Queue<Event> upcomingEvents = new();
        private Stack<Event> recentlyViewed = new();
        private List<string> searchHistory = new();
        private PriorityQueue<Event, int> priorityEvents = new();

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
        }

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
                upcomingEvents.Enqueue(ev);
                priorityEvents.Enqueue(ev, ev.Priority);
            }
        }

        // Populate ComboBox with all locations
        private void PopulateLocationFilter()
        {
            cmbLocationFilter.Items.Clear();
            cmbLocationFilter.Items.Add("All Locations");
            foreach (var loc in locations.OrderBy(l => l))
                cmbLocationFilter.Items.Add(loc);
            cmbLocationFilter.SelectedIndex = 0; // Default selection
        }

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
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        // Search button click
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        // Filter by query and selected location
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
        private void cmbLocationFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

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
                .ToList();

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