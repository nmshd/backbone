using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Infrastructure;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.Exceptions;
using Enmeshed.Crypto.Abstractions;
using Enmeshed.Crypto.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.ConsumerApi;

public class DevicesModule : AbstractModule
{
    public override string Name => "Devices";

    public override void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
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

        services.AddSqlDatabaseHealthCheck(Name, parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public override void ConfigureEventBus(IEventBus eventBus)
    {
        eventBus.AddDevicesIntegrationEventSubscriptions();
    }

    public override void PostStartupValidation(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetRequiredService<IOptions<Configuration>>().Value.Infrastructure.PushNotifications.Provider != Infrastructure.PushNotifications.IServiceCollectionExtensions.PROVIDER_DIRECT)
            return;

        var apnsOptions = serviceProvider.GetRequiredService<IOptions<DirectPnsCommunicationOptions.ApnsOptions>>().Value;
        var fcmOptions = serviceProvider.GetRequiredService<IOptions<DirectPnsCommunicationOptions.FcmOptions>>().Value;
        var devicesDbContext = serviceProvider.GetRequiredService<DevicesDbContext>();

        var supportedApnsAppIds = apnsOptions.GetSupportedBundleIds();
        var supportedFcmAppIds = fcmOptions.GetSupportedAppIds();
        var devicesWithInvalidAppId = devicesDbContext.GetInvalidRegistrations(supportedApnsAppIds, supportedFcmAppIds);

        if (devicesWithInvalidAppId.Any())
            throw new InfrastructureException(InfrastructureErrors.InvalidPushNotificationConfiguration(supportedApnsAppIds.Concat(supportedFcmAppIds).ToList()));
    }
}
