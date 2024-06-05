using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Outgoing;

public class TierQuotaDefinitionDeletedDomainEvent : DomainEvent
{
    public TierQuotaDefinitionDeletedDomainEvent(string tierId, string tierQuotaDefinitionId) : base($"{tierQuotaDefinitionId}/Deleted")
    {
        TierId = tierId;
        TierQuotaDefinitionId = tierQuotaDefinitionId;
    }

    public string TierId { get; }
    public string TierQuotaDefinitionId { get; }
}
