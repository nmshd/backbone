using Autofac;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public static class AzureServiceBusServiceCollectionExtensions
{
    public static void AddAzureServiceBus(this IServiceCollection services, Action<ServiceBusOptions> setupOptions)
    {
        var options = new ServiceBusOptions();
        setupOptions.Invoke(options);

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        services.AddSingleton<IServiceBusPersisterConnection>(
            new DefaultServiceBusPersisterConnection(options.ConnectionString));

        services.AddSingleton<IEventBus, EventBusAzureServiceBus>(sp =>
        {
            var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusAzureServiceBus>>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            return new EventBusAzureServiceBus(serviceBusPersisterConnection, logger,
                eventBusSubscriptionsManager, iLifetimeScope, options.NumberOfRetries, options.MinimumBackoff, options.MaximumBackoff, options.SubscriptionClientName);
        });
    }
}

public class ServiceBusOptions : BasicBusOptions
{
#pragma warning disable CS8618
    public string ConnectionString { get; set; }
#pragma warning restore CS8618
}
