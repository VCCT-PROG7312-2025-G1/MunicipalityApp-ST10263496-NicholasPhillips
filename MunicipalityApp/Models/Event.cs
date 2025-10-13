using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Part 2

namespace MunicipalityApp.Models
{
    public class Event
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int Priority { get; set; } // New property for priority level
        public string Category { get; internal set; }
    }
}
