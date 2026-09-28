using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class PeerDeletedDomainEvent : DomainEvent
{
    public required string PeerOfDeletedIdentity { get; set; }
    public required string RelationshipId { get; set; }
    public required string DeletedIdentity { get; set; }
}

public class PeerDeletionCancelledDomainEvent : DomainEvent
{
    public required string PeerOfIdentityWithDeletionCancelled { get; set; }
    public required string RelationshipId { get; set; }
    public required string IdentityWithDeletionCancelled { get; set; }
}

public class PeerFeatureFlagsChangedDomainEvent : DomainEvent
{
    public required string PeerAddress { get; set; }
    public required string NotifiedIdentityAddress { get; set; }
}

public class PeerToBeDeletedDomainEvent : DomainEvent
{
    public required string PeerOfIdentityToBeDeleted { get; set; }
    public required string RelationshipId { get; set; }
    public required string IdentityToBeDeleted { get; set; }
    public required DateTime GracePeriodEndsAt { get; set; }
}

public class RelationshipReactivationCompletedDomainEvent : DomainEvent
{
    public required string RelationshipId { get; set; }
    public required string NewRelationshipStatus { get; set; }
    public required string Peer { get; set; }
}

public class RelationshipReactivationRequestedDomainEvent : DomainEvent
{
    public required string RelationshipId { get; set; }
    public required string RequestingIdentity { get; set; }
    public required string Peer { get; set; }
}

public class RelationshipStatusChangedDomainEvent : DomainEvent
{
    public required string RelationshipId { get; set; }
    public required string NewStatus { get; set; }
    public required string Initiator { get; set; }
    public required string Peer { get; set; }
    public required bool WasDueToIdentityDeletion { get; set; }
}

public class RelationshipTemplateAllocationsExhaustedDomainEvent : DomainEvent
{
    public required string RelationshipTemplateId { get; set; }
    public required string CreatedBy { get; set; }
}

public class RelationshipTemplateCreatedDomainEvent : DomainEvent
{
    public required string TemplateId { get; set; }
    public required string CreatedBy { get; set; }
}
