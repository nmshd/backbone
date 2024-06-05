using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

public interface IEventBus
{
    void Publish(IEnumerable<DomainEvent> events)
    {
        foreach (var domainEvent in events)
        {
            Publish(domainEvent);
        }
    }

    void Publish(DomainEvent @event);
    void StartConsuming();

    void Subscribe<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>;
}
