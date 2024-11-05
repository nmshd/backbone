using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Announcements.Application;
using Backbone.Modules.Announcements.Application.Extensions;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Announcements.ConsumerApi;

public class AnnouncementsModule : AbstractModule
{
    public override string Name => "Announcements";

    public override void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

        services.AddApplication();

        services.AddDatabase(dbOptions =>
        {
            dbOptions.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            dbOptions.ConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
        });

        if (parsedConfiguration.Infrastructure.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name,
                parsedConfiguration.Infrastructure.SqlDatabase.Provider,
                parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public override void ConfigureEventBus(IEventBus eventBus)
    {
    }
}
