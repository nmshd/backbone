using Google.Cloud.PubSub.V1;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public interface IGoogleCloudPubSubPersisterConnection
    : IAsyncDisposable
{
    PublisherClient PublisherClient { get; }
    SubscriberClient SubscriberClient { get; }
}
