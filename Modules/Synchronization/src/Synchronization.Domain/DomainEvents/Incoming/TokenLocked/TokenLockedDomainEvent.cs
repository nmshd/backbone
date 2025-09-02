using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.TokenLocked;

public class TokenLockedDomainEvent : DomainEvent
{
    public required string TokenId { get; set; }
    public required string? CreatedBy { get; set; }
}
