using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.Crypto.Abstractions;
using Enmeshed.Crypto.Implementations;
using Enmeshed.Tooling.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

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
            MapFcmOptions(services, configuration);
            MapApnsOptions(services, configuration);
        }

        services.AddPushNotifications(parsedConfiguration.Infrastructure.PushNotifications);

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        services.AddSqlDatabaseHealthCheck(Name, parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    private static void MapFcmOptions(IServiceCollection services, IConfiguration configuration)
    {
        var fcmConfiguration = configuration.GetSection("Infrastructure:PushNotifications:DirectPnsCommunication:Fcm").Get<FcmSettings>();
        services.ConfigureAndValidate<DirectPnsCommunicationOptions.FcmOptions>(options =>
        {
            options.DefaultBundleId = fcmConfiguration!.DefaultBundleId;
            foreach (var app in fcmConfiguration!.Apps)
            {
                var serviceAccountJson = fcmConfiguration.ServiceAccounts.GetValueOrDefault(app.Value.ServiceAccountName)!.ServiceAccountJson;
                options.KeysByApplicationId[app.Key] = new DirectPnsCommunicationOptions.FcmOptions.ServiceAccount() { ServiceAccountJson = serviceAccountJson };
            }
        });
    }

    private static void MapApnsOptions(IServiceCollection services, IConfiguration configuration)
    {
        var apnsConfiguration = configuration.GetSection("Infrastructure:PushNotifications:DirectPnsCommunication:Apns").Get<ApnsSettings>();
        services.ConfigureAndValidate<DirectPnsCommunicationOptions.ApnsOptions>(options =>
        {
            options.DefaultBundleId = apnsConfiguration!.DefaultBundleId;
            foreach (var app in apnsConfiguration!.Bundles)
            {
                var keyConfig = apnsConfiguration.Keys.GetValueOrDefault(app.Value.KeyName)!;
                options.KeysByBundleId[app.Key] = new DirectPnsCommunicationOptions.ApnsOptions.Key()
                {
                    KeyId = keyConfig.KeyId,
                    PrivateKey = keyConfig.PrivateKey,
                    TeamId = keyConfig.TeamId,
                    ServerType = app.Value.ServerType
                };
            }
        });
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
                    if (fcmOptions.KeysByApplicationId.GetValueOrDefault(pnsRegistration.AppId) == null || fcmOptions.KeysByApplicationId[pnsRegistration.AppId!].ServiceAccountJson.IsNullOrEmpty())
                        throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPropertyValue("ServiceAccountJson"));
                    break;
                case PushNotificationPlatform.Apns:
                    if (apnsOptions.KeysByBundleId.GetValueOrDefault(pnsRegistration.AppId) == null || apnsOptions.KeysByBundleId[pnsRegistration.AppId!].PrivateKey.IsNullOrEmpty())
                        throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPropertyValue("PrivateKey"));
                    break;
                default: throw new Exception($"Unknown platform '{pnsRegistration.Handle.Platform}'.");
            }
        }
    }
}

public class FcmSettings
{
    public string DefaultBundleId { get; set; }
    public Dictionary<string, ServiceAccount> ServiceAccounts { get; set; } = new();
    public class ServiceAccount
    {
        public string ServiceAccountJson { get; set; } = string.Empty;
    }
    public Dictionary<string, ServiceAccountInformation> Apps { get; set; } = new();
    public class ServiceAccountInformation
    {
        public string ServiceAccountName { get; set; } = string.Empty;
    }
}

public class ApnsSettings
{
    public string DefaultBundleId { get; set; }
    public Dictionary<string, Key> Keys { get; set; } = new();
    public class Key
    {
        public string TeamId { get; set; } = string.Empty;
        public string KeyId { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
    }
    public Dictionary<string, Bundle> Bundles { get; set; } = new();
    public class Bundle
    {
        public string KeyName { get; set; }
        public DirectPnsCommunicationOptions.ApnsOptions.Key.ApnsServerType ServerType { get; set; }
    }
}
