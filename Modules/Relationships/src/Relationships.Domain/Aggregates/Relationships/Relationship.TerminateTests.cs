using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipTerminateTests : AbstractTestsBase
{
    [Fact]
    public void Terminating_relationship_transitions_relationship_to_status_terminated()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        relationship.Terminate(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Terminated);
    }

    [Fact]
    public void Terminating_relationship_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreateActiveRelationship();

        // Act
        relationship.Terminate(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.AuditLog.Should().HaveCount(3);

        var auditLogEntry = relationship.AuditLog.OrderBy(a => a.CreatedAt).Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.Termination);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Active);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_2);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_2);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Raises_RelationshipStatusChangedDomainEvent()
    {
        // Assert
        var relationship = CreateActiveRelationship();
        relationship.ClearDomainEvents();

        // Act
        relationship.Terminate(IDENTITY_2, DEVICE_2);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.NewStatus.Should().Be(relationship.Status.ToString());
        domainEvent.Initiator.Should().Be(relationship.LastModifiedBy);
        domainEvent.Peer.Should().Be(relationship.GetPeerOf(relationship.LastModifiedBy));
    }

    [Fact]
    public void Can_only_terminate_relationship_when_relationship_is_in_status_active()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.Terminate(IDENTITY_2, DEVICE_2);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationship.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Active)
        );
    }
}
