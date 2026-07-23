using Authorization.Domain.Constants;
using Authorization.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Infrastructure.Extensions;

public static class ApplicationExtensions
{
    public static async Task<WebApplication> SeedIdentityAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = role
                });
            }
        }

        return app;
    }
}