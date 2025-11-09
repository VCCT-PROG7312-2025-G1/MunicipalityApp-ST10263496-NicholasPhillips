using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Part 2

namespace MunicipalityApp.Models
{

    // Represents a municipal announcement or community event.
    public class Event
    {

        // Short headline of the event.
        public string Title { get; set; }

        // Detailed description for context.
        public string Description { get; set; }

        // Scheduled date and time of the event.
        public DateTime Date { get; set; }

        // Location where the event applies or will be held.
        public string Location { get; set; }

        // Priority level
        public int Priority { get; set; } // New property for priority level

        // Category of the event
        public string Category { get; internal set; }
    }
}
