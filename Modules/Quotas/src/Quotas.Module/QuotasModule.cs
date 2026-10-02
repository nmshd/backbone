using System.Reflection;
using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.BuildingBlocks.Module;
using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Metrics;
using Backbone.Modules.Quotas.Infrastructure;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Module.Features.Metrics.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Module;

public class QuotasModule : AbstractModule<ApplicationConfiguration, InfrastructureConfiguration>
{
    public override string Name => "Quotas";

    protected override void ConfigureServices(IServiceCollection services, InfrastructureConfiguration infrastructureConfiguration, IConfigurationSection _)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssembly(typeof(QuotasModule).Assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );

        services.AddScoped<IMetricStatusesService, MetricStatusesService>();
        services.AddTransient<MetricCalculatorFactory, ServiceProviderMetricCalculatorFactory>();
        AddEventHandlers(services);
        AddMetricCalculators(services);

        services.AddDatabase(infrastructureConfiguration.SqlDatabase);

        if (infrastructureConfiguration.SqlDatabase.EnableHealthCheck)
            services.AddSqlDatabaseHealthCheck(infrastructureConfiguration.SqlDatabase.Provider, infrastructureConfiguration.SqlDatabase.ConnectionString);

        services.AddResponseCaching();
    }

    public override async Task ConfigureEventBus(IEventBus eventBus)
    {
        await eventBus.AddQuotasDomainEventSubscriptions();
    }

    private static void AddEventHandlers(IServiceCollection services)
    {
        foreach (var eventHandler in GetAllDomainEventHandlers())
        {
            services.AddTransient(eventHandler);
        }
    }

    private static void AddMetricCalculators(IServiceCollection services)
    {
        var lookupType = typeof(IMetricCalculator);
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => lookupType.IsAssignableFrom(t) && !t.IsInterface);

        foreach (var type in types)
        {
            services.AddTransient(type);
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
