using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Quotas.Module.Features.Metrics.Shared;
using Backbone.Modules.Quotas.Module.Features.Tiers.CreateQuotaForTier;
using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Metrics;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Module;

internal static class ApplicationServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddMediatR(c => c
                .RegisterServicesFromAssemblyContaining<CreateQuotaForTierCommand>()
                .AddOpenBehavior(typeof(LoggingBehavior<,>))
                .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
                .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
            );

            services.AddScoped<IMetricStatusesService, MetricStatusesService>();
            services.AddTransient<MetricCalculatorFactory, ServiceProviderMetricCalculatorFactory>();
            services.AddEventHandlers();
            services.AddMetricCalculators();
        }

        private void AddEventHandlers()
        {
            foreach (var eventHandler in GetAllDomainEventHandlers())
            {
                services.AddTransient(eventHandler);
            }
        }

        private void AddMetricCalculators()
        {
            var lookupType = typeof(IMetricCalculator);
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => lookupType.IsAssignableFrom(t) && !t.IsInterface);

            foreach (var type in types)
            {
                services.AddTransient(type);
            }
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
