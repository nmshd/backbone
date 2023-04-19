using Backbone.API.Configuration;
using Backbone.Modules.Quotas.Infrastructure.Persistence;
using Backbone.Modules.Quotas.Application.Extensions;

namespace Backbone.API.Extensions;

public static class QuotasServiceCollectionExtensions
{
    public static IServiceCollection AddQuotas(this IServiceCollection services,
       QuotasConfiguration configuration)
    {
        services.AddApplication();

        services.AddDatabase(dbOptions =>
        {
            dbOptions.Provider = configuration.Infrastructure.SqlDatabase.Provider;
            dbOptions.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;
        });

        return services;
    }
}
