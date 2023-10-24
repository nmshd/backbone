using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Quotas.Application.AutoMapper;
using Backbone.Modules.Quotas.Application.Metrics;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;
using Backbone.Modules.Quotas.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateQuotaForTierCommand>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );

        services.AddScoped<IMetricStatusesService, MetricStatusesService>();
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddEventHandlers();
        services.AddMetricCalculators();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        foreach (var eventHandler in GetAllIntegrationEventHandlers())
        {
            services.AddTransient(eventHandler);
        }
    }

    private static void AddMetricCalculators(this IServiceCollection services)
    {
        var lookupType = typeof(IMetricCalculator);
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => lookupType.IsAssignableFrom(t) && !t.IsInterface);

        foreach (var type in types)
        {
            services.AddTransient(type);
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

