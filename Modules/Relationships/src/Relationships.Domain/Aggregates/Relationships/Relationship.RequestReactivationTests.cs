using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipRequestReactivationTests : AbstractTestsBase
{
    [Fact]
    public void Requesting_reactivation_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");
        var relationship = CreateTerminatedRelationship();

        // Act
        relationship.RequestReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.AuditLog.Should().HaveCount(4);

        var auditLogEntry = relationship.AuditLog.OrderBy(a => a.CreatedAt).Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.ReactivationRequested);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_2);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_2);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Raises_RelationshipReactivationRequestedDomainEvent()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship();
        relationship.ClearDomainEvents();

        // Act
        relationship.RequestReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<RelationshipReactivationRequestedDomainEvent>();
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.RequestingIdentity.Should().Be(IDENTITY_2);
        domainEvent.Peer.Should().Be(IDENTITY_1);
    }

    [Fact]
    public void Can_only_request_relationship_reactivation_when_relationship_is_in_status_terminated()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.RequestReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationship.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Terminated)
        );
    }

    [Fact]
    public void Cannot_request_reactivation_when_there_is_an_open_reactivation_request()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship();

        relationship.RequestReactivation(IDENTITY_2, DEVICE_2);

        // Act
        var acting = () => relationship.RequestReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationship.cannotRequestReactivationWhenThereIsAnOpenReactivationRequest");
    }
}
