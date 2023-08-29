using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
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
using Enmeshed.Tooling.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.ConsumerApi;

public class DevicesModule : IModule
{
    public string Name => "Devices";
    public const string PROVIDER_DIRECT = "Direct";

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

        if (parsedConfiguration.Infrastructure.PushNotifications.Provider == PROVIDER_DIRECT)
        {
            services.ConfigureAndValidate<DirectPnsCommunicationOptions.ApnsOptions>(options => configuration.GetSection("Infrastructure:PushNotifications:DirectPnsCommunication").Bind(options));
            services.ConfigureAndValidate<DirectPnsCommunicationOptions.FcmOptions>(options => configuration.GetSection("Infrastructure:PushNotifications:DirectPnsCommunication:Fcm").Bind(options));
        }

        services.AddPushNotifications(parsedConfiguration.Infrastructure.PushNotifications);

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        services.AddSqlDatabaseHealthCheck(Name, parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public void ConfigureEventBus(IEventBus eventBus)
    {
        eventBus.AddDevicesIntegrationEventSubscriptions();
    }

    public void PostStartupValidation(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetRequiredService<IOptions<Configuration>>().Value.Infrastructure.PushNotifications.Provider != PROVIDER_DIRECT)
            return;

        var apnsOptions = serviceProvider.GetRequiredService<IOptions<DirectPnsCommunicationOptions.ApnsOptions>>().Value;
        var fcmOptions = serviceProvider.GetRequiredService<IOptions<DirectPnsCommunicationOptions.FcmOptions>>().Value;
        var devicesDbContext = serviceProvider.GetRequiredService<DevicesDbContext>();

        foreach (var pnsRegistration in devicesDbContext.PnsRegistrations)
        {
            switch (pnsRegistration.Handle.Platform)
            {
                case PushNotificationPlatform.Fcm:
                    var appIdEntry = fcmOptions.Apps.GetValueOrDefault(pnsRegistration.AppId);
                    if (appIdEntry == null || appIdEntry.ServiceAccountName.IsNullOrEmpty() ||
                        fcmOptions.ServiceAccounts.GetValueOrDefault(appIdEntry.ServiceAccountName) == null || fcmOptions.ServiceAccounts[appIdEntry.ServiceAccountName].ServiceAccountJson.IsNullOrEmpty())
                        throw new InfrastructureException(GenericInfrastructureErrors.InvalidPushNotificationConfiguration());
                    break;
                case PushNotificationPlatform.Apns:
                    var bundle = apnsOptions.Bundles.GetValueOrDefault(pnsRegistration.AppId);
                    if (bundle == null || bundle.KeyName.IsNullOrEmpty() ||
                        apnsOptions.Keys.GetValueOrDefault(bundle.KeyName) == null || apnsOptions.Keys[bundle.KeyName].PrivateKey.IsNullOrEmpty())
                        throw new InfrastructureException(GenericInfrastructureErrors.InvalidPushNotificationConfiguration());
                    break;
                default: throw new Exception($"Unknown platform '{pnsRegistration.Handle.Platform}'.");
            }
        }
    }
}
