using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipAcceptTests : AbstractTestsBase
{
    [Fact]
    public void Accepting_creation_transitions_relationship_to_status_active()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Accept(IDENTITY_2, DEVICE_2, [0]);

        // Assert
        relationship.Status.ShouldBe(RelationshipStatus.Active);
        relationship.CreationResponseContent.ShouldBeEquivalentTo(new byte[] { 0 });
    }

    [Fact]
    public void Accepting_creation_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreatePendingRelationship();

        // Act
        relationship.Accept(IDENTITY_2, DEVICE_2, []);

        // Assert
        relationship.AuditLog.ShouldHaveCount(2);

        var auditLogEntry = relationship.AuditLog.OrderBy(a => a.CreatedAt).Last();

        auditLogEntry.Id.ShouldNotBeNull();
        auditLogEntry.Reason.ShouldBe(RelationshipAuditLogEntryReason.AcceptanceOfCreation);
        auditLogEntry.OldStatus.ShouldBe(RelationshipStatus.Pending);
        auditLogEntry.NewStatus.ShouldBe(RelationshipStatus.Active);
        auditLogEntry.CreatedBy.ShouldBe(IDENTITY_2);
        auditLogEntry.CreatedByDevice.ShouldBe(DEVICE_2);
        auditLogEntry.CreatedAt.ShouldBe(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Can_only_accept_creation_when_relationship_is_in_status_pending()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        var acting = () => relationship.Accept(IDENTITY_2, DEVICE_2, []);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError(
            "error.platform.validation.relationship.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Pending)
        );
    }

    [Fact]
    public void Cannot_accept_own_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.Accept(IDENTITY_1, DEVICE_1, []);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.cannotAcceptOrRejectRelationshipAddressedToSomeoneElse");
    }

    [Fact]
    public void Cannot_accept_foreign_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var foreignAddress = IdentityAddress.ParseUnsafe("some-other-identity");

        // Act
        var acting = () => relationship.Accept(foreignAddress, DeviceId.New(), []);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.validation.relationship.cannotAcceptOrRejectRelationshipAddressedToSomeoneElse");
    }

    [Fact]
    public void Raises_RelationshipStatusChangedDomainEvent()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Accept(IDENTITY_2, DEVICE_2, []);

        // Assert
        var domainEvent = relationship.ShouldHaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.ShouldBe(relationship.Id);
        domainEvent.NewStatus.ShouldBe(relationship.Status.ToString());
        domainEvent.Initiator.ShouldBe(relationship.LastModifiedBy);
        domainEvent.Peer.ShouldBe(relationship.GetPeerOf(relationship.LastModifiedBy));
    }
}
