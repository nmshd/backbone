using Autofac;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public static class RabbitMqServiceCollectionExtensions
{
    public static void AddRabbitMq(this IServiceCollection services, Action<RabbitMqOptions> setupOptions)
    {
        var options = new RabbitMqOptions();
        setupOptions.Invoke(options);

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();


            var factory = new ConnectionFactory
            {
                HostName = options.HostName
            };

            if (!string.IsNullOrEmpty(options.Username)) factory.UserName = options.Username;

            if (!string.IsNullOrEmpty(options.Password)) factory.Password = options.Password;

            return new DefaultRabbitMqPersistentConnection(factory, logger, options.RetryCount);
        });

        services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
        {
            var subscriptionClientName = options.SubscriptionClientName;

            var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            return new EventBusRabbitMq(rabbitMqPersistentConnection, logger, iLifetimeScope,
                eventBusSubscriptionsManager, subscriptionClientName, options.RetryCount);
        });
    }
}

public class RabbitMqOptions : BasicBusOptions
{
#pragma warning disable CS8618
    public string HostName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int RetryCount { get; set; } = 5;
#pragma warning restore CS8618
}
