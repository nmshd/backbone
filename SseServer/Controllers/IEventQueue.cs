namespace Backbone.SseServer.Controllers;

public interface IEventQueue
{
    void Register(string address);
    void Deregister(string address);
    IAsyncEnumerable<string> DequeueFor(string address, CancellationToken cancellationToken);
    Task EnqueueFor(string address, string eventName, CancellationToken cancellationToken);
}
