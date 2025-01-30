using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Backbone.SseServer.Controllers;

public class EventQueue : IEventQueue
{
    private readonly ILogger<EventQueue> _logger;
    private readonly ConcurrentDictionary<string, Channel<string>> _channels = new();

    public EventQueue(ILogger<EventQueue> logger)
    {
        _logger = logger;
    }

    public void Register(string address)
    {
        var success = _channels.TryAdd(address, Channel.CreateUnbounded<string>());

        if (!success)
            throw new ClientAlreadyRegisteredException();
    }

    public void Deregister(string address)
    {
        _channels.TryRemove(address, out _);
    }

    public async Task EnqueueFor(string address, string eventName, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Enqueueing event '{EventName}'", eventName);

        if (!_channels.TryGetValue(address, out var channel))
            throw new ClientNotFoundException();

        await channel.Writer.WriteAsync(eventName, cancellationToken);
    }

    public IAsyncEnumerable<string> DequeueFor(string address, CancellationToken cancellationToken)
    {
        if (!_channels.TryGetValue(address, out var channel))
            throw new Exception($"An sse client for the address {address} is not registered.");

        return channel.Reader.ReadAllAsync(cancellationToken);
    }
}

public class ClientNotFoundException : Exception;

public class ClientAlreadyRegisteredException : Exception;
