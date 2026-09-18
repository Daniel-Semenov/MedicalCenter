using Authorization.Application.Behaviours;
using Authorization.Application.Interfaces;
using Authorization.Application.Mapping;
using Authorization.Application.Services;
using Authorization.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Authorization.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services
            .AddServices()
            .AddFluentValidators();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
            cfg.AddOpenBehavior(typeof(ResultValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(UnitValidationBehavior<,>));
        });

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountResponseFactory, AccountResponseFactory>();

        return services;
    }

    private static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.AddFluentValidationAutoValidation();

        return services;
    }
}