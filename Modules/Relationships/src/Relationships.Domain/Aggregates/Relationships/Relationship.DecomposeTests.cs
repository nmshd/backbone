using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipDecomposeTests : AbstractTestsBase
{
    [Fact]
    public void Decomposing_as_firstParticipant_transitions_relationship_to_status_DeletionProposed()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship(IDENTITY_1, IDENTITY_2);

        // Act
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.ShouldBe(RelationshipStatus.DeletionProposed);
    }

    [Fact]
    public void Decomposing_as_second_participant_transitions_relationship_to_status_ReadyForDeletion()
    {
        // Arrange
        var relationship = CreateRelationshipDecomposedByFrom(IDENTITY_1, IDENTITY_2);

        // Act
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.ShouldBe(RelationshipStatus.ReadyForDeletion);
    }

    [Fact]
    public void Raises_RelationshipStatusChangedDomainEvent()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship(IDENTITY_1, IDENTITY_2);
        relationship.ClearDomainEvents();

        // Act
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        var domainEvent = relationship.ShouldHaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.ShouldBe(relationship.Id);
        domainEvent.NewStatus.ShouldBe(relationship.Status.ToString());
        domainEvent.Initiator.ShouldBe(relationship.LastModifiedBy);
        domainEvent.Peer.ShouldBe(relationship.GetPeerOf(relationship.LastModifiedBy));
    }

    [Fact]
    public void Can_only_decompose_when_relationship_is_in_status_Terminated_or_DeletionProposed()
    {
        // Arrange
        var relationship = CreatePendingRelationship(IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.Decompose(IDENTITY_1, DEVICE_1);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError(
            "error.platform.validation.relationship.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.DeletionProposed)
        );
    }


    [Theory]
    [InlineData(RelationshipStatus.Pending)]
    [InlineData(RelationshipStatus.Active)]
    public void Can_only_decompose_as_firstParticipant_when_relationship_is_in_proper_status(RelationshipStatus invalidStatus)
    {
        // Arrange
        var relationship = CreateRelationshipInStatus(invalidStatus, IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.Decompose(IDENTITY_1, DEVICE_1);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError(
            "error.platform.validation.relationship.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Terminated)
        );
    }

    [Fact]
    public void Decomposing_as_firstParticipant_creates_an_AuditLog_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");
        var relationship = CreateTerminatedRelationship(IDENTITY_1, IDENTITY_2);

        // Act
        relationship.Decompose(IDENTITY_1, DEVICE_1);

        // Assert
        relationship.AuditLog.ShouldHaveCount(4);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.ShouldNotBeNull();
        auditLogEntry.Reason.ShouldBe(RelationshipAuditLogEntryReason.Decomposition);
        auditLogEntry.OldStatus.ShouldBe(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.ShouldBe(RelationshipStatus.DeletionProposed);
        auditLogEntry.CreatedBy.ShouldBe(IDENTITY_1);
        auditLogEntry.CreatedByDevice.ShouldBe(DEVICE_1);
        auditLogEntry.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Decomposing_as_secondParticipant_creates_an_AuditLog_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreateRelationshipDecomposedByFrom(IDENTITY_1, IDENTITY_2);

        // Act
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.AuditLog.ShouldHaveCount(5); // AuditLog(Creation->Acceptance->Termination->Decomposition->Decomposition)

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.ShouldNotBeNull();
        auditLogEntry.Reason.ShouldBe(RelationshipAuditLogEntryReason.Decomposition);
        auditLogEntry.OldStatus.ShouldBe(RelationshipStatus.DeletionProposed);
        auditLogEntry.NewStatus.ShouldBe(RelationshipStatus.ReadyForDeletion);
        auditLogEntry.CreatedBy.ShouldBe(IDENTITY_2);
        auditLogEntry.CreatedByDevice.ShouldBe(DEVICE_2);
        auditLogEntry.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Identity_must_belong_to_relationship_to_decompose_it()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship(IDENTITY_1, IDENTITY_2);
        var externalIdentity = CreateRandomIdentityAddress();
        var externalDeviceId = DeviceId.New();

        // Act
        var acting = () => relationship.Decompose(externalIdentity, externalDeviceId);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.requestingIdentityDoesNotBelongToRelationship");
    }

    [Fact]
    public void Identity_from_can_only_decompose_once()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship(IDENTITY_1, IDENTITY_2);
        relationship.Decompose(IDENTITY_1, DEVICE_1);

        // Act
        var acting = () => relationship.Decompose(IDENTITY_1, DEVICE_1);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.relationshipAlreadyDecomposed");
    }

    [Fact]
    public void Identity_to_can_only_decompose_once()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship(IDENTITY_1, IDENTITY_2);
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Act
        var acting = () => relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.relationshipAlreadyDecomposed");
    }
}
