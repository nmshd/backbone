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

    public static Relationship CreateRelationshipWithDecomposeRequest(IdentityAddress? from = null, IdentityAddress? to = null)
    {
        var relationship = CreateActiveRelationship(from, to);

        // replace with DecomposeRequest when implemented
        relationship.AuditLog.Add(new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.Decomposed,
            RelationshipStatus.Active,
            RelationshipStatus.DeletionProposed,
            from,
            TestDataGenerator.CreateRandomDeviceId()
        ));

        return relationship;
    }
}
