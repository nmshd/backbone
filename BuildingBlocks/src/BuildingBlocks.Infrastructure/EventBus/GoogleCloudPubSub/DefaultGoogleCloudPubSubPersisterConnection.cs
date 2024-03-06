using Backbone.Tooling.Extensions;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public class DefaultGoogleCloudPubSubPersisterConnection : IGoogleCloudPubSubPersisterConnection
{
    private bool _disposed;

    public DefaultGoogleCloudPubSubPersisterConnection(string projectId, string topicId,
        string subscriptionName, string connectionInfo)
    {
        var topicName = TopicName.FromProjectTopic(projectId, topicId);
        var gcpCredentials = connectionInfo.IsEmpty() ? GoogleCredential.GetApplicationDefault() : GoogleCredential.FromJson(connectionInfo);

        PublisherClient = new PublisherClientBuilder
        {
            GoogleCredential = gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
            TopicName = topicName
        }.Build();

        SubscriberClient = new SubscriberClientBuilder
        {
            GoogleCredential = gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
            SubscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionName),
            ClientCount = 5
        }.Build();
    }

    public PublisherClient PublisherClient { get; }
    public SubscriberClient SubscriberClient { get; }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        PublisherClient.ShutdownAsync(CancellationToken.None).GetAwaiter().GetResult();

        SubscriberClient.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
    }
}
