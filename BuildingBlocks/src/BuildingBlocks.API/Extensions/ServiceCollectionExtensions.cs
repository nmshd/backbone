﻿using Backbone.BuildingBlocks.API.AspNetCoreIdentityCustomizations;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backbone.BuildingBlocks.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSqlDatabaseHealthCheck(this IServiceCollection services, string name, string provider,
        string connectionString)
    {
        switch (provider)
        {
            case "SqlServer":
                services.AddHealthChecks().AddSqlServer(
                    connectionString,
                    name: name
                );
                break;
            case "Postgres":
                services.AddHealthChecks().AddNpgSql(
                    npgsqlConnectionString: connectionString,
                    name: name);
                break;
            default:
                throw new Exception($"Unsupported database provider: {provider}");
        }
    }

    public static IServiceCollection AddModule<TModule>(this IServiceCollection services, IConfiguration configuration)
        where TModule : AbstractModule, new()
    {
        // Register assembly in MVC so it can find controllers of the module
        services.AddControllers().ConfigureApplicationPartManager(manager =>
            manager.ApplicationParts.Add(new AssemblyPart(typeof(TModule).Assembly)));

        var module = new TModule();

        var moduleConfiguration = configuration.GetSection($"Modules:{module.Name}");

        module.ConfigureServices(services, moduleConfiguration);

        services.AddSingleton<AbstractModule>(module);

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
            }
            else
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
            }
        })
        .AddEntityFrameworkStores<DevicesDbContext>()
        .AddSignInManager<CustomSigninManager>()
        .AddUserStore<CustomUserStore>();

        services.AddScoped<ILookupNormalizer, CustomLookupNormalizer>();

        return services;
    }
}
