using System.Reflection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Relationships.Infrastructure;
using Backbone.Modules.Relationships.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Relationships.Module;

public class RelationshipsModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Relationships";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssembly(typeof(RelationshipsModule).Assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );

        services.AddValidatorsFromAssembly(typeof(RelationshipsModule).Assembly);

        AddEventHandlers(services);

        services.AddPersistence(infrastructureConfiguration.SqlDatabase);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddRelationshipsDomainEventSubscriptions();
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
