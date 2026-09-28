using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Contracts.DomainEvents;

public class TierQuotaDefinitionDeletedDomainEvent : DomainEvent
{
    public required string TierId { get; set; }
    public required string TierQuotaDefinitionId { get; set; }
}
