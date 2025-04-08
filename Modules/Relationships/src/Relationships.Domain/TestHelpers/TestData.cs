using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Domain.TestHelpers;

public static class TestData
{
    public static readonly IdentityAddress IDENTITY_1 = IdentityAddress.Create([1, 1, 1], "localhost");
    public static readonly DeviceId DEVICE_1 = DeviceId.New();

    public static readonly IdentityAddress IDENTITY_2 = IdentityAddress.Create([2, 2, 2], "localhost");
    public static readonly DeviceId DEVICE_2 = DeviceId.New();

    public static readonly RelationshipTemplate RELATIONSHIP_TEMPLATE_OF_1 = new(IDENTITY_1, DEVICE_1, 1, null, []);
    public static readonly RelationshipTemplate RELATIONSHIP_TEMPLATE_OF_2 = new(IDENTITY_2, DEVICE_2, 1, null, []);

    public static RelationshipTemplate CreateRelationshipTemplate(IdentityAddress creatorAddress, IdentityAddress? forIdentityAddress = null, byte[]? password = null,
        int? maxNumberOfAllocations = null)
    {
        maxNumberOfAllocations ??= 999;
        return new RelationshipTemplate(creatorAddress, DeviceId.New(), maxNumberOfAllocations, null, [], forIdentityAddress, password);
    }

    public static Relationship CreatePendingRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        to ??= IDENTITY_2;
        from ??= IDENTITY_1;
        var template = new RelationshipTemplate(to, DEVICE_2, 999, null, []);
        var relationship = new Relationship(template, from, DEVICE_1, [], []);
        relationship.ClearDomainEvents();
        return relationship;
    }

    public static Relationship CreateRelationshipInStatus(RelationshipStatus status, IdentityAddress? from = null, IdentityAddress? to = null)
    {
        return status switch
        {
            RelationshipStatus.Pending => CreatePendingRelationship(from, to),
            RelationshipStatus.Rejected => CreateRejectedRelationship(from, to),
            RelationshipStatus.Revoked => CreateRevokedRelationship(),
            RelationshipStatus.Active => CreateActiveRelationship(from, to),
            RelationshipStatus.Terminated => CreateTerminatedRelationship(from, to),
            RelationshipStatus.DeletionProposed => CreateRelationshipWithProposedDeletion(from, to),
            RelationshipStatus.ReadyForDeletion => CreateRelationshipReadyForDeletion(from, to),
            _ => throw new NotSupportedException($"This method currently does not support relationship status {status}.")
        };
    }

    public static Relationship CreateActiveRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        from ??= IDENTITY_1;
        to ??= IDENTITY_2;
        var relationship = CreatePendingRelationship(from, to);
        relationship.Accept(to, DEVICE_2, []);
        relationship.ClearDomainEvents();
        return relationship;
    }

    public static Relationship CreateRejectedRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        from ??= IDENTITY_1;
        to ??= IDENTITY_2;
        var relationship = CreatePendingRelationship(from, to);
        relationship.Reject(to, DEVICE_2, null);
        relationship.ClearDomainEvents();
        return relationship;
    }

    public static Relationship CreateRevokedRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        from ??= IDENTITY_1;
        to ??= IDENTITY_2;
        var relationship = CreatePendingRelationship(from, to);
        relationship.Revoke(from, DEVICE_2, null);
        relationship.ClearDomainEvents();
        return relationship;
    }

    public static Relationship CreateTerminatedRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        var relationship = CreateActiveRelationship(from, to);
        relationship.Terminate(relationship.From, DEVICE_1);
        relationship.ClearDomainEvents();
        return relationship;
    }

    public static Relationship CreateRelationshipWithRequestedReactivation(IdentityAddress from, IdentityAddress to, IdentityAddress reactivationRequestedBy)
    {
        var relationship = CreateTerminatedRelationship(from, to);
        relationship.RequestReactivation(reactivationRequestedBy, DEVICE_1);
        relationship.ClearDomainEvents();
        return relationship;
    }

    public static Relationship CreateRelationshipWithProposedDeletion(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        var relationship = CreateTerminatedRelationship(from, to);
        relationship.Decompose(relationship.From, DEVICE_1);
        relationship.ClearDomainEvents();
        return relationship;
    }

    public static Relationship CreateRelationshipReadyForDeletion(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        var relationship = CreateRelationshipWithProposedDeletion(from, to);
        relationship.Decompose(relationship.To, DEVICE_1);
        relationship.ClearDomainEvents();
        return relationship;
    }

    public static Relationship CreateRelationshipDecomposedByFrom(IdentityAddress from, IdentityAddress to)
    {
        var relationship = CreateTerminatedRelationship(from, to);

        relationship.Decompose(from, DEVICE_1);

        relationship.ClearDomainEvents();

        return relationship;
    }
}
