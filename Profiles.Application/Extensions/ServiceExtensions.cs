using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Profiles.Application.Interfaces.Services;
using Profiles.Application.Services;
using Profiles.Application.Validators.Doctor;

namespace Profiles.Application.Extensions;

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
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IReceptionistService, ReceptionistService>();
        services.AddScoped<IPatientService, PatientService>();

        return services;
    }

    private static IServiceCollection AddFluentValidators(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateDoctorRequestValidator>();
        
        services.AddFluentValidationAutoValidation();

        return services;
    }
}