using Autofac;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public static class GoogleCloudPubSubServiceCollectionExtensions
{
    public static void AddGoogleCloudPubSub(this IServiceCollection services, Action<GoogleCloudPubSubOptions> setupOptions)
    {
        var options = new GoogleCloudPubSubOptions();
        setupOptions.Invoke(options);

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        services.AddSingleton<IGoogleCloudPubSubPersisterConnection>(
            new DefaultGoogleCloudPubSubPersisterConnection(options.ProjectId, options.TopicName, options.SubscriptionClientName, options.ConnectionInfo));

        services.AddSingleton<IEventBus, EventBusGoogleCloudPubSub>(sp =>
        {
            var googleCloudPubSubPersisterConnection = sp.GetRequiredService<IGoogleCloudPubSubPersisterConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusGoogleCloudPubSub>>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            return new EventBusGoogleCloudPubSub(googleCloudPubSubPersisterConnection, logger,
                eventBusSubscriptionsManager, iLifetimeScope, options.HandlerRetryBehavior);
        });
    }
}

public class GoogleCloudPubSubOptions : BasicBusOptions
{
    public string ProjectId { get; set; } = null!;
    public string TopicName { get; set; } = null!;
    public string ConnectionInfo { get; set; } = null!;
}
