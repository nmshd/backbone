using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Domain.Tests.TestHelpers;

public static class TestData
{
    public static readonly IdentityAddress IDENTITY_1 = IdentityAddress.Create([1, 1, 1], "id1");
    public static readonly DeviceId DEVICE_1 = DeviceId.New();

    public static readonly IdentityAddress IDENTITY_2 = IdentityAddress.Create([2, 2, 2], "id1");
    public static readonly DeviceId DEVICE_2 = DeviceId.New();

    public static readonly RelationshipTemplate RELATIONSHIP_TEMPLATE_OF_1 = new(IDENTITY_1, DEVICE_1, 1, null, []);
    public static readonly RelationshipTemplate RELATIONSHIP_TEMPLATE_OF_2 = new(IDENTITY_2, DEVICE_2, 1, null, []);

    public static Relationship CreatePendingRelationship()
    {
        return new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, null, []);
    }

    public static Relationship CreateActiveRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        to ??= IDENTITY_2;
        var template = new RelationshipTemplate(to, DEVICE_2, 999, null, []);
        var relationship = new Relationship(template, from ?? IDENTITY_1, DEVICE_1, null, []);
        relationship.Accept(to, DEVICE_2, []);
        return relationship;
    }

    public static Relationship CreateRejectedRelationship()
    {
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, null, []);
        relationship.Reject(IDENTITY_2, DEVICE_2, null);
        return relationship;
    }

    public static Relationship CreateRevokedRelationship()
    {
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, null, []);
        relationship.Revoke(IDENTITY_1, DEVICE_1, null);
        return relationship;
    }

    public static Relationship CreateTerminatedRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        to ??= IDENTITY_2;
        var template = new RelationshipTemplate(to, DEVICE_2, 999, null, []);
        var relationship = new Relationship(template, from ?? IDENTITY_1, DEVICE_1, null, []);
        relationship.Accept(to, DEVICE_2, []);
        relationship.Terminate(IDENTITY_1, DEVICE_1);
        return relationship;
    }

    public static Relationship CreateRelationshipWithRequestedReactivation(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        to ??= IDENTITY_2;
        var template = new RelationshipTemplate(to, DEVICE_2, 999, null, []);
        var relationship = new Relationship(template, from ?? IDENTITY_1, DEVICE_1, null, []);
        relationship.Accept(to, DEVICE_2, []);
        relationship.Terminate(IDENTITY_1, DEVICE_1);
        relationship.RequestReactivation(IDENTITY_1, DEVICE_1);
        return relationship;
    }
}
