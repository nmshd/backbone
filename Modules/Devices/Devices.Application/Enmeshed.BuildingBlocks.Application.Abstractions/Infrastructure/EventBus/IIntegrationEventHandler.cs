using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

// ReSharper disable once CheckNamespace
namespace Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler { }
}
