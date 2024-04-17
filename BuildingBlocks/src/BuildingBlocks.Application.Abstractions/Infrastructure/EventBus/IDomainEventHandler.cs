using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

public interface IDomainEventHandler<in TIntegrationEvent> : IDomainEventHandler
    where TIntegrationEvent : DomainEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IDomainEventHandler;
