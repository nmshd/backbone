using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Relationships.Domain.DomainEvents;

public static class PeerDeletedDomainEventExtensions
{
    extension(PeerDeletedDomainEvent)
    {
        public static PeerDeletedDomainEvent Create(IdentityAddress peerOfDeletedIdentity, RelationshipId relationshipId, IdentityAddress deletedIdentity) => new()
        {
            DomainEventId = $"{relationshipId}/peerDeletionCancelled/{deletedIdentity}",
            PeerOfDeletedIdentity = peerOfDeletedIdentity,
            RelationshipId = relationshipId,
            DeletedIdentity = deletedIdentity
        };
    }
}

public static class PeerDeletionCancelledDomainEventExtensions
{
    extension(PeerDeletionCancelledDomainEvent)
    {
        public static PeerDeletionCancelledDomainEvent Create(
            IdentityAddress peerOfIdentityWithDeletionCancelled,
            RelationshipId relationshipId,
            IdentityAddress identityWithDeletionCancelled) => new()
            {
                DomainEventId = DomainEventIdFactory.Randomize($"{relationshipId}/peerDeletionCancelled/{identityWithDeletionCancelled}"),
                PeerOfIdentityWithDeletionCancelled = peerOfIdentityWithDeletionCancelled,
                RelationshipId = relationshipId,
                IdentityWithDeletionCancelled = identityWithDeletionCancelled
            };
    }
}

public static class PeerFeatureFlagsChangedDomainEventExtensions
{
    extension(PeerFeatureFlagsChangedDomainEvent)
    {
        public static PeerFeatureFlagsChangedDomainEvent Create(string peerAddress, string notifiedIdentityAddress) => new()
        {
            PeerAddress = peerAddress,
            NotifiedIdentityAddress = notifiedIdentityAddress
        };
    }
}

public static class PeerToBeDeletedDomainEventExtensions
{
    extension(PeerToBeDeletedDomainEvent)
    {
        public static PeerToBeDeletedDomainEvent Create(
            IdentityAddress peerOfIdentityToBeDeleted,
            RelationshipId relationshipId,
            IdentityAddress identityToBeDeleted,
            DateTime gracePeriodEndsAt) => new()
            {
                DomainEventId = DomainEventIdFactory.Randomize($"{relationshipId}/peerToBeDeleted/{identityToBeDeleted}"),
                PeerOfIdentityToBeDeleted = peerOfIdentityToBeDeleted,
                RelationshipId = relationshipId,
                IdentityToBeDeleted = identityToBeDeleted,
                GracePeriodEndsAt = gracePeriodEndsAt
            };
    }
}

public static class RelationshipReactivationCompletedDomainEventExtensions
{
    extension(RelationshipReactivationCompletedDomainEvent)
    {
        public static RelationshipReactivationCompletedDomainEvent Create(Relationship relationship, IdentityAddress peer) => new()
        {
            DomainEventId = $"{relationship.Id}/ReactivationCompleted/{relationship.AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedAt}",
            RelationshipId = relationship.Id,
            Peer = peer,
            NewRelationshipStatus = relationship.Status.ToString()
        };
    }
}

public static class RelationshipReactivationRequestedDomainEventExtensions
{
    extension(RelationshipReactivationRequestedDomainEvent)
    {
        public static RelationshipReactivationRequestedDomainEvent Create(Relationship relationship, IdentityAddress requestingIdentity, IdentityAddress peer) => new()
        {
            DomainEventId = $"{relationship.Id}/ReactivationRequested/{relationship.AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedAt}",
            RelationshipId = relationship.Id,
            RequestingIdentity = requestingIdentity.Value,
            Peer = peer.Value
        };
    }
}

public static class RelationshipStatusChangedDomainEventExtensions
{
    extension(RelationshipStatusChangedDomainEvent)
    {
        public static RelationshipStatusChangedDomainEvent Create(Relationship relationship) => new()
        {
            DomainEventId = $"{relationship.Id}/StatusChanged/{relationship.AuditLog.OrderBy(a => a.CreatedAt).Last().CreatedAt.ToUniversalString()}",
            RelationshipId = relationship.Id,
            NewStatus = relationship.Status.ToString(),
            Initiator = relationship.LastModifiedBy,
            Peer = relationship.GetPeerOf(relationship.LastModifiedBy),
            WasDueToIdentityDeletion = false
        };

        public static RelationshipStatusChangedDomainEvent Create(
            string relationshipId,
            string newStatus,
            string initiator,
            string peer,
            bool wasDueToIdentityDeletion) => new()
            {
                RelationshipId = relationshipId,
                NewStatus = newStatus,
                Initiator = initiator,
                Peer = peer,
                WasDueToIdentityDeletion = wasDueToIdentityDeletion
            };
    }
}

public static class RelationshipTemplateAllocationsExhaustedDomainEventExtensions
{
    extension(RelationshipTemplateAllocationsExhaustedDomainEvent)
    {
        public static RelationshipTemplateAllocationsExhaustedDomainEvent Create(RelationshipTemplate template) => new()
        {
            DomainEventId = $"{template.Id}/maxNumberOfAllocationsExhausted",
            RelationshipTemplateId = template.Id,
            CreatedBy = template.CreatedBy
        };
    }
}

public static class RelationshipTemplateCreatedDomainEventExtensions
{
    extension(RelationshipTemplateCreatedDomainEvent)
    {
        public static RelationshipTemplateCreatedDomainEvent Create(RelationshipTemplate template) => new()
        {
            DomainEventId = $"{template.Id}/Created",
            TemplateId = template.Id,
            CreatedBy = template.CreatedBy
        };
    }
}

internal static class DomainEventIdFactory
{
    public static string Randomize(string domainEventId) => domainEventId + "/" + Guid.NewGuid().ToString("N")[..3];
}
