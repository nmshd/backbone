using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Tests.Extensions;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;
public class DecomposeRelationshipTests
{
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

    [Fact]
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
