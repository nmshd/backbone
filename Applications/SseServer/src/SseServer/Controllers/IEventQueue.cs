namespace Backbone.SseServer.Controllers;

public interface IEventQueue
{
    void Register(string address, CancellationToken cancellationToken);
    void Deregister(string address);
    IAsyncEnumerable<string> DequeueFor(string address, CancellationToken cancellationToken);
    Task EnqueueFor(string address, string eventName, CancellationToken cancellationToken);
}
