using System.Diagnostics;
using System.Net.Sockets;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class DefaultRabbitMqPersistentConnection
    : IRabbitMqPersistentConnection
{
    private const int CONNECTION_RETRY_COUNT = 5;

    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DefaultRabbitMqPersistentConnection> _logger;
    private readonly AsyncRetryPolicy _connectionRetryPolicy;

    private IConnection? _connection;
    private bool _disposed;

    public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMqPersistentConnection> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _connectionRetryPolicy = Policy.Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetryAsync(CONNECTION_RETRY_COUNT,
                _ => 2.Seconds(),
                (ex, _, attempt, _) =>
                {
                    var logLevel = attempt % 10 == 1 ? LogLevel.Warning : LogLevel.Debug;

                    _logger.ConnectionError(attempt, ex, logLevel);
                }
            );
    }

    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    public async Task Connect()
    {
        if (IsConnected)
            return;

        await _semaphore.WaitAsync();

        try
        {
            if (IsConnected)
                return;

            await ConnectInternal();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task ConnectInternal()
    {
        _logger.LogInformation("RabbitMQ Client is trying to connect");

        await _connectionRetryPolicy.ExecuteAsync(async () => _connection = await _connectionFactory.CreateConnectionAsync());

        if (!IsConnected)
        {
            _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
            throw new Exception("RabbitMQ connections could not be created and opened");
        }

        _connection!.ConnectionShutdownAsync += OnConnectionShutdown;
        _connection.ConnectionBlockedAsync += OnConnectionBlocked;

        _logger.LogInformation("RabbitMQ persistent connection acquired a connection to '{hostName}' and is subscribed to failure events", _connection.Endpoint.HostName);
    }

    public async Task<IChannel> CreateChannel()
    {
        Debug.Assert(IsConnected, "RabbitMQ connection is not established");

        var channel = await _connection!.CreateChannelAsync();

        _logger.CreatedChannel();

        return channel;
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        try
        {
            _connection?.Dispose();
        }
        catch (IOException ex)
        {
            _logger.LogCritical(ex, "There was an error while disposing the connection.");
        }
    }

    private async Task OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed)
            return;

        _logger.ConnectionIsBlocked();
        await Connect();
    }

    private async Task OnConnectionShutdown(object sender, ShutdownEventArgs reason)
    {
        if (_disposed)
            return;

        _logger.ConnectionIsShutdown();
        await Connect();
    }
}

internal static partial class DefaultRabbitMqPersistentConnectionLogs
{
    [LoggerMessage(
        EventId = 715507,
        EventName = "DefaultRabbitMqPersistentConnection.ConnectionError",
        Message = "There was an error while trying to connect to RabbitMQ. Making retry attempt number {retryAttempt}...")]
    public static partial void ConnectionError(this ILogger logger, int retryAttempt, Exception exception, LogLevel logLevel = LogLevel.Error);

    [LoggerMessage(
        EventId = 953485,
        EventName = "DefaultRabbitMqPersistentConnection.CreatedChannel",
        Level = LogLevel.Debug,
        Message = "Successfully created a new channel.")]
    public static partial void CreatedChannel(this ILogger logger);

    [LoggerMessage(
        EventId = 119836,
        EventName = "DefaultRabbitMqPersistentConnection.ConnectionIsShutdown",
        Level = LogLevel.Warning,
        Message = "A RabbitMQ connection is shutdown. Trying to re-connect...")]
    public static partial void ConnectionIsShutdown(this ILogger logger);

    [LoggerMessage(
        EventId = 143946,
        EventName = "DefaultRabbitMqPersistentConnection.ConnectionThrewAnException",
        Level = LogLevel.Warning,
        Message = "A RabbitMQ connection threw an exception. Trying to re-connect...")]
    public static partial void ConnectionThrewAnException(this ILogger logger);

    [LoggerMessage(
        EventId = 454129,
        EventName = "DefaultRabbitMqPersistentConnection.ConnectionIsBlocked",
        Level = LogLevel.Warning,
        Message = "A RabbitMQ connection is blocked. Trying to re-connect...")]
    public static partial void ConnectionIsBlocked(this ILogger logger);
}
