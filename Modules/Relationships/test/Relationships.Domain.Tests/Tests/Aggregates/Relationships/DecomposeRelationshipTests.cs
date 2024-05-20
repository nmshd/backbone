using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Tests.Extensions;
using Backbone.Tooling;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;

public class DecomposeRelationshipTests
{
    [Fact]
    public void Decomposing_relationship_transitions_relationship_to_status_deletion_proposed()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship(IDENTITY_1, IDENTITY_2);

        // Act
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.DeletionProposed);
    }

    [Fact]
    public void Can_only_decompose_when_relationship_is_in_status_terminated()
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
    public void Decomposing_relationship_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");
        var relationship = CreateTerminatedRelationship(IDENTITY_1, IDENTITY_2);

        var relationship = CreateRelationshipInDecompositionByFirstParticipant();

        // Act
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.AuditLog.Should().HaveCount(4);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.Decomposed);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.DeletionProposed);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_2);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_2);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Only_a_identity_belonging_to_the_relationship_can_decompose_it()
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

    // todo: sort these other tests
    [Fact]
    public void Decomposing_as_second_participant_transitions_relationship_to_status_ReadyForDeletion()
    {
        // Arrange
        var relationship = CreateRelationshipInDecompositionByFirstParticipant();

        // Act
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.ReadyForDeletion);
    }

    public void Decompose_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreateRelationshipInDecompositionByFirstParticipant();

        // Act
        relationship.Decompose(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.AuditLog.Should().HaveCount(4); // AuditLog(Creation->Acceptance->Decomposition->Decomposition)

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
    public void Decompose_throws_if_activeIdentity_alreadyDecomposed()
    {
        // Arrange
        var relationship = CreateRelationshipInDecompositionByFirstParticipant();

        // Act
        var acting = () => relationship.Decompose(IDENTITY_1, DEVICE_1);

        // Assert
        acting.Should().Throw<DomainException>()
            .WithError("error.platform.validation.decompose.activeIdentityAlreadyDecomposed");
    }
}
