﻿using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Tests.Extensions;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;
public class ReactivateRelationshipTests
{
    [Fact]
    public void Reactivate_relationship_transitions_relationship_to_status_reactivated()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship();

        // Act
        relationship.Reactivate(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Reactivated);
    }

    [Fact]
    public void Reactivating_relationship_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");
        var relationship = CreateTerminatedRelationship();

        // Act
        relationship.Reactivate(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.AuditLog.Should().HaveCount(4);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.ReactivationRequested);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Reactivated);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_2);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_2);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Can_only_reactivate_relationship_when_relationship_is_in_status_terminated()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.Reactivate(IDENTITY_2, DEVICE_2);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Terminated)
        );
    }

    [Fact]
    public void Cannot_reactivate_relationship_that_is_already_reactivated()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship();

        relationship.Reactivate(IDENTITY_2, DEVICE_2);

        // Act
        var acting = () => relationship.Reactivate(IDENTITY_2, DEVICE_2);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Terminated)
        );
    }

    // TODO: Cannot reactivate foreign relationships
}
