using Google.Cloud.PubSub.V1;

namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public interface IGoogleCloudPubSubPersisterConnection
    : IDisposable
{
    PublisherClient PublisherClient { get; }
    SubscriberClient SubscriberClient { get; }
}
