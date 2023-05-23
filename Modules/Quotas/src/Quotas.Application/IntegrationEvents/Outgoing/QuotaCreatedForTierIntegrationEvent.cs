using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;

public class QuotaCreatedForTierIntegrationEvent : IntegrationEvent
{
    public QuotaCreatedForTierIntegrationEvent(Tier tier, TierQuotaDefinition quota) : base($"{tier.Id}/Created")
    {
        TierId = tier.Id;
        TierName = tier.Name;
        Quota = quota;
    }

    public string TierId { get; }
    public string TierName { get; }
    public TierQuotaDefinition Quota { get; }
}
