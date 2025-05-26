using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipRevokeTests : AbstractTestsBase
{
    [Fact]
    public void Revoke_creation_transitions_relationship_to_status_revoked()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Revoke(IDENTITY_1, DEVICE_1, null);

        // Assert
        relationship.Status.ShouldBe(RelationshipStatus.Revoked);
    }

    [Fact]
    public void Revoking_creation_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreatePendingRelationship();

        // Act
        relationship.Revoke(IDENTITY_1, DEVICE_1, null);

        // Assert
        relationship.AuditLog.ShouldHaveCount(2);

        var auditLogEntry = relationship.AuditLog.OrderBy(a => a.CreatedAt).Last();

        auditLogEntry.Id.ShouldNotBeNull();
        auditLogEntry.Reason.ShouldBe(RelationshipAuditLogEntryReason.RevocationOfCreation);
        auditLogEntry.OldStatus.ShouldBe(RelationshipStatus.Pending);
        auditLogEntry.NewStatus.ShouldBe(RelationshipStatus.Revoked);
        auditLogEntry.CreatedBy.ShouldBe(IDENTITY_1);
        auditLogEntry.CreatedByDevice.ShouldBe(DEVICE_1);
        auditLogEntry.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Raises_RelationshipStatusChangedDomainEvent()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Revoke(IDENTITY_1, DEVICE_1, null);

        // Assert
        var domainEvent = relationship.ShouldHaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.ShouldBe(relationship.Id);
        domainEvent.NewStatus.ShouldBe(relationship.Status.ToString());
        domainEvent.Initiator.ShouldBe(relationship.LastModifiedBy);
        domainEvent.Peer.ShouldBe(relationship.GetPeerOf(relationship.LastModifiedBy));
    }

    [Fact]
    public void Can_only_revoke_creation_when_relationship_is_in_status_pending()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        var acting = () => relationship.Revoke(IDENTITY_2, DEVICE_2, null);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError(
            "error.platform.validation.relationship.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Pending)
        );
    }

    [Fact]
    public void Cannot_revoke_own_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.Revoke(IDENTITY_2, DEVICE_2, null);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.cannotRevokeRelationshipNotCreatedByYourself");
    }

    [Fact]
    public void Cannot_revoke_foreign_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var foreignAddress = IdentityAddress.ParseUnsafe("some-other-identity");

        // Act
        var acting = () => relationship.Revoke(foreignAddress, DeviceId.New(), null);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.cannotRevokeRelationshipNotCreatedByYourself");
    }
}
