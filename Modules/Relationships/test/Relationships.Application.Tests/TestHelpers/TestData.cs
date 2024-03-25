using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.UnitTestTools.Data;

namespace Backbone.Modules.Relationships.Application.Tests.TestHelpers;

public static class TestData
{
    public static Relationship CreatePendingRelationship(IdentityAddress? to = null)
    {
        return new Relationship(CreateRelationshipTemplate(createdBy: to), TestDataGenerator.CreateRandomIdentityAddress(), TestDataGenerator.CreateRandomDeviceId(), null, []);
    }

    public static RelationshipTemplate CreateRelationshipTemplate(IdentityAddress? createdBy = null)
    {
        createdBy ??= TestDataGenerator.CreateRandomIdentityAddress();
        return new RelationshipTemplate(createdBy, TestDataGenerator.CreateRandomDeviceId(), 1, null, []);
    }

    public static Relationship CreateActiveRelationship()
    {
        var relationship = new Relationship(CreateRelationshipTemplate(), TestDataGenerator.CreateRandomIdentityAddress(), TestDataGenerator.CreateRandomDeviceId(), null, []);
        relationship.Accept(TestDataGenerator.CreateRandomIdentityAddress(), TestDataGenerator.CreateRandomDeviceId());
        return relationship;
    }

    public static Relationship CreateRejectedRelationship()
    {
        var relationship = new Relationship(CreateRelationshipTemplate(), TestDataGenerator.CreateRandomIdentityAddress(), TestDataGenerator.CreateRandomDeviceId(), null, []);
        relationship.Reject(TestDataGenerator.CreateRandomIdentityAddress(), TestDataGenerator.CreateRandomDeviceId());
        return relationship;
    }

    public static Relationship CreateRevokedRelationship()
    {
        var relationship = new Relationship(CreateRelationshipTemplate(), TestDataGenerator.CreateRandomIdentityAddress(), TestDataGenerator.CreateRandomDeviceId(), null, []);
        relationship.Revoke(TestDataGenerator.CreateRandomIdentityAddress(), TestDataGenerator.CreateRandomDeviceId());
        return relationship;
    }
}
