using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipCreateTests : AbstractTestsBase
{
    [Fact]
    public void New_Relationship_Has_Correct_Data()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        // Act
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, [0, 1, 2], []);

        // Assert
        relationship.Id.ShouldNotBeNull();
        relationship.From.ShouldBe(IDENTITY_1);
        relationship.To.ShouldBe(IDENTITY_2);
        relationship.Status.ShouldBe(RelationshipStatus.Pending);
        relationship.RelationshipTemplateId.ShouldBe(RELATIONSHIP_TEMPLATE_OF_2.Id);
        relationship.RelationshipTemplate.ShouldBe(RELATIONSHIP_TEMPLATE_OF_2);
        relationship.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
        relationship.Details.CreationContent.ShouldBe([0, 1, 2]);
    }

    [Fact]
    public void Raises_RelationshipStatusChangedDomainEvent()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        // Act
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, [0, 1, 2], []);

        // Assert
        var domainEvent = relationship.ShouldHaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.ShouldBe(relationship.Id);
        domainEvent.NewStatus.ShouldBe("Pending");
        domainEvent.Initiator.ShouldBe(IDENTITY_1);
        domainEvent.Peer.ShouldBe(IDENTITY_2);
    }

    [Fact]
    public void New_relationship_has_AuditLogEntry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        // Act
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, null, []);

        // Assert
        relationship.AuditLog.ShouldHaveCount(1);

        var auditLogEntry = relationship.AuditLog.First();

        auditLogEntry.Id.ShouldNotBeNull();
        auditLogEntry.Reason.ShouldBe(RelationshipAuditLogEntryReason.Creation);
        auditLogEntry.OldStatus.ShouldBe(null);
        auditLogEntry.NewStatus.ShouldBe(RelationshipStatus.Pending);
        auditLogEntry.CreatedBy.ShouldBe(IDENTITY_1);
        auditLogEntry.CreatedByDevice.ShouldBe(DEVICE_1);
        auditLogEntry.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Cannot_create_Relationship_to_self()
    {
        // Act
        var acting = () => new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_1, DEVICE_1, null, []);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.cannotCreateRelationshipWithYourself");
    }

    [Theory]
    [InlineData(RelationshipStatus.Pending)]
    [InlineData(RelationshipStatus.Active)]
    [InlineData(RelationshipStatus.Terminated)]
    [InlineData(RelationshipStatus.DeletionProposed)]
    public void New_relationship_cannot_be_created_when_there_is_an_existing_one_in_status_x(RelationshipStatus relationshipStatus)
    {
        // Arrange
        var existingRelationships = new List<Relationship>
        {
            CreateRelationshipInStatus(relationshipStatus)
        };

        // Act
        var acting = () => new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, null, existingRelationships);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.relationshipToTargetAlreadyExists");
    }

    [Fact]
    public void New_relationship_can_be_created_when_existing_ones_are_rejected_revoked_or_ready_for_deletion()
    {
        // Arrange
        var existingRelationships = new List<Relationship>
        {
            CreateRejectedRelationship(),
            CreateRevokedRelationship(),
            CreateRelationshipReadyForDeletion()
        };

        // Act
        var acting = () => new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, null, existingRelationships);

        // Assert
        acting.ShouldNotThrow();
    }
}
