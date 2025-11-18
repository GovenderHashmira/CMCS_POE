using Microsoft.AspNetCore.Identity;

namespace CMCS_POE.Models
{
    public class AppUser : IdentityUser
    {
        // First Name of the user
        public string FirstName { get; set; }

        // Last Name of the user
        public string LastName { get; set; }

        // Hourly Rate for Lecturers
        public decimal HourlyRate { get; set; }

        // Role name 
        public string? RoleName { get; set; }
    }
}
