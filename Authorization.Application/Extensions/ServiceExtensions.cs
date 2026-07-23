using Authorization.Application.Interfaces;
using Authorization.Application.Services;
using Authorization.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationLayer(
        this IServiceCollection services)
    {
        services
            .AddServices()
            .AddFluentValidators();

        return services;
    }

    private static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    private static IServiceCollection AddFluentValidators(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        
        services.AddFluentValidationAutoValidation();

        return services;
    }
}