using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

public interface IEventBus
{
    void Publish(DomainEvent @event);
    void StartConsuming();

    void Subscribe<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>;
}
