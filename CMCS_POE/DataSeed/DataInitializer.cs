using CMCS_POE.Models;
using Microsoft.AspNetCore.Identity;

namespace CMCS_POE.DataSeed
{
    public static class DataInitializer
    {
        public static async Task SeedUsersAndRolesAsync(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "HR", "Lecturer", "Coordinator" };

            // Ensure all roles exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed HR user
            if (await userManager.FindByEmailAsync("hr@test.com") == null)
            {
                var user = new AppUser
                {
                    FirstName = "HR",
                    LastName = "User",
                    Email = "hr@test.com",
                    UserName = "hr@test.com"
                };

                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, "HR");
            }

            // Seed Lecturer user
            if (await userManager.FindByEmailAsync("lecturer@test.com") == null)
            {
                var user = new AppUser
                {
                    FirstName = "Lecturer",
                    LastName = "User",
                    Email = "lecturer@test.com",
                    UserName = "lecturer@test.com"
                };

                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, "Lecturer");
            }

            // Seed Coordinator user
            if (await userManager.FindByEmailAsync("coordinator@test.com") == null)
            {
                var user = new AppUser
                {
                    FirstName = "Coordinator",
                    LastName = "Manager",
                    Email = "coordinator@test.com",
                    UserName = "coordinator@test.com"
                };

                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, "Coordinator");
            }
        }
    }
}
