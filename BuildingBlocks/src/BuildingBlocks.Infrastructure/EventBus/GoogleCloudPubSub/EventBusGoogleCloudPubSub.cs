using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.BuildingBlocks.Infrastructure.EventBus.Json;
using Backbone.Tooling.Extensions;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;

public class EventBusGoogleCloudPubSub : IEventBus, IDisposable, IAsyncDisposable
{
    private static readonly TimeSpan MESSAGE_ACK_DEADLINE = 60.Seconds();

    private static class PubSubMessageAttributes
    {
        public const string EVENT_NAME = "Subject";
        public const string CORRELATION_ID = "CorrelationId";
    }

    private static readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new()
    {
        ContractResolver = new ContractResolverWithPrivates()
    };

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventBusGoogleCloudPubSub> _logger;

    private readonly HandlerRetryBehavior _handlerRetryBehavior;

    private readonly string _projectId;
    private readonly TopicName _topicName;
    private readonly SubscriberServiceApiClient _subscriberService;
    private readonly PublisherClient _publisherClient;
    private readonly GoogleCredential _gcpCredentials;
    private readonly List<Subscription> _subscriptions = [];
    private bool _disposed;

    public EventBusGoogleCloudPubSub(ILogger<EventBusGoogleCloudPubSub> logger, IServiceProvider serviceProvider, HandlerRetryBehavior handlerRetryBehavior, string projectId, string topicId,
        string connectionInfo)
    {
        _projectId = projectId;
        _topicName = TopicName.FromProjectTopic(projectId, topicId);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider;
        _handlerRetryBehavior = handlerRetryBehavior;

        _gcpCredentials = connectionInfo.IsEmpty() ? GoogleCredential.GetApplicationDefault() : GoogleCredential.FromJson(connectionInfo);
        _subscriberService = new SubscriberServiceApiClientBuilder { GoogleCredential = _gcpCredentials, EmulatorDetection = EmulatorDetection.EmulatorOrProduction }.Build();

        _publisherClient = new PublisherClientBuilder
        {
            GoogleCredential = _gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
            TopicName = _topicName
        }.Build();
    }

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

    public async Task Publish(DomainEvent @event)
    {
        var eventName = @event.GetEventName();

        var jsonMessage = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
        {
            ContractResolver = new ContractResolverWithPrivates()
        });

        var message = new PubsubMessage
        {
            Data = ByteString.CopyFromUtf8(jsonMessage),
            Attributes =
            {
                { PubSubMessageAttributes.EVENT_NAME, eventName },
                { PubSubMessageAttributes.CORRELATION_ID, CustomLogContext.GetCorrelationId() }
            }
        };

        var messageId = await _logger.TraceTime(
            () => _publisherClient.PublishAsync(message), nameof(_publisherClient.PublishAsync));

