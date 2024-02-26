using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class DefaultRabbitMqPersistentConnection
    : IRabbitMqPersistentConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DefaultRabbitMqPersistentConnection> _logger;
    private readonly int _retryCount;

    private readonly object _syncRoot = new();
    private IConnection? _connection;
    private bool _disposed;

    public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory,
        ILogger<DefaultRabbitMqPersistentConnection> logger, int retryCount = 5)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryCount = retryCount;
    }

    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    public IModel CreateModel()
    {
        if (!IsConnected)
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

        return _connection!.CreateModel();
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

    public bool TryConnect()
    {
        _logger.LogInformation("RabbitMQ Client is trying to connect");

        lock (_syncRoot)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, _) => _logger.ConnectionError(ex));

            policy.Execute(() => _connection = _connectionFactory
                    .CreateConnection());

            if (!IsConnected)
            {
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                return false;
            }

            _connection!.ConnectionShutdown += OnConnectionShutdown;
            _connection!.CallbackException += OnCallbackException;
            _connection!.ConnectionBlocked += OnConnectionBlocked;

            _logger.LogInformation(
                "RabbitMQ persistent connection acquired a connection '{hostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

            return true;
        }
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;
        _logger.ConnectionIsBlocked();
        TryConnect();
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return;
        _logger.ConnectionThrewAnException();
        TryConnect();
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs reason)
    {
        if (_disposed) return;
        _logger.ConnectionIsShutdown();
        TryConnect();
    }
}

internal static partial class DefaultRabbitMqPersistentConnectionLogs
{
    [LoggerMessage(
        EventId = 715507,
        EventName = "DefaultRabbitMqPersistentConnection.ConnectionError",
        Level = LogLevel.Warning,
        Message = "There was an error while trying to connect to RabbitMQ. Attempting to retry...")]
    public static partial void ConnectionError(this ILogger logger, Exception exception);

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
