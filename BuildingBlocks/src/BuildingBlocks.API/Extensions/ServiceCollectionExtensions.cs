using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Enmeshed.BuildingBlocks.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static AutofacServiceProvider ToAutofacServiceProvider(this IServiceCollection services)
    {
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        var container = containerBuilder.Build();
        return new AutofacServiceProvider(container);
    }

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
}