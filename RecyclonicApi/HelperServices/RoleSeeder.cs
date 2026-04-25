using Microsoft.AspNetCore.Identity;

namespace RecyclonicApi.HelperServices
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var roles = new List<string> { "Admin", "Employee", "Delivery_Lead" , "Delivery", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                }
            }
        }
    }
}
