using System.Reflection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Synchronization.Infrastructure;
using Backbone.Modules.Synchronization.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Synchronization.Module;

public class SynchronizationModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Synchronization";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddPersistence(infrastructureConfiguration.SqlDatabase);

        services.AddMediatR(c => c
            .RegisterServicesFromAssembly(typeof(SynchronizationModule).Assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );

        services.AddValidatorsFromAssembly(typeof(SynchronizationModule).Assembly);
        AddEventHandlers(services);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddSynchronizationDomainEventSubscriptions();
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
