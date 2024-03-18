namespace Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

public interface IDynamicIntegrationEventHandler
{
    Task Handle(dynamic eventData);
}
