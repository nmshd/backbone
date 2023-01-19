using Autofac;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class RabbitMQServiceCollectionExtensions
    {
        public static void AddRabbitMQ(this IServiceCollection services, Action<RabbitMQOptions> setupOptions)
        {
            var options = new RabbitMQOptions();
            setupOptions.Invoke(options);

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


                var factory = new ConnectionFactory
                {
                    HostName = options.HostName
                };

                if (!string.IsNullOrEmpty(options.Username)) factory.UserName = options.Username;

                if (!string.IsNullOrEmpty(options.Password)) factory.Password = options.Password;

                return new DefaultRabbitMQPersistentConnection(factory, logger, options.RetryCount);
            });

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var subscriptionClientName = options.SubscriptionClientName;

                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope,
                    eventBusSubcriptionsManager, subscriptionClientName, options.RetryCount);
            });
        }
    }

    public class RabbitMQOptions
    {
#pragma warning disable CS8618
        public string HostName { get; set; }
        public string SubscriptionClientName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RetryCount { get; set; } = 5;
#pragma warning restore CS8618
    }
}