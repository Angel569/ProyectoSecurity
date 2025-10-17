using Galaxy.Security.Infraestructure.Configurations.IdentityEntities;
using Galaxy.Security.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace Galaxy.Security.Infraestructure.Configurations.IdentitySeed
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<UserExtension>>();
            var rolManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed Roles
            var supervisor = new IdentityRole(RolesConstants.SupervisorRole);
            var customer = new IdentityRole(RolesConstants.CustomerRole);
            if (!await rolManager.RoleExistsAsync(RolesConstants.SupervisorRole))
            {
                await rolManager.CreateAsync(supervisor);
            }
            if (!await rolManager.RoleExistsAsync(RolesConstants.CustomerRole))
            {
                await rolManager.CreateAsync(customer);
            }

            //Seed default user Supervisor
            var supervisorUser = new UserExtension
            {
                FullName = "Luis Alcantara",
                UserName = "admin",
                Email = "alcantaraangel712@gmail.com",
                EmailConfirmed = true,
                UserId = new("4fa2b6f5-1c3b-4e2e-8f0a-2c3b5f6e7d8a")
            };

            var result = await userManager.CreateAsync(supervisorUser, "Password2025");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(supervisorUser, RolesConstants.SupervisorRole);
            }
        }
    }
}
