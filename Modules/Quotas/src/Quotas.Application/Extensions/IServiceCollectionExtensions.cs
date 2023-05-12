using System.Reflection;
using Backbone.Modules.Quotas.Application.AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.MediatR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backbone.Modules.Quotas.Application.Extensions;

public static class IServiceCollectionExtensions
{
 
    public static void AddApplication(this IServiceCollection services)
    {
        // services.AddMediatR(typeof(...)); // needs to be filled as soon as there is the first command/query
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddEventHandlers();
        // services.AddValidatorsFromAssemblyContaining<...>(); // needs to be filled as soon as there is the first validator
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        foreach (var eventHandler in GetAllIntegrationEventHandlers())
        {
            services.AddTransient(eventHandler);
        }
    }

    private static IEnumerable<Type> GetAllIntegrationEventHandlers()
    {
        var integrationEventHandlerTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
                                           from i in t.GetInterfaces()
                                           where t.IsClass && !t.IsAbstract && i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)
                                           select t;

        return integrationEventHandlerTypes;
    }
}

