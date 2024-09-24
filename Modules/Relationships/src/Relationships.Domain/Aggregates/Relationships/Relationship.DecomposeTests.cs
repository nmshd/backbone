using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using Backbone.UnitTestTools.Extensions;
using FluentAssertions;
using Xunit;
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
        relationship.Status.Should().Be(RelationshipStatus.DeletionProposed);
    }

    [Fact]
    public void Decomposing_as_second_participant_transitions_relationship_to_status_ReadyForDeletion()
    {
        // Arrange
        var relationship = CreateRelationshipDecomposedByFrom(IDENTITY_1, IDENTITY_2);

        // Act
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.ReadyForDeletion);
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
        var domainEvent = relationship.Should().HaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.NewStatus.Should().Be(relationship.Status.ToString());
        domainEvent.Initiator.Should().Be(relationship.LastModifiedBy);
        domainEvent.Peer.Should().Be(relationship.GetPeerOf(relationship.LastModifiedBy));
    }

    [Fact]
    public void Can_only_decompose_when_relationship_is_in_status_Terminated_or_DeletionProposed()
    {
        // Arrange
        var relationship = CreatePendingRelationship(IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.Decompose(IDENTITY_1, DEVICE_1);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.DeletionProposed)
        );
    }

    [Fact]
    public void Can_only_decompose_as_firstParticipant_when_relationship_is_in_status_Terminated()
    {
        // Arrange
        var relationship = CreatePendingRelationship(IDENTITY_1, IDENTITY_2);

        // Act
        var acting = () => relationship.Decompose(IDENTITY_1, DEVICE_1);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
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
        relationship.AuditLog.Should().HaveCount(4);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.Decomposition);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.DeletionProposed);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_1);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_1);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
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
        relationship.AuditLog.Should().HaveCount(5); // AuditLog(Creation->Acceptance->Termination->Decomposition->Decomposition)

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.Decomposition);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.DeletionProposed);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.ReadyForDeletion);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_2);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_2);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Identity_must_belong_to_relationship_to_decompose_it()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship(IDENTITY_1, IDENTITY_2);
        var externalIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var externalDeviceId = DeviceId.New();

        // Act
        var acting = () => relationship.Decompose(externalIdentity, externalDeviceId);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.requestingIdentityDoesNotBelongToRelationship");
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
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.relationshipAlreadyDecomposed");
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
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.relationshipAlreadyDecomposed");
    }
}
