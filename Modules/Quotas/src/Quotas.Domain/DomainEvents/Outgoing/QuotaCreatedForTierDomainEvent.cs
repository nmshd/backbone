using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Outgoing;

public class QuotaCreatedForTierDomainEvent : DomainEvent
{
    public QuotaCreatedForTierDomainEvent(string tierId, string tierQuotaDefinitionId) : base($"{tierQuotaDefinitionId}/Created")
    {
        TierId = tierId;
        TierQuotaDefinitionId = tierQuotaDefinitionId;
    }

    public string TierId { get; }
    public string TierQuotaDefinitionId { get; }
}
