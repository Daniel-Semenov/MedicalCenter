using Authorization.Domain.Constants;
using Authorization.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Authorization.Infrastructure.Identity;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
    {
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new ApplicationRole
                    {
                        Name = role
                    });
            }
        }
    }
}