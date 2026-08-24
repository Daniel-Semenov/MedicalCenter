using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Authorization.Application.Interfaces;
using Authorization.Domain.Entities;
using Authorization.Infrastructure.Auth;
using Authorization.Infrastructure.Db;
using Authorization.Infrastructure.Repositories;
using MassTransit;
using Messaging.Contracts.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration)
            .AddRepositories()
            .AddJwtTokens(configuration)
            .AddIdentityConfiguration()
            .AddRedis(configuration)
            .AddJwtAuthentication(configuration)
            .AddMessaging(configuration);
        
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

        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
    
    
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
      
        return services;
    }
    
    public static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services)
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
    
   public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtOptions =
                configuration.GetSection(JwtOptions.SectionName)
                    .Get<JwtOptions>()!;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                jwtOptions.SecretKey)),

                    ClockSkew = TimeSpan.Zero
                };

            options.IncludeErrorDetails = true;

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var jti = context.Principal?
                        .FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                    if (string.IsNullOrEmpty(jti))
                    {
                        context.Fail("Token has no jti claim.");
                        return;
                    }

                    var blacklistService = context.HttpContext
                        .RequestServices
                        .GetRequiredService<TokenBlacklistService>();

                    if (await blacklistService.IsBlacklistedAsync(jti))
                    {
                        context.Fail("Token has been revoked.");
                    }
                }
            };
        });

        services.AddAuthorization();

        return services;
    }


    public static IServiceCollection AddRedis(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =
                configuration.GetConnectionString("Redis");
        });

        services.AddScoped<RefreshTokenService>();
        services.AddSingleton<RefreshTokenGenerator>();
        services.AddScoped<TokenBlacklistService>();

        return services;
    }
    private static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(options =>
        {
            configuration.GetSection(RabbitMqOptions.SectionName).Bind(options);
        });

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddEntityFrameworkOutbox<ApplicationDbContext>(outboxConfigurator =>
            {
                outboxConfigurator.UsePostgres();
                outboxConfigurator.UseBusOutbox();
            });

            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                rabbitConfigurator.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitMqOptions.Username);
                    hostConfigurator.Password(rabbitMqOptions.Password);
                });
            });
        });

        return services;
    }
}
