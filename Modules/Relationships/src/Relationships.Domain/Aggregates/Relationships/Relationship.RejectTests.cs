﻿using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipRejectTests : AbstractTestsBase
{
    [Fact]
    public void Reject_creation_transitions_relationship_to_status_rejected()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Reject(IDENTITY_2, DEVICE_2, null);

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
        relationship.Reject(IDENTITY_2, DEVICE_2, null);

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
        relationship.Reject(IDENTITY_2, DEVICE_2, null);

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
        var acting = () => relationship.Reject(IDENTITY_2, DEVICE_2, null);

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
        var acting = () => relationship.Reject(IDENTITY_1, DEVICE_1, null);

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
        var acting = () => relationship.Reject(foreignAddress, DeviceId.New(), null);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse");
    }
}
