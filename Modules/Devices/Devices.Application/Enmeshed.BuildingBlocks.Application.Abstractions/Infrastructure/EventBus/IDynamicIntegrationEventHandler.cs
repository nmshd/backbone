// ReSharper disable once CheckNamespace

namespace Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
