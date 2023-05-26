using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enmeshed.BuildingBlocks.API.Extensions;

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
        where TModule : IModule, new()
    {
        // Register assembly in MVC so it can find controllers of the module
        services.AddControllers().ConfigureApplicationPartManager(manager =>
            manager.ApplicationParts.Add(new AssemblyPart(typeof(TModule).Assembly)));

        var module = new TModule();

        var moduleConfiguration = configuration.GetSection($"Modules:{module.Name}");

        module.ConfigureServices(services, moduleConfiguration);

        services.AddSingleton<IModule>(module);

        return services;
    }
}