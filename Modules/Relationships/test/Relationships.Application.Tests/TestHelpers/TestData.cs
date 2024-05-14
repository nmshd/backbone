using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.UnitTestTools.Data;

namespace Backbone.Modules.Relationships.Application.Tests.TestHelpers;

public static class TestData
{
    public static Relationship CreatePendingRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        from ??= TestDataGenerator.CreateRandomIdentityAddress();
        return new Relationship(CreateRelationshipTemplate(createdBy: to), from, TestDataGenerator.CreateRandomDeviceId(), null, []);
    }

    public static RelationshipTemplate CreateRelationshipTemplate(IdentityAddress? createdBy = null)
    {
        createdBy ??= TestDataGenerator.CreateRandomIdentityAddress();
        return new RelationshipTemplate(createdBy, TestDataGenerator.CreateRandomDeviceId(), 1, null, []);
    }

    public static Relationship CreateActiveRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        from ??= TestDataGenerator.CreateRandomIdentityAddress();
        to ??= TestDataGenerator.CreateRandomIdentityAddress();

        var relationship = new Relationship(CreateRelationshipTemplate(createdBy: to), from, TestDataGenerator.CreateRandomDeviceId(), null, []);
        relationship.Accept(to, TestDataGenerator.CreateRandomDeviceId(), null);

        return relationship;
    }

    public static Relationship CreateTerminatedRelationship()
    {
        var relationship = CreateActiveRelationship();
        relationship.Terminate(relationship.From, TestDataGenerator.CreateRandomDeviceId());

        return relationship;
    }

    public static Relationship CreateTerminatedRelationship(IdentityAddress activeIdentity, IdentityAddress? to = null)
    {
        to ??= TestDataGenerator.CreateRandomIdentityAddress();

        var relationship = CreateActiveRelationship(activeIdentity, to);
        relationship.Terminate(relationship.From, TestDataGenerator.CreateRandomDeviceId());

        return relationship;
    }

    public static Relationship CreateRelationshipWithRequestedReactivation(IdentityAddress from, IdentityAddress to, IdentityAddress reactivationRequestedBy)
    {
        var relationship = CreateTerminatedRelationship(from, to);
        relationship.RequestReactivation(reactivationRequestedBy, TestDataGenerator.CreateRandomDeviceId());

        return relationship;
    }

    public static Relationship CreateRelationshipWithRequestedReactivation(IdentityAddress activeIdentity)
    {
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();

        var relationship = CreateActiveRelationship(to: activeIdentity);
        relationship.Terminate(activeIdentity, activeDevice);
        relationship.RequestReactivation(activeIdentity, activeDevice);

        return relationship;
    }
}
