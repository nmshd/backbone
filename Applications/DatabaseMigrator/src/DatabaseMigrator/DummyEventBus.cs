using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.DatabaseMigrator;

public class DummyEventBus : IEventBus
{
    public void Publish(DomainEvent @event)
    {
    }

    public void StartConsuming()
    {
    }

    public void Subscribe<T, TH>() where T : DomainEvent where TH : IDomainEventHandler<T>
    {
    }
}
