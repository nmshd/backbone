using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
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
        relationship.Status.ShouldBe(RelationshipStatus.Terminated);
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
        relationship.AuditLog.ShouldHaveCount(3);

        var auditLogEntry = relationship.AuditLog.OrderBy(a => a.CreatedAt).Last();

        auditLogEntry.Id.ShouldNotBeNull();
        auditLogEntry.Reason.ShouldBe(RelationshipAuditLogEntryReason.Termination);
        auditLogEntry.OldStatus.ShouldBe(RelationshipStatus.Active);
        auditLogEntry.NewStatus.ShouldBe(RelationshipStatus.Terminated);
        auditLogEntry.CreatedBy.ShouldBe(IDENTITY_2);
        auditLogEntry.CreatedByDevice.ShouldBe(DEVICE_2);
        auditLogEntry.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
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
        var domainEvent = relationship.ShouldHaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.ShouldBe(relationship.Id);
        domainEvent.NewStatus.ShouldBe(relationship.Status.ToString());
        domainEvent.Initiator.ShouldBe(relationship.LastModifiedBy);
        domainEvent.Peer.ShouldBe(relationship.GetPeerOf(relationship.LastModifiedBy));
    }

    [Fact]
    public void Can_only_terminate_relationship_when_relationship_is_in_status_active()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.Terminate(IDENTITY_2, DEVICE_2);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError(
            "error.platform.validation.relationship.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Active)
        );
    }
}
