using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Devices.Domain.DomainEvents.Incoming.TokenLocked;

public class TokenLockedDomainEvent : DomainEvent
{
    public required string TokenId { get; set; }
    public required string? CreatedBy { get; set; }
}
