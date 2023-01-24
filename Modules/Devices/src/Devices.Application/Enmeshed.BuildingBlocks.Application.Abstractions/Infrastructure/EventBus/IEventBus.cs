using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

// ReSharper disable once CheckNamespace
namespace Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
