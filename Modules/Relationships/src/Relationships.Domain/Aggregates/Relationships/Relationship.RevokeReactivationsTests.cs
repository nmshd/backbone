using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipRevokeReactivationTests : AbstractTestsBase
{
    [Fact]
    public void Revoking_relationship_reactivation_request_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");
        var relationship = CreateRelationshipWithRequestedReactivation(from: IDENTITY_1, to: IDENTITY_2, reactivationRequestedBy: IDENTITY_1);

        // Act
        relationship.RevokeReactivation(IDENTITY_1, DEVICE_1);

        // Assert
        relationship.AuditLog.ShouldHaveCount(5);

        var auditLogEntry = relationship.AuditLog.OrderBy(a => a.CreatedAt).Last();

        auditLogEntry.Id.ShouldNotBeNull();
        auditLogEntry.Reason.ShouldBe(RelationshipAuditLogEntryReason.RevocationOfReactivation);
        auditLogEntry.OldStatus.ShouldBe(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.ShouldBe(RelationshipStatus.Terminated);
        auditLogEntry.CreatedBy.ShouldBe(IDENTITY_1);
        auditLogEntry.CreatedByDevice.ShouldBe(DEVICE_1);
        auditLogEntry.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Raises_RelationshipReactivationCompletedDomainEvent()
    {
        // Arrange
        var relationship = CreateRelationshipWithRequestedReactivation(IDENTITY_1, IDENTITY_2, IDENTITY_1);
        relationship.ClearDomainEvents();

        // Act
        relationship.RevokeReactivation(IDENTITY_1, DEVICE_1);

        // Assert
        var domainEvent = relationship.ShouldHaveASingleDomainEvent<RelationshipReactivationCompletedDomainEvent>();
        domainEvent.RelationshipId.ShouldBe(relationship.Id);
        domainEvent.Peer.ShouldBe(IDENTITY_2);
    }

    [Fact]
    public void Can_only_revoke_reactivation_of_relationship_when_reactivation_has_been_requested()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship();

        // Act
        var acting = () => relationship.RevokeReactivation(IDENTITY_1, DEVICE_1);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError(
            "error.platform.validation.relationship.noRevocableReactivationRequestExists"
        );
    }

    [Fact]
    public void Can_only_revoke_reactivation_of_relationship_when_reactivation_has_been_requested_by_self()
    {
        // Arrange
        var relationship = CreateRelationshipWithRequestedReactivation(from: IDENTITY_1, to: IDENTITY_2, reactivationRequestedBy: IDENTITY_1);

        // Act
        var acting = () => relationship.RevokeReactivation(IDENTITY_2, DEVICE_1);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError(
            "error.platform.validation.relationship.noRevocableReactivationRequestExists"
        );
    }
}
