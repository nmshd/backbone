using System.Reflection;
using Backbone.Modules.Quotas.Application.AutoMapper;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backbone.Modules.Quotas.Application.Extensions;

public static class IServiceCollectionExtensions
{

    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<CreateQuotaForTierCommand>());
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddEventHandlers();
        services.AddValidatorsFromAssembly(typeof(CreateQuotaForTierCommandValidator).Assembly);
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

