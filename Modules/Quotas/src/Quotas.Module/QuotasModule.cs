using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Quotas.Application;
using Backbone.Modules.Quotas.Application.Extensions;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Quotas.Module;

public class QuotasModule : AbstractModule
{
    public override string Name => "Quotas";

    public override void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

        services.AddApplication();

        services.AddDatabase(dbOptions =>
        {
            dbOptions.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            dbOptions.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
        });

        if (parsedConfiguration.Infrastructure.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name,
                parsedConfiguration.Infrastructure.SqlDatabase.Provider,
                parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);

        services.AddResponseCaching();
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddQuotasDomainEventSubscriptions();
    }
}
