using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.Crypto.Abstractions;
using Enmeshed.Crypto.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.ConsumerApi;

public class DevicesModule : IModule
{
    public string Name => "Devices";

    public void ConfigureServices(IServiceCollection services, IConfigurationSection configuration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(options => configuration.GetSection("Application").Bind(options));
        MapFcmOptions(services, configuration);
        MapApnsOptions(services, configuration);

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
