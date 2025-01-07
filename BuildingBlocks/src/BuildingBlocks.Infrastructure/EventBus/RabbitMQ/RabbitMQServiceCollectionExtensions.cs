using System.ComponentModel.DataAnnotations;
using Autofac;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

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

            return new DefaultRabbitMqPersistentConnection(factory, logger, options.ConnectionRetryCount);
        });

        services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
        {
            var subscriptionClientName = options.SubscriptionClientName;

            var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            return new EventBusRabbitMq(rabbitMqPersistentConnection, logger, iLifetimeScope, eventBusSubscriptionsManager,
                options.HandlerRetryBehavior, options.ExchangeName, subscriptionClientName, options.ConnectionRetryCount);
        });
    }
}

public class RabbitMqOptions : BasicBusOptions
{
    [Required]
    [Length(1, 1000)]
    public string ExchangeName { get; set; } = "enmeshed";

    [Required]
    [Length(1, 1000)]
    public string HostName { get; set; } = null!;

    [Required]
    [Length(1, 1000)]
    public string Username { get; set; } = null!;

    [Required]
    [Length(1, 1000)]
    public string Password { get; set; } = null!;

    [Required]
    [Range(0, 100)]
    public int ConnectionRetryCount { get; set; } = 5;
}
