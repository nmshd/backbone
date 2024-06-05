using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Fcm;
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
            options.Provider = parsedConfiguration.Infrastructure.SqlDatabase.Provider;
            options.ConnectionString = parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString;
        });

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        services.AddSqlDatabaseHealthCheck(Name, parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public override void ConfigureEventBus(IEventBus eventBus)
    {
        eventBus.AddDevicesDomainEventSubscriptions();
    }

    public override void PostStartupValidation(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IOptions<Configuration>>();
        var devicesDbContext = serviceProvider.GetRequiredService<DevicesDbContext>();

        var failingApnsBundleIds = new List<string>();
        var supportedApnsBundleIds = new List<string>();

        var failingFcmAppIds = new List<string>();
        var supportedFcmAppIds = new List<string>();

        if (configuration.Value.Infrastructure.PushNotifications.Providers.Apns is { Enabled: true })
        {
            var apnsOptions = serviceProvider.GetRequiredService<IOptions<ApnsOptions>>().Value;
            supportedApnsBundleIds = apnsOptions.GetSupportedBundleIds();
            failingApnsBundleIds = devicesDbContext.GetApnsBundleIdsForWhichNoConfigurationExists(supportedApnsBundleIds);
        }

        if (configuration.Value.Infrastructure.PushNotifications.Providers.Fcm is { Enabled: true })
        {
            var fcmOptions = serviceProvider.GetRequiredService<IOptions<FcmOptions>>().Value;
            supportedFcmAppIds = fcmOptions.GetSupportedAppIds();
            failingFcmAppIds = devicesDbContext.GetFcmAppIdsForWhichNoConfigurationExists(supportedFcmAppIds);
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
