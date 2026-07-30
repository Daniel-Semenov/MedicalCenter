using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Application.Interfaces;
using Services.Infrastructure.Data;
using Services.Infrastructure.Repositories;

namespace Services.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();

        return services;
    }
}