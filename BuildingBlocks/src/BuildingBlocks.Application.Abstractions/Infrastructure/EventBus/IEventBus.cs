using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

public interface IEventBus
{
    void Publish(IntegrationEvent @event);
    void StartConsuming();
    void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}
