using Backbone.BuildingBlocks.API.AspNetCoreIdentityCustomizations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;

namespace Backbone.BuildingBlocks.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSqlDatabaseHealthCheck(this IServiceCollection services, string name, string provider,
        string connectionString)
    {
        switch (provider)
        {
            case DatabaseConfiguration.SQLSERVER:
                services.AddHealthChecks().AddSqlServer(
                    connectionString,
                    name: $"{name}Database"
                );
                break;
            case DatabaseConfiguration.POSTGRES:
                services.AddHealthChecks().AddNpgSql(
                    connectionString: connectionString,
                    name: $"{name}Database"
                );
                break;
            default:
                throw new Exception($"Unsupported database provider: {provider}");
        }
    }

    public static IServiceCollection AddModule<TModule, TApplicationConfiguration, TInfrastructureConfiguration>(this IServiceCollection services, IConfiguration configuration)
        where TModule : AbstractModule<TApplicationConfiguration, TInfrastructureConfiguration>, new()
        where TApplicationConfiguration : class
        where TInfrastructureConfiguration : class
    {
        var module = new TModule();

        var moduleConfiguration = configuration.GetSection($"Modules:{module.Name}");

        module.ConfigureServices(services, moduleConfiguration, configuration.GetSection("ModuleDefaults"));

        services.AddSingleton<IEventBusConfigurator>(module);

        return services;
    }

    public static IServiceCollection AddCustomIdentity(this IServiceCollection services, IHostEnvironment environment)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                if (environment.IsDevelopment() || environment.IsLocal())
                {
                    options.Password.RequiredLength = 1;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;

                    options.User.AllowedUserNameCharacters += " ";

                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                }
                else
                {
                    options.Password.RequiredLength = 10;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;

                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                }
            })
            .AddEntityFrameworkStores<DevicesDbContext>()
            .AddSignInManager<CustomSigninManager>()
            .AddUserStore<CustomUserStore>();

        services.AddScoped<ILookupNormalizer, CustomLookupNormalizer>();

        return services;
    }

    public static IServiceCollection AddOpenTelemetryWithPrometheusExporter(this IServiceCollection services, string name)
    {
        services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddPrometheusExporter()
                    .AddMeter(name)
                    .AddMeter("Microsoft.EntityFrameworkCore")
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Diagnostics")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel");
            });

        return services;
    }
}
