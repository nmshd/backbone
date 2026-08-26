using System.Diagnostics;
using System.Net.Sockets;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public partial class EventBusRabbitMq : IEventBus, IDisposable
{
    private const int CONNECTION_RETRY_COUNT = 6;

    private const string MESSAGING_SYSTEM = "rabbitmq";

    private readonly ILogger<EventBusRabbitMq> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;

    private readonly ChannelPool _channelPool;

    private readonly string _exchangeName;
    private readonly EventBusMetrics _metrics;

    private EventBusRabbitMq(IConnection connection, ILogger<EventBusRabbitMq> logger, IServiceProvider serviceProvider, string exchangeName, EventBusMetrics metrics)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider;
        _exchangeName = exchangeName;
        _metrics = metrics;
        _deadLetterExchangeName = $"deadletterexchange.{exchangeName}";
        _deadLetterQueueName = $"deadletterqueue.{exchangeName}";
        _connection = connection;
        _channelPool = new ChannelPool(connection);

        _publishRetryPolicy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .Or<AlreadyClosedException>()
            .WaitAndRetryAsync(PUBLISH_RETRY_COUNT,
                _ => 2.Seconds(),
                (ex, _) => Activity.Current?.AddException(ex));

        _connection.ConnectionShutdownAsync += (_, args) =>
        {
            _logger.ConnectionShutdown(args.Initiator, args.ReplyCode, args.ReplyText);
            return Task.CompletedTask;
        };

        _connection.ConnectionRecoveryErrorAsync += (_, args) =>
        {
            _logger.ConnectionRecoveryError(args.Exception.Message);
            return Task.CompletedTask;
        };

        _connection.RecoverySucceededAsync += (_, _) =>
        {
            _logger.RecoverySucceeded();
            return Task.CompletedTask;
        };
    }

    public static async Task<EventBusRabbitMq> Create(IConnectionFactory connectionFactory, ILogger<EventBusRabbitMq> logger, IServiceProvider serviceProvider, string exchangeName,
        EventBusMetrics metrics)
    {
        var connectionRetryPolicy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .Or<AlreadyClosedException>()
            .WaitAndRetryAsync(CONNECTION_RETRY_COUNT,
                _ => 2.Seconds(),
                (ex, _) => logger.RetryingInitialConnect(ex.Message));

        var connection = await connectionRetryPolicy.ExecuteAsync(() => connectionFactory.CreateConnectionAsync());

        var eventBus = new EventBusRabbitMq(connection, logger, serviceProvider, exchangeName, metrics);

        await eventBus.Init();

        return eventBus;
    }

    private async Task Init()
    {
        await EnsureExchangeExists(_exchangeName);
        await EnsureExchangeExists(_deadLetterExchangeName, "fanout");
        await EnsureDeadLetterQueueExists();
    }

    private async Task EnsureExchangeExists(string exchangeName, string exchangeType = "direct")
    {
        try
        {
            var channel = await _channelPool.Get();
            await channel.ExchangeDeclarePassiveAsync(exchangeName);
            _channelPool.Return(channel);
        }
        catch (OperationInterruptedException ex)
        {
            if (ex.ShutdownReason?.ReplyCode == 404)
            {
                try
                {
                    var channel = await _channelPool.Get();
                    await channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable: true);
                    _channelPool.Return(channel);
                }
                catch (Exception)
                {
                    _logger.LogCritical("The exchange '{ExchangeName}' does not exist and could not be created.", exchangeName);
                    throw new Exception($"The exchange '{exchangeName}' does not exist and could not be created.");
                }
            }
        }
    }

    public void Dispose()
    {
        _channelPool.Dispose();
    }

    public bool IsConnected => _connection.IsOpen;
}

internal static partial class EventBusRabbitMqLogs
{
    [LoggerMessage(
        EventId = 746534,
        EventName = "EventBusRabbitMQ.RetryingInitialConnect",
        Level = LogLevel.Warning,
        Message = "There was an error while trying to initially connect to RabbitMQ: '{errorMessage}'. Attempting to retry...")]
    public static partial void RetryingInitialConnect(this ILogger logger, string errorMessage);

    [LoggerMessage(
        EventId = 900001,
        EventName = "EventBusRabbitMQ.ConnectionShutdown",
        Level = LogLevel.Error,
        Message = "A shutdown of the connection was initiated. Initiator: {shutdownInitiator}, ReplyCode: {replyCode}, ReplyText: {replyText}")]
    public static partial void ConnectionShutdown(this ILogger logger, ShutdownInitiator shutdownInitiator, ushort replyCode, string replyText);

    [LoggerMessage(
        EventId = 900002,
        EventName = "EventBusRabbitMQ.ConnectionRecoveryError",
        Level = LogLevel.Warning,
        Message = "An error occurred while trying to recover the connection: {errorMessage}")]
    public static partial void ConnectionRecoveryError(this ILogger logger, string errorMessage);

    [LoggerMessage(
        EventId = 900003,
        EventName = "EventBusRabbitMQ.RecoverySucceeded",
        Level = LogLevel.Information,
        Message = "The connection was successfully recovered.")]
    public static partial void RecoverySucceeded(this ILogger logger);
}
