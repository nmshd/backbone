﻿using Backbone.Modules.Files.Application;
using Backbone.Modules.Files.Application.Extensions;
using Backbone.Modules.Files.Infrastructure.Persistence;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.Tooling.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IStartup = Enmeshed.BuildingBlocks.API.IStartup;

namespace Files.ConsumerApi;

public class Startup : IStartup
{
    public void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

        services.AddApplication();

        services.AddPersistence(options =>
        {
            options.DbOptions.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            options.DbOptions.DbConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;

            options.BlobStorageOptions.ConnectionInfo = parsedConfiguration.Infrastructure.BlobStorage.ConnectionInfo;
            options.BlobStorageOptions.CloudProvider = parsedConfiguration.Infrastructure.BlobStorage.CloudProvider;
            options.BlobStorageOptions.Container =
                parsedConfiguration.Infrastructure.BlobStorage.ContainerName.IsNullOrEmpty()
                    ? "files"
                    : parsedConfiguration.Infrastructure.BlobStorage.ContainerName;
        });

        services.AddSqlDatabaseHealthCheck("Files", parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public void Configure(WebApplication app)
    {
    }

    public void ConfigureEventBus(IEventBus eventBus)
    {
    }
}
