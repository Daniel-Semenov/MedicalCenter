using Authorization.Application.Interfaces;
using Authorization.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddServices();
        
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
 
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}