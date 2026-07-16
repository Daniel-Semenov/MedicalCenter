using Authorization.Application.Interfaces;
using Authorization.Domain.Entities;
using Authorization.Infrastructure.Auth;
using Authorization.Infrastructure.Db;
using Authorization.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration)
            .AddRepositories()
            .AddJwtTokens(configuration)
            .AddIdentityConfiguration(configuration);
        
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        
        return services;
    }
    
    private static IServiceCollection AddJwtTokens(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<
            ITokenService,
            JwtTokenService>();

        return services;
    }
    
    
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
      
        return services;
    }
    
    public static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentity<Account, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;

                options.User.RequireUniqueEmail = true;

            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


        return services;
    }
}