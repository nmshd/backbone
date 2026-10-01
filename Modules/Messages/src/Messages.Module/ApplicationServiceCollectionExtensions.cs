using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Messages.Module;

internal static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(MessagesModule).Assembly;
        services.AddMediatR(configuration => configuration
            .RegisterServicesFromAssembly(assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>)));
        services.AddValidatorsFromAssembly(assembly);

        foreach (var eventHandler in GetDomainEventHandlers(assembly))
            services.AddTransient(eventHandler);
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
