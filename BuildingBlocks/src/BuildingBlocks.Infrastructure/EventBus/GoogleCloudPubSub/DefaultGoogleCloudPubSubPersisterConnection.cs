using Backbone.Tooling.Extensions;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public class DefaultGoogleCloudPubSubPersisterConnection : IGoogleCloudPubSubPersisterConnection
{
    private readonly ILogger<DefaultGoogleCloudPubSubPersisterConnection> _logger;
    private bool _disposed;

    public DefaultGoogleCloudPubSubPersisterConnection(ILogger<DefaultGoogleCloudPubSubPersisterConnection> logger, string projectId, string topicId,
        string subscriptionName, string connectionInfo)
    {
        _logger = logger;
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
        Task.Run(async () => await DisposeAsync()).GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        try
        {
            await PublisherClient.ShutdownAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while shutting down the publisher client.");
        }

        try
        {
            await SubscriberClient.StopAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            if (ex.Message != "Can only stop a started instance.")
                throw;

            _logger.LogError(ex, "An error occurred while stopping the subscriber client.");
        }
    }
}
