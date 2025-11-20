using Microsoft.AspNetCore.Identity;

namespace CMCS_POE.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal HourlyRate { get; set; }

        public string? RoleName { get; set; }
    }
}
