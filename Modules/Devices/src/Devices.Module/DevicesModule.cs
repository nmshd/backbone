using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Module;

public class DevicesModule : AbstractModule<ApplicationOptions, InfrastructureConfiguration>
{
    public override string Name => "Devices";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection rawModuleConfiguration)
    {
        services.AddApplication(rawModuleConfiguration.GetSection("Application"));

        services.AddDatabase(options =>
        {
            options.Provider = infrastructureConfiguration.SqlDatabase.Provider;
            options.ConnectionString = infrastructureConfiguration.SqlDatabase.ConnectionString;
        });

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        services.AddPushNotifications(infrastructureConfiguration.PushNotifications);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddDevicesDomainEventSubscriptions();
    }

    public override void PostStartupValidation(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IOptions<InfrastructureConfiguration>>().Value;
        var devicesDbContext = serviceProvider.GetRequiredService<DevicesDbContext>();

        var failingApnsBundleIds = new List<string>();
        var supportedApnsBundleIds = new List<string>();

        var failingFcmAppIds = new List<string>();
        var supportedFcmAppIds = new List<string>();

        if (configuration.PushNotifications.Providers.Apns is { Enabled: true })
        {
            var apnsOptions = serviceProvider.GetRequiredService<IOptions<ApnsOptions>>().Value;
            supportedApnsBundleIds = apnsOptions.GetSupportedBundleIds();
            failingApnsBundleIds = devicesDbContext.GetApnsBundleIdsForWhichNoConfigurationExists(supportedApnsBundleIds).GetAwaiter().GetResult();
        }

        if (configuration.PushNotifications.Providers.Fcm is { Enabled: true })
        {
            var fcmOptions = serviceProvider.GetRequiredService<IOptions<FcmOptions>>().Value;
            supportedFcmAppIds = fcmOptions.GetSupportedAppIds();
            failingFcmAppIds = devicesDbContext.GetFcmAppIdsForWhichNoConfigurationExists(supportedFcmAppIds).GetAwaiter().GetResult();
        }

        if (failingFcmAppIds.Count + failingApnsBundleIds.Count > 0)
        {
            var configuredFcmAppIdsString = string.Join(", ", supportedFcmAppIds.Select(x => $"'{x}'"));
            var configuredApnsBundleIdsString = string.Join(", ", supportedApnsBundleIds.Select(x => $"'{x}'"));

            var failingFcmAppIdsString = failingFcmAppIds.Count > 0
                ? "\nThe questionable FCM app ids are: " + string.Join(", ", failingFcmAppIds.Select(x => $"'{x}'")) + $". The configured app ids are: {configuredFcmAppIdsString}."
                : "";
            var failingApnsBundleIdsString = failingApnsBundleIds.Count > 0
                ? "\nThe questionable APNs app ids are: " + string.Join(", ", failingApnsBundleIds.Select(x => $"'{x}'")) + $". The configured app ids are: {configuredApnsBundleIdsString}."
                : "";

            var errorMessage = $"There are push notification registrations in the database with an app id for which there is no configuration.{failingFcmAppIdsString}{failingApnsBundleIdsString}";

            throw new Exception(errorMessage);
        }
    }
}
