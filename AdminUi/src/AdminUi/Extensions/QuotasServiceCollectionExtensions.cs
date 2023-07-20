﻿using AdminUi.Configuration;
using Backbone.Modules.Quotas.Application.Extensions;
using Backbone.Modules.Quotas.Infrastructure.Persistence;

namespace AdminUi.Extensions;

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
