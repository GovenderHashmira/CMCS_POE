using CMCS_POE.Models;
using Microsoft.AspNetCore.Identity;

namespace CMCS_POE.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // Get RoleManager and UserManager from DI
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // 1. Seed Roles
            string[] roles = { "HR", "Lecturer", "Coordinator", "Manager" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Default HR Account
            var defaultHR = await userManager.FindByEmailAsync("hr@cmcs.com");

            if (defaultHR == null)
            {
                var hrUser = new AppUser
                {
                    UserName = "hr@cmcs.com",
                    Email = "hr@cmcs.com",
                    FirstName = "Default",
                    LastName = "HR",
                    HourlyRate = 0,      // HR does not need an hourly rate but can remain 0
                    RoleName = "HR"
                };

                var result = await userManager.CreateAsync(hrUser, "Hr123!@#"); // Default password

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(hrUser, "HR");
                }
                else
                {
                    throw new Exception("Failed to create default HR user: "
                                        + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
