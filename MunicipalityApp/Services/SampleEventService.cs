using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using MunicipalityApp.Models;

namespace MunicipalityApp.Services
{
    // Generates sample Event data. Extracted out of the UI for testability and reuse.
    public class SampleEventService
    {
        public IEnumerable<Event> GenerateSampleEvents(int count = 50)
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

            return faker.Generate(count);
        }
    }
}
