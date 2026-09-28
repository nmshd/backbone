using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Tokens.Contracts.DomainEvents;

public class TokenCreatedDomainEvent : DomainEvent
{
    public required string TokenId { get; set; }
    public string? CreatedBy { get; set; }
}

public class TokenLockedDomainEvent : DomainEvent
{
    public required string TokenId { get; set; }
    public string? CreatedBy { get; set; }
}
