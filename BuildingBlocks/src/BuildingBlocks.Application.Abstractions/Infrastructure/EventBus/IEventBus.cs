using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

public interface IEventBus
{
    async Task Publish(IEnumerable<DomainEvent> events)
    {
        foreach (var domainEvent in events)
        {
            await Publish(domainEvent);
        }
    }

    Task Publish(DomainEvent @event);
    Task StartConsuming(CancellationToken cancellationToken);
    Task StopConsuming(CancellationToken cancellationToken);

    Task Subscribe<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>;
}
