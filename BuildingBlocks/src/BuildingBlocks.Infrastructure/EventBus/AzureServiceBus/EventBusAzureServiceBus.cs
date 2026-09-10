using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public partial class EventBusAzureServiceBus : IEventBus, IDisposable, IAsyncDisposable
{
    private const string TOPIC_NAME = "default";
    private const string MESSAGING_SYSTEM = "servicebus";
    private const int MAX_DELIVERY_COUNT = 5;

    private readonly ServiceBusProcessorOptions _options = new()
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = 1,
        PrefetchCount = 10
    };

    private readonly IServiceProvider _serviceProvider;
    private readonly EventBusMetrics _metrics;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusAdministrationClient _adminClient;
    private readonly ServiceBusSender _sender;
    private readonly ILogger<EventBusAzureServiceBus> _logger;

    private readonly List<ServiceBusProcessor> _processors = [];

    public EventBusAzureServiceBus(ServiceBusClient client, ServiceBusAdministrationClient adminClient, ILogger<EventBusAzureServiceBus> logger, IServiceProvider serviceProvider,
        EventBusMetrics metrics)
    {
        _client = client;
        _adminClient = adminClient;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider;
        _metrics = metrics;
        _sender = client.CreateSender(TOPIC_NAME);
    }

    public void Dispose()
    {
        Task.Run(async () => await DisposeAsync()).GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var processor in _processors)
        {
            await processor.CloseAsync();
        }
    }

    public bool IsConnected => !_client.IsClosed;

    public static string GetSubscriptionName<THandler, TEvent>() where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventHandlerFullName = typeof(THandler).FullName!;

        var moduleName = eventHandlerFullName.Split('.').ElementAt(2);

        return $"{moduleName}.{typeof(TEvent).GetEventName()}".TruncateToXChars(50);
    }
}

internal static partial class EventBusAzureServiceBusLogs
{
    [LoggerMessage(
        EventId = 302940,
        EventName = "EventBusAzureServiceBus.SendingDomainEvent",
        Level = LogLevel.Debug,
        Message = "Sending domain event with id '{messageId}'...")]
    public static partial void SendingDomainEvent(this ILogger logger, string messageId);

    [LoggerMessage(
        EventId = 630568,
        EventName = "EventBusAzureServiceBus.EventWasNotProcessed",
        Level = LogLevel.Information,
        Message = "The event with the MessageId '{messageId}' wasn't processed and will therefore not be completed.")]
    public static partial void EventWasNotProcessed(this ILogger logger, string messageId);

    [LoggerMessage(
        EventId = 949322,
        EventName = "EventBusAzureServiceBus.ErrorHandlingMessage",
        Level = LogLevel.Error,
        Message = "Error handling message with context {exceptionContext}.")]
    public static partial void ErrorHandlingMessage(this ILogger logger, ServiceBusErrorSource exceptionContext, Exception exception);
}
