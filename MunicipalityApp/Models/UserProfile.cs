using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Models
{

    // Represents a user's profile
    public class UserProfile
    {

    // The user's display name.
    public string UserName { get; set; } = string.Empty;

    // The user's location or area.
    public string Location { get; set; } = string.Empty;
        
        // Accumulated points (used for badges and rewards).
        public int Points { get; set; } = 0;

        // The badge earned by the user based on their current point total.
        public string Badge 
        {
            get
            {
                if (Points >= 100) return "Community Hero";
                if (Points >= 50) return "Active Citizen";
                if (Points >= 20) return "Contributor";
                return "Newcomer";
            }
        }
        // Adds the specified amount of points to the user's total.
        public void AddPoints(int amount)
        {
            Points += amount;
        }
    }
}
