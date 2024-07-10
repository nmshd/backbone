using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Relationships.Domain.Tests.Extensions;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;

public class RejectRelationshipTests : AbstractTestsBase
{
    [Fact]
    public void Reject_creation_transitions_relationship_to_status_rejected()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Reject(IDENTITY_2, DEVICE_2, null, [relationship]);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Rejected);
    }

    [Fact]
    public void Rejecting_creation_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreatePendingRelationship();

        // Act
        relationship.Reject(IDENTITY_2, DEVICE_2, null, [relationship]);

        // Assert
        relationship.AuditLog.Should().HaveCount(2);

        var auditLogEntry = relationship.AuditLog.OrderBy(a => a.CreatedAt).Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.RejectionOfCreation);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Pending);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Rejected);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_2);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_2);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Raises_RelationshipStatusChangedDomainEvent()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Reject(IDENTITY_2, DEVICE_2, null, [relationship]);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.NewStatus.Should().Be(relationship.Status.ToString());
        domainEvent.Initiator.Should().Be(relationship.LastModifiedBy);
        domainEvent.Peer.Should().Be(relationship.GetPeerOf(relationship.LastModifiedBy));
    }

    [Fact]
    public void Can_only_reject_creation_when_relationship_is_in_status_pending()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        var acting = () => relationship.Reject(IDENTITY_2, DEVICE_2, null, [relationship]);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Pending)
        );
    }

    [Fact]
    public void Cannot_reject_own_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.Reject(IDENTITY_1, DEVICE_1, null, [relationship]);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse");
    }

    [Fact]
    public void Cannot_reject_foreign_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var foreignAddress = IdentityAddress.ParseUnsafe("some-other-identity");

        // Act
        var acting = () => relationship.Reject(foreignAddress, DeviceId.New(), null, [relationship]);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse");
    }

    [Fact]
    public void P1_active_identity_P1_not_decomposed_P2_decomposed()
    {
        // Arrange
        var existingRelationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);
        existingRelationship.Terminate(IDENTITY_2, DEVICE_2);
        existingRelationship.Decompose(IDENTITY_2, DEVICE_2);

        var newRelationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, null, [existingRelationship]);

        // Act
        var acting = () => newRelationship.Reject(IDENTITY_1, DEVICE_1, [], [existingRelationship]);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.oldRelationshipNotDecomposed");
    }

    [Fact]
    public void P1_active_identity_P1_decomposed_P2_not_decomposed()
    {
        // Arrange
        var existingRelationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);
        existingRelationship.Terminate(IDENTITY_2, DEVICE_2);
        existingRelationship.Decompose(IDENTITY_2, DEVICE_2);

        var newRelationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, null, [existingRelationship]);

        existingRelationship.Decompose(IDENTITY_1, DEVICE_1);

        // Act
        newRelationship.Reject(IDENTITY_1, DEVICE_1, [], [existingRelationship]);

        // Assert
        newRelationship.Status.Should().Be(RelationshipStatus.Rejected);
    }

    [Fact]
    public void P2_active_identity_P1_not_decomposed_P2_decomposed()
    {
        // Arrange
        var existingRelationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);
        existingRelationship.Terminate(IDENTITY_1, DEVICE_1);
        existingRelationship.Decompose(IDENTITY_1, DEVICE_1);

        var newRelationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, null, [existingRelationship]);

        // Act
        var acting = () => newRelationship.Reject(IDENTITY_2, DEVICE_2, [], [existingRelationship]);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.oldRelationshipNotDecomposed");
    }

    [Fact]
    public void P2_active_identity_P1_decomposed_P2_not_decomposed()
    {
        // Arrange
        var existingRelationship = CreateActiveRelationship(IDENTITY_1, IDENTITY_2);
        existingRelationship.Terminate(IDENTITY_1, DEVICE_1);
        existingRelationship.Decompose(IDENTITY_1, DEVICE_1);

        var newRelationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, null, [existingRelationship]);

        existingRelationship.Decompose(IDENTITY_2, DEVICE_2);

        // Act
        newRelationship.Reject(IDENTITY_2, DEVICE_2, [], [existingRelationship]);

        // Assert
        newRelationship.Status.Should().Be(RelationshipStatus.Rejected);
    }
}
