using Autofac;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

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
                eventBusSubscriptionsManager, iLifetimeScope);
        });
    }
}

public class GoogleCloudPubSubOptions
{
#pragma warning disable CS8618
    public string ProjectId { get; set; }
    public string TopicName { get; set; }
    public string SubscriptionClientName { get; set; }
    public string ConnectionInfo { get; set; }
#pragma warning restore CS8618
}
