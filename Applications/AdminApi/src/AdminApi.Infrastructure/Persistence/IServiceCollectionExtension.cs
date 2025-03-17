using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.AdminApi.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    private const string SQLSERVER_MIGRATIONS_ASSEMBLY = "Backbone.AdminApi.Infrastructure.Database.SqlServer";
    private const string POSTGRES_MIGRATIONS_ASSEMBLY = "Backbone.AdminApi.Infrastructure.Database.Postgres";

    public static IServiceCollection AddDatabase(this IServiceCollection services, DatabaseConfiguration configuration)
    {
        services.AddDbContext<AdminApiDbContext>(configuration,
            p => p switch
            {
                BuildingBlocks.Infrastructure.Persistence.Database.IServiceCollectionExtensions.SQLSERVER => SQLSERVER_MIGRATIONS_ASSEMBLY,
                BuildingBlocks.Infrastructure.Persistence.Database.IServiceCollectionExtensions.POSTGRES => POSTGRES_MIGRATIONS_ASSEMBLY,
                _ => throw new Exception($"Unsupported database provider for Admin API")
            }, "AdminUi");

        return services;
    }
}
