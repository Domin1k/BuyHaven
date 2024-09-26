namespace BuyHaven.Identity.Services.Identity
{
    using BuyHaven.Common.Infrastructure;
    using BuyHaven.Common.Services;
    using BuyHaven.Identity.Data;
    using Microsoft.AspNetCore.Identity;

    public class IdentityDataSeeder : IDataSeeder
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public IdentityDataSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task SeedData()
        {
            if (roleManager.Roles.Any())
            {
                return;
            }

            var adminRole = new IdentityRole(InfrastructureConstants.AuthConstants.AdministratorRoleName);

            await roleManager.CreateAsync(adminRole);

            var adminUser = new User
            {
                UserName = "admin@mysite.com",
                Email = "admin@mysite.com",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            
            // TODO move to secret
            await userManager.CreateAsync(adminUser, "123456");

            await userManager.AddToRoleAsync(adminUser, InfrastructureConstants.AuthConstants.AdministratorRoleName);
        }
    }
}
