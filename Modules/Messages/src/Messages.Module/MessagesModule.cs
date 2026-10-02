using System.Reflection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Messages.Infrastructure;
using Backbone.Modules.Messages.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Messages.Module;

public class MessagesModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Messages";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddPersistence(infrastructureConfiguration.SqlDatabase);

        var assembly = typeof(MessagesModule).Assembly;
        services.AddMediatR(configuration => configuration
            .RegisterServicesFromAssembly(assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>)));
        services.AddValidatorsFromAssembly(assembly);

        foreach (var eventHandler in GetDomainEventHandlers(assembly))
            services.AddTransient(eventHandler);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddMessagesDomainEventSubscriptions();
    }

    private static IEnumerable<Type> GetDomainEventHandlers(Assembly assembly)
    {
        return from type in assembly.GetTypes()
               from implementedInterface in type.GetInterfaces()
               where type.IsClass && !type.IsAbstract && implementedInterface.IsGenericType &&
                     implementedInterface.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
               select type;
    }
}
