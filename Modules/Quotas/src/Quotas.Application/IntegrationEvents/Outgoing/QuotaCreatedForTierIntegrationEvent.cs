using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;

public class QuotaCreatedForTierIntegrationEvent : IntegrationEvent
{
    public QuotaCreatedForTierIntegrationEvent(Tier tier, TierQuotaDefinition tierQuotaDefinition) : base($"{tier.Id}/Created")
    {
        TierId = tier.Id;
        TierQuotaDefinitionId = tierQuotaDefinition.Id;
    }

    public string TierId { get; }
    public string TierQuotaDefinitionId { get; }
}
