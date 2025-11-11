using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Bogus;
using MunicipalityApp.Models;

namespace MunicipalityApp.Pages
{
    public partial class AnnouncementsPage : Page
    {
        private SortedDictionary<DateTime, List<Event>> eventsByDate = new();
        private HashSet<string> categories = new();
        private HashSet<string> locations = new();
        private Queue<Event> upcomingEvents = new();
        private Stack<Event> recentlyViewed = new();
        private List<string> searchHistory = new();
        private PriorityQueue<Event, int> priorityEvents = new();

        public AnnouncementsPage()
        {
            InitializeComponent();
            LoadSampleEvents();
            PopulateLocationFilter();
            DisplayEvents();

            txtSearch.TextChanged += TxtSearch_TextChanged;
            cmbLocationFilter.SelectionChanged += cmbLocationFilter_SelectionChanged;
        }

        private void LoadSampleEvents()
        {
            var categoriesList = new[] { "Water", "Electricity", "Transport", "Public Safety", "Community" };
            var issueTypes = new[] { "Water Leak", "Burst Pipe", "Scheduled Maintenance", "Car Accident", "Power Outage", "Load Shedding", "Road Repair", "Traffic Lights Faulty", "Public Gathering", "Storm Damage", "Tree Removal", "Waste Collection Delay" };
            var locationList = new[] { "Cape Town", "Durban", "Johannesburg", "Pretoria", "Port Elizabeth", "Bloemfontein", "East London" };

            var faker = new Faker<Event>()
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

        private void PopulateLocationFilter()
        {
            cmbLocationFilter.Items.Clear();
            cmbLocationFilter.Items.Add("All Locations");
            foreach (var loc in locations.OrderBy(l => l))
                cmbLocationFilter.Items.Add(loc);
            cmbLocationFilter.SelectedIndex = 0;
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

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        private void RunSearch(string query)
        {
            var selectedLocation = cmbLocationFilter.SelectedItem?.ToString();
            var allEvents = eventsByDate.SelectMany(ev => ev.Value);

            var results = allEvents.Where(ev =>
                (string.IsNullOrWhiteSpace(query) || ev.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || ev.Category.Contains(query, StringComparison.OrdinalIgnoreCase) || ev.Location.Contains(query, StringComparison.OrdinalIgnoreCase))
                && (selectedLocation == "All Locations" || ev.Location.Equals(selectedLocation, StringComparison.OrdinalIgnoreCase))
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

        private void cmbLocationFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RunSearch(txtSearch.Text.Trim());
        }

        private void ShowRecommendations(string query)
        {
            lstRecommendations.Items.Clear();

            var allEvents = eventsByDate.SelectMany(ev => ev.Value);

            var preferredCategories = searchHistory
                .SelectMany(q => allEvents.Where(ev => ev.Category.Contains(q, StringComparison.OrdinalIgnoreCase)).Select(ev => ev.Category))
                .GroupBy(cat => cat)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .ToList();

            var recommendations = allEvents
                .Where(ev => !recentlyViewed.Contains(ev) && !ev.Title.Contains(query, StringComparison.OrdinalIgnoreCase) && (ev.Category.Contains(query, StringComparison.OrdinalIgnoreCase) || ev.Location.Contains(query, StringComparison.OrdinalIgnoreCase) || preferredCategories.Contains(ev.Category)))
                .OrderByDescending(ev => ev.Priority)
                .ThenBy(ev => ev.Date)
                .Take(5)
                .ToList();

            foreach (var ev in recommendations)
            {
                lstRecommendations.Items.Add(new { ev.Title, ev.Location, Date = ev.Date.ToString("dddd, MMM dd") });
            }
        }

        private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new MainHomePage());
        }
    }
}
