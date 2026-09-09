using Documents.Application.Interfaces;
using Documents.Application.Interfaces.Repositories;
using Documents.Infrastructure.Db;
using Documents.Infrastructure.Generators;
using Documents.Infrastructure.Repositories;
using Documents.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Documents.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DocumentsDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.Configure<MinioOptions>(
            configuration.GetSection(MinioOptions.SectionName));

        var minioOptions = configuration
                               .GetSection(MinioOptions.SectionName)
                               .Get<MinioOptions>()
                           ?? throw new InvalidOperationException(
                               "MinIO configuration is missing.");

        services.AddSingleton<IMinioClient>(_ =>
            new MinioClient()
                .WithEndpoint(minioOptions.Endpoint)
                .WithCredentials(
                    minioOptions.AccessKey,
                    minioOptions.SecretKey)
                .WithSSL(minioOptions.UseSsl)
                .Build());

        services.AddSingleton<IObjectStorageOptions>(
            new ObjectStorageOptions(
                minioOptions.BucketName));

        services.AddScoped<IDocumentRepository, DocumentRepository>();

        services.AddScoped<IObjectStorage, MinioObjectStorage>();

        services.AddSingleton<IObjectKeyGenerator, ObjectKeyGenerator>();

        services.AddSingleton<MinioBucketInitializer>();

        return services;
    }
}