namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

public interface IDynamicDomainEventHandler
{
    Task Handle(dynamic eventData);
}
