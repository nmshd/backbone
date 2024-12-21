using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.DatabaseMigrator;

public class DummyEventBus : IEventBus
{
    public Task Publish(DomainEvent @event)
    {
        return Task.CompletedTask;
    }

    public Task StartConsuming()
    {
        return Task.CompletedTask;
    }

    public Task Subscribe<T, TH>() where T : DomainEvent where TH : IDomainEventHandler<T>
    {
        return Task.CompletedTask;
    }
}
