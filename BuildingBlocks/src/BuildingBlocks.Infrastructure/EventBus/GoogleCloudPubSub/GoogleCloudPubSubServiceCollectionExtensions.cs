using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public static class GoogleCloudPubSubServiceCollectionExtensions
{
    public static void AddGoogleCloudPubSub(this IServiceCollection services, GoogleCloudPubSubConfiguration configuration)
    {
        services.AddSingleton<IEventBus, EventBusGoogleCloudPubSub>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<EventBusGoogleCloudPubSub>>();
            var metrics = sp.GetRequiredService<EventBusMetrics>();

            return new EventBusGoogleCloudPubSub(logger, sp, configuration.ProjectId, configuration.TopicName, configuration.ServiceAccountJson, metrics);
        });
    }
}

public class GoogleCloudPubSubConfiguration
{
    [Required]
    [Length(4, 30)]
    public string ProjectId { get; set; } = null!;

    [Required]
    [Length(1, 255)]
    public string TopicName { get; set; } = null!;

    [Required]
    [MinLength(50)]
    public string ServiceAccountJson { get; set; } = null!;
}
