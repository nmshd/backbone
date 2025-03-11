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

        services.AddSingleton<IEventBus, EventBusGoogleCloudPubSub>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<EventBusGoogleCloudPubSub>>();

            return new EventBusGoogleCloudPubSub(logger, sp, options.HandlerRetryBehavior, options.ProjectId, options.TopicName, options.ConnectionInfo);
        });
    }
}

public class GoogleCloudPubSubOptions : BasicBusOptions
{
    public string ProjectId { get; set; } = null!;
    public string TopicName { get; set; } = null!;
    public string ConnectionInfo { get; set; } = null!;
}
