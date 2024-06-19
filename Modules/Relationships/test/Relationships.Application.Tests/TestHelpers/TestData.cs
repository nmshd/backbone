using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.UnitTestTools.Data;

namespace Backbone.Modules.Relationships.Application.Tests.TestHelpers;

public static class TestData
{
    public static RelationshipTemplate CreateRelationshipTemplate(IdentityAddress? createdBy = null)
    {
        createdBy ??= TestDataGenerator.CreateRandomIdentityAddress();
        return new RelationshipTemplate(createdBy, TestDataGenerator.CreateRandomDeviceId(), 1, null, []);
    }

    public static Relationship CreateActiveRelationship(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        from ??= TestDataGenerator.CreateRandomIdentityAddress();
        to ??= TestDataGenerator.CreateRandomIdentityAddress();

        var relationship = new Relationship(CreateRelationshipTemplate(createdBy: to), from, TestDataGenerator.CreateRandomDeviceId(), []);
        var change = relationship.Changes.First();
        relationship.AcceptChange(change.Id, to, TestDataGenerator.CreateRandomDeviceId(), []);

        return relationship;
    }
}
