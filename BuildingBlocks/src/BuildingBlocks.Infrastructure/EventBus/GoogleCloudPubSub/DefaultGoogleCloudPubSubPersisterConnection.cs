using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Core;

namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public class DefaultGoogleCloudPubSubPersisterConnection : IGoogleCloudPubSubPersisterConnection
{
    private const string MESSAGE_ATTRIBUTE_PREFIX = "Subject";

    private readonly string _projectId;
    private readonly TopicName _topicName;
    private readonly string _subscriptionNamePrefix;
    private readonly GoogleCredential? _gcpCredentials;

    private readonly List<SubscriberClient> _subscriberClients;

    private bool _disposed;

    public DefaultGoogleCloudPubSubPersisterConnection(string projectId, string topicId,
        string subscriptionNamePrefix, string connectionInfo)
    {
        _projectId = projectId;
        _topicName = TopicName.FromProjectTopic(_projectId, topicId);
        _subscriptionNamePrefix = subscriptionNamePrefix;
        _gcpCredentials = connectionInfo.IsEmpty() ? null : GoogleCredential.FromJson(connectionInfo);

        _subscriberClients = new List<SubscriberClient>();

        PublisherClient = new PublisherClientBuilder
        {
            GoogleCredential = _gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
            TopicName = _topicName
        }.Build();


        EnsureTopicExists();
    }

    private void EnsureTopicExists()
    {
        var publisherServiceApiClient = new PublisherServiceApiClientBuilder
        {
            GoogleCredential = _gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction
        }.Build();

        try
        {
            publisherServiceApiClient.GetTopic(_topicName);
        }
        catch (RpcException e)
        {
            if (e.Status.StatusCode == StatusCode.NotFound)
            {
                publisherServiceApiClient.CreateTopic(_topicName);
            }
            else
            {
                throw;
            }
        }
    }

    public PublisherClient PublisherClient { get; }

    public SubscriberClient GetSubscriberClient(string eventName)
    {
        var subscriptionName = GetSubscriptionName(eventName);

        var subscriberServiceApiClient = new SubscriberServiceApiClientBuilder
        {
            GoogleCredential = _gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction
        }.Build();

        try
        {
            subscriberServiceApiClient.GetSubscription(subscriptionName);
        }
        catch (RpcException ex)
        {
            // If the subscription doesn't exist, create it
            if (ex.Status.StatusCode == StatusCode.NotFound)
            {
                var filter = $"attributes.{MESSAGE_ATTRIBUTE_PREFIX} = \"{eventName}\"";

                // The subscription is of type pull, with a default timer of 10 seconds for an ackowledgement
                var subscriptionRequest = new Subscription
                {
                    SubscriptionName = subscriptionName,
                    TopicAsTopicName = _topicName,
                    Filter = filter,
                    EnableExactlyOnceDelivery = true
                };

                try
                {
                    var subscription = subscriberServiceApiClient.CreateSubscription(subscriptionRequest);
                    Console.WriteLine(subscription);
                }
                catch (Exception ex2)
                {
                    Console.WriteLine(ex2);
                }
            }
            else
            {
                throw;
            }
        }

        var subscriber = new SubscriberClientBuilder
        {
            GoogleCredential = _gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
            SubscriptionName = subscriptionName
        }.Build();

        _subscriberClients.Add(subscriber);

        return subscriber;
    }

    private SubscriptionName GetSubscriptionName(string eventName)
    {
        var subscriptionId = $"{_subscriptionNamePrefix}-{eventName}";
        return SubscriptionName.FromProjectSubscription(_projectId, subscriptionId);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        PublisherClient.ShutdownAsync(CancellationToken.None).GetAwaiter().GetResult();

        foreach (var subscriberClient in _subscriberClients)
        {
            subscriberClient.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}