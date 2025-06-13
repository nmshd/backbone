using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Options;

namespace Backbone.SseServer.Controllers;

public class EventQueue : IEventQueue
{
    private const string KEEP_ALIVE_EVENT = "keep-alive";

    private readonly ILogger<EventQueue> _logger;
    private readonly ConcurrentDictionary<string, Channel<string>> _channels = new();
    private readonly Configuration _configuration;

    public EventQueue(ILogger<EventQueue> logger, IOptions<Configuration> options)
    {
        _logger = logger;
        _configuration = options.Value;
    }

    public void Register(string address, CancellationToken cancellationToken)
    {
        var success = _channels.TryAdd(address, Channel.CreateUnbounded<string>());

        if (!success)
            throw new ClientAlreadyRegisteredException();

        Task.Run(async () => await SendKeepAliveEvents(address, cancellationToken), cancellationToken);
    }

    public void Deregister(string address)
    {
        _channels.TryRemove(address, out _);
    }

    public async Task EnqueueFor(string address, string eventName, CancellationToken cancellationToken)
    {
        var sanitizedEventName = eventName.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
        _logger.LogDebug("Enqueueing event '{EventName}'", sanitizedEventName);

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

    private async Task SendKeepAliveEvents(string address, CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_configuration.Sse.KeepAliveEventInterval));
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            if (!_channels.ContainsKey(address)) break;

            await EnqueueFor(address, KEEP_ALIVE_EVENT, cancellationToken);
        }
    }
}

public class ClientNotFoundException : Exception;

public class ClientAlreadyRegisteredException : Exception;
