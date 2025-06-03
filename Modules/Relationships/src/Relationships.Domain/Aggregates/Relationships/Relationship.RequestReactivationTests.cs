using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
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
        relationship.AuditLog.ShouldHaveCount(4);

        var auditLogEntry = relationship.AuditLog.OrderBy(a => a.CreatedAt).Last();

        auditLogEntry.Id.ShouldNotBeNull();
        auditLogEntry.Reason.ShouldBe(RelationshipAuditLogEntryReason.ReactivationRequested);
        auditLogEntry.OldStatus.ShouldBe(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.ShouldBe(RelationshipStatus.Terminated);
        auditLogEntry.CreatedBy.ShouldBe(IDENTITY_2);
        auditLogEntry.CreatedByDevice.ShouldBe(DEVICE_2);
        auditLogEntry.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
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
        var domainEvent = relationship.ShouldHaveASingleDomainEvent<RelationshipReactivationRequestedDomainEvent>();
        domainEvent.RelationshipId.ShouldBe(relationship.Id);
        domainEvent.RequestingIdentity.ShouldBe(IDENTITY_2);
        domainEvent.Peer.ShouldBe(IDENTITY_1);
    }

    [Fact]
    public void Can_only_request_relationship_reactivation_when_relationship_is_in_status_terminated()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.RequestReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError(
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
        acting.ShouldThrow<DomainException>().ShouldHaveError(
            "error.platform.validation.relationship.cannotRequestReactivationWhenThereIsAnOpenReactivationRequest");
    }
}
