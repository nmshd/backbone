using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using IServiceCollectionExtensions = Backbone.Modules.Devices.Infrastructure.PushNotifications.IServiceCollectionExtensions;

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

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        services.AddSqlDatabaseHealthCheck(Name, parsedConfiguration.Infrastructure.SqlDatabase.Provider, parsedConfiguration.Infrastructure.SqlDatabase.ConnectionString);
    }

    public override void ConfigureEventBus(IEventBus eventBus)
    {
        eventBus.AddDevicesIntegrationEventSubscriptions();
    }

    public override void PostStartupValidation(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IOptions<Configuration>>();
        if (configuration.Value.Infrastructure.PushNotifications.Provider != IServiceCollectionExtensions.PROVIDER_DIRECT)
            return;

        var fcmOptions = serviceProvider.GetRequiredService<IOptions<DirectPnsCommunicationOptions.FcmOptions>>().Value;
        var apnsOptions = serviceProvider.GetRequiredService<IOptions<DirectPnsCommunicationOptions.ApnsOptions>>().Value;
        var devicesDbContext = serviceProvider.GetRequiredService<DevicesDbContext>();

        var supportedFcmAppIds = fcmOptions.GetSupportedAppIds();
        var supportedApnsBundleIds = apnsOptions.GetSupportedBundleIds();

        var failingFcmAppIds = devicesDbContext.GetFcmAppIdsForWhichNoConfigurationExists(supportedFcmAppIds);
        var failingApnsBundleIds = devicesDbContext.GetApnsBundleIdsForWhichNoConfigurationExists(supportedApnsBundleIds);

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
