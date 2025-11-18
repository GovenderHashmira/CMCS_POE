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

            //  Roles
            string[] roles = { "HR", "Lecturer", "Coordinator", "Manager" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            //  Default HR User
            var defaultHR = await userManager.FindByEmailAsync("hr@cmcs.com");
            if (defaultHR == null)
            {
                var hrUser = new AppUser
                {
                    UserName = "hr@cmcs.com",
                    Email = "hr@cmcs.com",
                    FirstName = "Default",
                    LastName = "HR",
                    HourlyRate = 0,
                    RoleName = "HR" 
                };


                var result = await userManager.CreateAsync(hrUser, "Hr123!@#"); 
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(hrUser, "HR");
                }
            }
        }
    }
}
