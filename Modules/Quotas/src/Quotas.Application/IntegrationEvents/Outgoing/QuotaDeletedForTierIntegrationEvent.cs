using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
public class QuotaDeletedForTierIntegrationEvent : IntegrationEvent
{
    public QuotaDeletedForTierIntegrationEvent(string tierId, string tierQuotaDefinitionId) : base($"{tierQuotaDefinitionId}/Deleted")
    {
        TierId = tierId;
        TierQuotaDefinitionId = tierQuotaDefinitionId;
    }

    public string TierId { get; }
    public string TierQuotaDefinitionId { get; }
}
