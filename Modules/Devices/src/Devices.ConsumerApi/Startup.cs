﻿using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Devices.ConsumerApi;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.Crypto.Abstractions;
using Enmeshed.Crypto.Implementations;
using Enmeshed.Tooling.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IStartup = Enmeshed.BuildingBlocks.API.IStartup;

namespace Devices.ConsumerApi;

public class Startup : IStartup
{
    public void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        services.ConfigureAndValidate<Configuration>(configuration.Bind);

        var parsedConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<Configuration>>().Value;

        services.AddApplication();

        services.AddDatabase(options =>
        {
            options.ConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
            options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
        });

        services.AddPushNotifications(parsedConfiguration.Infrastructure.PushNotifications);

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        services.AddSqlDatabaseHealthCheck("Devices", parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public void Configure(WebApplication app)
    {
    }

    public void ConfigureEventBus(IEventBus eventBus)
    {
        eventBus.AddDevicesIntegrationEventSubscriptions();
    }
}
