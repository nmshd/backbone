using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Tooling.Extensions;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using Type = System.Type;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public partial class EventBusGoogleCloudPubSub : IEventBus, IDisposable, IAsyncDisposable
{
    private const int SUBSCRIPTION_MINIMUM_BACKOFF = 2;
    private const int SUBSCRIPTION_MAXIMUM_BACKOFF = 120;

    private static readonly TimeSpan MESSAGE_ACK_DEADLINE = 60.Seconds();

    private static class PubSubMessageAttributes
    {
        public const string EVENT_NAME = "Subject";
        public const string CORRELATION_ID = "CorrelationId";
    }

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventBusGoogleCloudPubSub> _logger;
    private readonly string _projectId;
    private readonly EventBusMetrics _metrics;
    private readonly TopicName _topicName;
    private readonly SubscriberServiceApiClient _subscriberService;

    private readonly PublisherClient _publisherClient;
    private readonly GoogleCredential _gcpCredentials;

    private readonly List<Subscription> _subscriptions = [];

    private bool _disposed;

    public EventBusGoogleCloudPubSub(ILogger<EventBusGoogleCloudPubSub> logger, IServiceProvider serviceProvider, string projectId, string topicId, string connectionInfo, EventBusMetrics metrics)
    {
        _projectId = projectId;
        _metrics = metrics;
        _topicName = TopicName.FromProjectTopic(projectId, topicId);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider;

        _gcpCredentials = connectionInfo.IsEmpty()
            ? GoogleCredential.GetApplicationDefault()
            : GoogleCredential.FromServiceAccountCredential(CredentialFactory.FromJson<ServiceAccountCredential>(connectionInfo));
        _subscriberService = new SubscriberServiceApiClientBuilder { GoogleCredential = _gcpCredentials, EmulatorDetection = EmulatorDetection.EmulatorOrProduction }.Build();

        _publisherClient = new PublisherClientBuilder
        {
            GoogleCredential = _gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
            TopicName = _topicName
        }.Build();
    }

    // We currently don't know how to properly implement this for Pub/Sub. So for now, we just return true.
    public bool IsConnected => true;

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
            await _publisherClient.ShutdownAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while shutting down the publisher client.");
        }

        try
        {
            foreach (var subscription in _subscriptions)
            {
                await subscription.SubscriberClient.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            if (ex.Message != "Can only stop a started instance.")
                throw;

            _logger.LogError(ex, "An error occurred while stopping the subscriber client.");
        }
    }

    public static SubscriptionName GetSubscriptionName<THandler, TEvent>(string projectId) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        return GetSubscriptionName(projectId, typeof(THandler), typeof(TEvent));
    }

    private static SubscriptionName GetSubscriptionName(string projectId, Type handlerType, Type eventType)
    {
        var eventHandlerFullName = handlerType.FullName!;

        var moduleName = eventHandlerFullName.Split('.').ElementAt(2);

        return new SubscriptionName(projectId, $"{moduleName}.{eventType.GetEventName()}");
    }

    private record Subscription
    {
        public Subscription(SubscriberClient subscriberClient, Type eventType, Type handlerType)
        {
            if (!eventType.IsAssignableTo(typeof(DomainEvent)))
                throw new ArgumentException("Event type must be a DomainEvent", nameof(eventType));

            if (!handlerType.IsAssignableTo(typeof(IDomainEventHandler)))
                throw new ArgumentException("Handler type must implement IDomainEventHandler", nameof(handlerType));

            SubscriberClient = subscriberClient;
            EventType = eventType;
            HandlerType = handlerType;
        }

        public SubscriberClient SubscriberClient { get; }
        public Type EventType { get; }
        public Type HandlerType { get; }
    }
}

internal static partial class EventBusGoogleCloudPubSubLogs
{
    [LoggerMessage(
        EventId = 830408,
        EventName = "EventBusGoogleCloudPubSub.SuccessfullySentDomainEvent",
        Level = LogLevel.Debug,
        Message = "Successfully sent domain event with id '{messageId}'.")]
    public static partial void SuccessfullySentDomainEvent(this ILogger logger, string messageId);

    [LoggerMessage(
        EventId = 712382,
        EventName = "EventBusGoogleCloudPubSub.ErrorHandlingMessage",
        Level = LogLevel.Error,
        Message = "Error handling message with context {exceptionSource}.")]
    public static partial void ErrorHandlingMessage(this ILogger logger, string exceptionSource, Exception exception);
}
