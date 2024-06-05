using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierOfIdentityChanged;

public class TierOfIdentityChangedDomainEvent : DomainEvent
{
    public required string OldTierId { get; set; }
    public required string NewTierId { get; set; }
    public required string IdentityAddress { get; set; }
}
