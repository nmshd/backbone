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

    public static Relationship CreateActiveRelationship()
    {
        var relationship = new Relationship(CreateRelationshipTemplate(), TestDataGenerator.CreateRandomIdentityAddress(), TestDataGenerator.CreateRandomDeviceId(), null, []);
        relationship.Accept(TestDataGenerator.CreateRandomIdentityAddress(), TestDataGenerator.CreateRandomDeviceId(), []);
        return relationship;
    }

    public static Relationship CreateTerminatedRelationship()
    {
        var identity1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identity2 = TestDataGenerator.CreateRandomIdentityAddress();

        var relationshipTemplate = CreateRelationshipTemplate(identity1);
        var relationship = new Relationship(relationshipTemplate, identity2, TestDataGenerator.CreateRandomDeviceId(), null, []);

        relationship.Accept(identity1, TestDataGenerator.CreateRandomDeviceId());
        relationship.Terminate(identity1, TestDataGenerator.CreateRandomDeviceId());

        return relationship;
    }
}
