using System.Reflection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.BuildingBlocks.Module;
using Backbone.Crypto.Abstractions;
using Backbone.Crypto.Implementations;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Module;

public class DevicesModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Devices";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection rawModuleConfiguration)
    {
        var applicationConfiguration = rawModuleConfiguration.GetSection("Application");

        services.ConfigureAndValidate<ApplicationConfiguration>(applicationConfiguration.Bind);

        applicationConfiguration.GetSection("IdentityDeletion").Bind(IdentityDeletionConfiguration.Instance);

        services.AddMediatR(c => c
            .RegisterServicesFromAssembly(typeof(DevicesModule).Assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(DevicesModule).Assembly);
        services.AddScoped<ChallengeValidator>();
        services.AddScoped<Backbone.Modules.Devices.Contracts.IIdentityStatusProvider, Backbone.Modules.Devices.Module.Features.Identities.GetIdentity.IdentityStatusProvider>();

        AddEventHandlers(services);

        services.AddDatabase(infrastructureConfiguration.SqlDatabase);

        services.AddSingleton<ISignatureHelper, SignatureHelper>(_ => SignatureHelper.CreateEd25519WithRawKeyFormat());

        services.AddPushNotifications(infrastructureConfiguration.PushNotifications);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddDevicesDomainEventSubscriptions();
    }

    private static void AddEventHandlers(IServiceCollection services)
    {
        foreach (var eventHandler in GetAllDomainEventHandlers())
        {
            services.AddTransient(eventHandler);
        }
    }

    private static IEnumerable<Type> GetAllDomainEventHandlers()
    {
        var domainEventHandlerTypes =
            from t in Assembly.GetExecutingAssembly().GetTypes()
            from i in t.GetInterfaces()
            where t.IsClass && !t.IsAbstract && i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
            select t;

        return domainEventHandlerTypes;
    }
}
