using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;

public class QuotaCreatedForTierIntegrationEvent : IntegrationEvent
{
    public QuotaCreatedForTierIntegrationEvent(Tier tier, string tierQuotaDefinitionId) : base($"{tier.Id}/Created")
    {
        TierId = tier.Id;
        TierQuotaDefinitionId = tierQuotaDefinitionId;
    }

    public string TierId { get; }
    public string TierQuotaDefinitionId { get; }
}
