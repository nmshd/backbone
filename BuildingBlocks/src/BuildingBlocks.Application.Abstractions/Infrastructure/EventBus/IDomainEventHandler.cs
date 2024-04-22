using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

public interface IDomainEventHandler<in TDomainEvent> : IDomainEventHandler
    where TDomainEvent : DomainEvent
{
    Task Handle(TDomainEvent @event);
}

public interface IDomainEventHandler;
