using Microsoft.AspNetCore.Identity;
using CMCS_POE.Models;

namespace CMCS_POE.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "HR", "Lecturer", "Coordinator" };

            // Create roles if missing
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed HR
            if (await userManager.FindByEmailAsync("hr@test.com") == null)
            {
                var hr = new AppUser
                {
                    UserName = "hr@test.com",
                    Email = "hr@test.com",
                    FirstName = "HR",
                    LastName = "User"
                };
                await userManager.CreateAsync(hr, "Password123!");
                await userManager.AddToRoleAsync(hr, "HR");
            }

            // Seed Lecturer
            if (await userManager.FindByEmailAsync("lecturer@test.com") == null)
            {
                var lecturer = new AppUser
                {
                    UserName = "lecturer@test.com",
                    Email = "lecturer@test.com",
                    FirstName = "Lecturer",
                    LastName = "User"
                };
                await userManager.CreateAsync(lecturer, "Password123!");
                await userManager.AddToRoleAsync(lecturer, "Lecturer");
            }

            // Seed Coordinator
            if (await userManager.FindByEmailAsync("coord@test.com") == null)
            {
                var coord = new AppUser
                {
                    UserName = "coord@test.com",
                    Email = "coord@test.com",
                    FirstName = "Coordinator",
                    LastName = "User"
                };
                await userManager.CreateAsync(coord, "Password123!");
                await userManager.AddToRoleAsync(coord, "Coordinator");
            }
        }
    }
}
