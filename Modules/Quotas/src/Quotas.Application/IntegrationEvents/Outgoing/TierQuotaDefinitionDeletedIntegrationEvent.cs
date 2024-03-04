using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
public class TierQuotaDefinitionDeletedIntegrationEvent : IntegrationEvent
{
    public TierQuotaDefinitionDeletedIntegrationEvent(string tierId, string tierQuotaDefinitionId) : base($"{tierQuotaDefinitionId}/Deleted")
    {
        TierId = tierId;
        TierQuotaDefinitionId = tierQuotaDefinitionId;
    }

    public string TierId { get; }
    public string TierQuotaDefinitionId { get; }
}
