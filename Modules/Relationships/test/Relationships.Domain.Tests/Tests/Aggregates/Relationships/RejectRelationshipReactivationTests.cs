using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Tests.Extensions;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;
public class RejectRelationshipReactivationTests
{
    [Fact]
    public void RejectReactivation_leaves_relationship_in_status_terminated()
    {
        // Arrange
        var relationship = CreateActiveRelationship();
        relationship.Test_SetStatusAsTerminated();

        var auditLogEntry = new RelationshipAuditLogEntry(
            RelationshipAuditLogEntryReason.Reactivation,
            RelationshipStatus.Terminated,
            RelationshipStatus.Terminated,
            IDENTITY_1,
            DEVICE_1
        );
        relationship.AuditLog.Add(auditLogEntry);

        // Act
        relationship.RejectReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Terminated);
    }

    [Fact]
    public void RejectReactivation_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreateActiveRelationship();
        relationship.Test_SetStatusAsTerminated();

        // Act
        relationship.RejectReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.AuditLog.Should().HaveCount(3);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.RejectionOfReactivation);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_2);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_2);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Can_only_reject_relationship_reactivation_request_addressed_to_self()
    {
        // Arrange
        var relationship = CreateActiveRelationship();
        relationship.Test_SetStatusAsTerminated();

        // Act
        var acting = () => relationship.RejectReactivation(IDENTITY_1, DEVICE_1);

        // Assert
        acting.Should().Throw<DomainException>()
            .WithError("error.platform.validation.relationshipRequest.cannotRejectRelationshipReactivationRequestAddressedToSomeoneElse");
    }

    [Fact]
    public void Can_only_reject_reactivation_when_relationship_is_in_status_terminated()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        var acting = () => relationship.RejectReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Terminated)
        );
    }
}
