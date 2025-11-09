using MunicipalityApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Data
{

    // Repository for managing the current user's profile across the application lifetime.
    class UserRepository
    {

        // The profile of the currently logged-in user, available application-wide.
        public static UserProfile CurrentUser { get; set; } = new UserProfile();
    }
}
