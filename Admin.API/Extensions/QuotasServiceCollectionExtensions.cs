using Admin.API.Configuration;
using Backbone.Modules.Quotas.Application.Extensions;
using Backbone.Modules.Quotas.Infrastructure.Persistence;

namespace Admin.API.Extensions;

public static class QuotasServiceCollectionExtensions
{
    public static IServiceCollection AddQuotas(this IServiceCollection services,
        QuotasConfiguration configuration)
    {
        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = configuration.Infrastructure.SqlDatabase.Provider;
            options.DbOptions.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;
        });

        services.AddApplication();

        return services;
    }
}