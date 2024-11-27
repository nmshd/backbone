using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
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
        relationship.Status.Should().Be(RelationshipStatus.Active);
        relationship.CreationResponseContent.Should().BeEquivalentTo([0]);
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
        relationship.AuditLog.Should().HaveCount(2);

        var auditLogEntry = relationship.AuditLog.OrderBy(a => a.CreatedAt).Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.AcceptanceOfCreation);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Pending);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Active);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_2);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_2);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Can_only_accept_creation_when_relationship_is_in_status_pending()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        var acting = () => relationship.Accept(IDENTITY_2, DEVICE_2, []);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
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
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationship.cannotAcceptOrRejectRelationshipAddressedToSomeoneElse");
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
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationship.cannotAcceptOrRejectRelationshipAddressedToSomeoneElse");
    }

    [Fact]
    public void Raises_RelationshipStatusChangedDomainEvent()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Accept(IDENTITY_2, DEVICE_2, []);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.NewStatus.Should().Be(relationship.Status.ToString());
        domainEvent.Initiator.Should().Be(relationship.LastModifiedBy);
        domainEvent.Peer.Should().Be(relationship.GetPeerOf(relationship.LastModifiedBy));
    }
}
