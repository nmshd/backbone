﻿using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Infrastructure.Persistence;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.ConsumerApi;

public class RelationshipsModule : AbstractModule
{
    public override string Name => "Relationships";

    public override void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

        services.AddApplication();

        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            options.DbOptions.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;

            options.BlobStorageOptions.CloudProvider = parsedConfiguration.Infrastructure.BlobStorage.CloudProvider;
            options.BlobStorageOptions.ConnectionInfo = parsedConfiguration.Infrastructure.BlobStorage.ConnectionInfo;
            options.BlobStorageOptions.Container =
                parsedConfiguration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                    ? "relationships"
                    : parsedConfiguration.Infrastructure.BlobStorage.ContainerName;
        });

        services.AddSqlDatabaseHealthCheck(Name, parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public override void ConfigureEventBus(IEventBus eventBus)
    {
    }
}
