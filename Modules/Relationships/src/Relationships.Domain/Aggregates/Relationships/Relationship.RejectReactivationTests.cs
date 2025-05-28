using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipRejectReactivationTests : AbstractTestsBase
{
    [Fact]
    public void RejectReactivation_leaves_relationship_in_status_terminated()
    {
        // Arrange
        var relationship = CreateRelationshipWithRequestedReactivation(from: IDENTITY_1, to: IDENTITY_2, reactivationRequestedBy: IDENTITY_1);

        // Act
        relationship.RejectReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.ShouldBe(RelationshipStatus.Terminated);
    }

    [Fact]
    public void RejectReactivation_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreateRelationshipWithRequestedReactivation(from: IDENTITY_1, to: IDENTITY_2, reactivationRequestedBy: IDENTITY_1);

        // Act
        relationship.RejectReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.AuditLog.ShouldHaveCount(5); // AuditLog(Creation->Acceptance->Termination->Reactivation->Rejection)

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.ShouldNotBeNull();
        auditLogEntry.Reason.ShouldBe(RelationshipAuditLogEntryReason.RejectionOfReactivation);
        auditLogEntry.OldStatus.ShouldBe(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.ShouldBe(RelationshipStatus.Terminated);
        auditLogEntry.CreatedBy.ShouldBe(IDENTITY_2);
        auditLogEntry.CreatedByDevice.ShouldBe(DEVICE_2);
        auditLogEntry.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Raises_RelationshipReactivationCompletedDomainEvent()
    {
        // Arrange
        var relationship = CreateRelationshipWithRequestedReactivation(IDENTITY_1, IDENTITY_2, IDENTITY_1);
        relationship.ClearDomainEvents();

        // Act
        relationship.RejectReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        var domainEvent = relationship.ShouldHaveASingleDomainEvent<RelationshipReactivationCompletedDomainEvent>();
        domainEvent.RelationshipId.ShouldBe(relationship.Id);
        domainEvent.Peer.ShouldBe(IDENTITY_1);
    }

    [Fact]
    public void Can_only_reject_reactivation_when_reactivation_request_has_been_made()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship();

        // Act
        var acting = () => relationship.RejectReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        acting.ShouldThrow<DomainException>()
            .ShouldHaveError("error.platform.validation.relationship.noRejectableReactivationRequestExists");
    }

    [Fact]
    public void Can_not_reject_your_own_relationship_reactivation_request()
    {
        // Arrange
        var relationship = CreateRelationshipWithRequestedReactivation(from: IDENTITY_1, to: IDENTITY_2, reactivationRequestedBy: IDENTITY_1);

        // Act
        var acting = () => relationship.RejectReactivation(IDENTITY_1, DEVICE_1);

        // Assert
        acting.ShouldThrow<DomainException>()
            .ShouldHaveError("error.platform.validation.relationship.noRejectableReactivationRequestExists");
    }
}
