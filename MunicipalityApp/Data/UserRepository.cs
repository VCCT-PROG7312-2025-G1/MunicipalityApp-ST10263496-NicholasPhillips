using MunicipalityApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Data
{
    // Repository class for managing the current user's profile
    class UserRepository
    {
        // Stores the profile of the currently logged-in user
        // Accessible throughout the application as a static property
        public static UserProfile CurrentUser { get; set; } = new UserProfile();
    }
}