        _logger.EventWasNotProcessed(messageId);
    }

    public async Task Subscribe<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>
    {
        var eventName = typeof(T).GetEventName();

        _logger.LogInformation("Subscribing to event '{EventName}' with {EventHandler}", eventName, typeof(TH).Name);

        var subscriptionName = GetSubscriptionName<TH, T>(_projectId);
        await CreateSubscriptionIfNotExists(subscriptionName, eventName);

        var subscriberClient = await new SubscriberClientBuilder
        {
            SubscriptionName = subscriptionName,
            GoogleCredential = _gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction,
        }.BuildAsync();

        _subscriptions.Add(new Subscription(subscriberClient, typeof(T), typeof(TH)));
    }

    private async Task CreateSubscriptionIfNotExists(SubscriptionName subscriptionName, string eventName)
    {
        try
        {
            var subscriptionRequest = new Google.Cloud.PubSub.V1.Subscription
            {
                SubscriptionName = subscriptionName,
                TopicAsTopicName = _topicName,
                Filter = $"attributes.{PubSubMessageAttributes.EVENT_NAME} = \"{eventName}\"",
                AckDeadlineSeconds = (int)MESSAGE_ACK_DEADLINE.TotalSeconds
            };
            await _subscriberService.CreateSubscriptionAsync(subscriptionRequest);
        }
        catch (RpcException ex)
        {
            if (ex.StatusCode == StatusCode.AlreadyExists)
                return;

            throw;
        }
    }

    public async Task StartConsuming(CancellationToken cancellationToken)
    {
        var consumptionTasks = _subscriptions.Select(s => s.SubscriberClient.StartAsync((e, _) => OnIncomingEvent(e, s.EventType, s.HandlerType)));

        await Task.WhenAll(consumptionTasks);
    }

    private async Task<SubscriberClient.Reply> OnIncomingEvent(PubsubMessage @event, Type eventType, Type handlerType)
    {
        var eventData = @event.Data.ToStringUtf8();

        try
        {
            @event.Attributes.TryGetValue(PubSubMessageAttributes.CORRELATION_ID, out var correlationId);

            correlationId = correlationId.IsNullOrEmpty() ? CustomLogContext.GenerateCorrelationId() : correlationId;

            using (CustomLogContext.SetCorrelationId(correlationId))
            {
                await ProcessEvent(eventData, eventType, handlerType);
            }
        }
        catch (Exception ex)
        {
            _logger.ErrorHandlingMessage(ex.StackTrace!, ex);
            return SubscriberClient.Reply.Nack;
        }

        return SubscriberClient.Reply.Ack;
    }

    private async Task ProcessEvent(string message, Type eventType, Type handlerType)
    {
        var domainEvent = JsonConvert.DeserializeObject(message, eventType, JSON_SERIALIZER_SETTINGS)!;

        await using var scope = _serviceProvider.CreateAsyncScope();

        if (scope.ServiceProvider.GetService(handlerType) is not IDomainEventHandler handler)
            throw new Exception("Domain event handler could not be resolved from dependency container or it does not implement IDomainEventHandler.");

        var handleMethod = handler.GetType().GetMethod("Handle");

        await (Task)handleMethod!.Invoke(handler, [domainEvent])!;
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        foreach (var subscription in _subscriptions)
        {
            await subscription.SubscriberClient.StopAsync(CancellationToken.None);
        }
    }

    public static SubscriptionName GetSubscriptionName<THandler, TEvent>(string projectId) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventHandlerFullName = typeof(THandler).FullName!;

        var moduleName = eventHandlerFullName.Split('.').ElementAt(2);

        return new SubscriptionName(projectId, $"{moduleName}.{typeof(TEvent).GetEventName()}");
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
        EventName = "EventBusGoogleCloudPubSub.SendingDomainEvent",
        Level = LogLevel.Debug,
        Message = "Successfully sent domain event with id '{messageId}'.")]
    public static partial void EventWasNotProcessed(this ILogger logger, string messageId);

    [LoggerMessage(
        EventId = 712382,
        EventName = "EventBusGoogleCloudPubSub.ErrorHandlingMessage",
        Level = LogLevel.Error,
        Message = "Error handling message with context {exceptionSource}.")]
    public static partial void ErrorHandlingMessage(this ILogger logger, string exceptionSource, Exception exception);

    [LoggerMessage(
        EventId = 590747,
        EventName = "EventBusGoogleCloudPubSub.NoSubscriptionForEvent",
        Level = LogLevel.Warning,
        Message = "No subscription for event: '{eventName}'.")]
    public static partial void NoSubscriptionForEvent(this ILogger logger, string eventName);

    [LoggerMessage(
        EventId = 304842,
        EventName = "EventBusGoogleCloudPubSub.ErrorWhileExecutingEventHandlerType",
        Level = LogLevel.Warning,
        Message = "An error was thrown while executing '{eventHandlerType}'. Attempting to retry...")]
    public static partial void ErrorWhileExecutingEventHandlerType(this ILogger logger, string eventHandlerType, Exception exception);
}
