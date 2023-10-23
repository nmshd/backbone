using Backbone.AdminUi.Configuration;
using Backbone.Quotas.Application.Extensions;
using Backbone.Quotas.Infrastructure.Persistence.Database;

namespace Backbone.AdminUi.Extensions;

public static class QuotasServiceCollectionExtensions
{
    public static IServiceCollection AddQuotas(this IServiceCollection services,
        QuotasConfiguration configuration)
    {
        services.AddApplication();

        services.AddDatabase(options =>
        {
            options.DbConnectionString = configuration.Infrastructure.SqlDatabase.ConnectionString;
            options.Provider = configuration.Infrastructure.SqlDatabase.Provider;
        });


        return services;
    }
}
