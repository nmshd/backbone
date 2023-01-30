using Google.Cloud.PubSub.V1;

namespace Backbone.Modules.Devices.Infrastructure.Enmeshed.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub
{
    public interface IGoogleCloudPubSubPersisterConnection
        : IDisposable
    {
        PublisherClient PublisherClient { get; }
        SubscriberClient GetSubscriberClient(string eventName);
    }
}