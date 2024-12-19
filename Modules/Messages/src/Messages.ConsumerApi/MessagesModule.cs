using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Application;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.ConsumerApi;

public class MessagesModule : AbstractModule
{
    public override string Name => "Messages";

    public override void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            options.DbOptions.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
        });

        services.AddApplication();

        if (parsedConfiguration.Infrastructure.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name,
                parsedConfiguration.Infrastructure.SqlDatabase.Provider,
                parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddMessagesDomainEventSubscriptions();
    }
}
