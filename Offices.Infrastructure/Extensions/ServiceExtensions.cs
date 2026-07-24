using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Offices.Application.Interfaces;
using Offices.Infrastructure.Db;
using Offices.Infrastructure.Repositories;
using Offices.Infrastructure.Services;

namespace Offices.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        OfficeConfiguration.Configure();

        var connectionString = configuration.GetConnectionString("MongoDb");
        
        var databaseName = configuration["MongoDb:DatabaseName"] ?? "OfficesDb";

        services.AddSingleton(sp => new MongoDbContext(connectionString, databaseName));

        services.AddScoped<IOfficeRepository, OfficeRepository>();

        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}