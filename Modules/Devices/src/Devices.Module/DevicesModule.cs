using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Module;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Infrastructure;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Module;

public class DevicesModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Devices";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection rawModuleConfiguration)
    {
        services.AddApplication(rawModuleConfiguration.GetSection("Application"));

        services.AddDatabase(infrastructureConfiguration.SqlDatabase);

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        services.AddPushNotifications(infrastructureConfiguration.PushNotifications);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(Name, infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddDevicesDomainEventSubscriptions();
    }
}
