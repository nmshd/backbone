using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.TestHelpers;

public static class TestData
{
    public static Relationship CreatePendingRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        from ??= CreateRandomIdentityAddress();
        return new Relationship(CreateRelationshipTemplate(createdBy: to), from, CreateRandomDeviceId(), null, []);
    }

    public static RelationshipTemplate CreateRelationshipTemplate(IdentityAddress? createdBy = null)
    {
        createdBy ??= CreateRandomIdentityAddress();
        return new RelationshipTemplate(createdBy, CreateRandomDeviceId(), 1, null, []);
    }

    public static Relationship CreateActiveRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        from ??= CreateRandomIdentityAddress();
        to ??= CreateRandomIdentityAddress();

        var relationship = new Relationship(CreateRelationshipTemplate(createdBy: to), from, CreateRandomDeviceId(), null, []);
        relationship.Accept(to, CreateRandomDeviceId(), null);

        relationship.ClearDomainEvents();

        return relationship;
    }

    public static Relationship CreateTerminatedRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        from ??= CreateRandomIdentityAddress();
        to ??= CreateRandomIdentityAddress();

        var relationship = CreateActiveRelationship(from, to);
        relationship.Terminate(relationship.From, CreateRandomDeviceId());

        return relationship;
    }

    public static Relationship CreateRelationshipWithRequestedReactivation(IdentityAddress from, IdentityAddress to, IdentityAddress reactivationRequestedBy)
    {
        var relationship = CreateTerminatedRelationship(from, to);
        relationship.RequestReactivation(reactivationRequestedBy, CreateRandomDeviceId());

        return relationship;
    }
}
