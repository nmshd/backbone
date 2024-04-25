using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Tests.Extensions;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;
public class AcceptRelationshipReactivationTests
{
    [Fact]
    public void Accepting_reactivation_transitions_relationship_to_status_active()
    {
        // Arrange
        var relationship = CreateTerminatedRelationshipWithPendingReactivationRequest();

        // Act
        relationship.AcceptReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Active);
    }

    [Fact]
    public void Accepting_reactivation_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreateTerminatedRelationshipWithPendingReactivationRequest();

        // Act
        relationship.AcceptReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        relationship.AuditLog.Should().HaveCount(5);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.AcceptanceOfReactivation);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Active);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_2);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_2);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Can_only_accept_reactivation_when_reactivation_request_has_been_made()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship();

        // Act
        var acting = () => relationship.AcceptReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipRevivalIfNoRequestToDoSoHasBeenMade"
        );
    }

    [Fact]
    public void Can_only_accept_relationship_reactivation_request_addressed_to_self()
    {
        // Arrange
        var relationship = CreateTerminatedRelationshipWithPendingReactivationRequest();

        // Act
        var acting = () => relationship.AcceptReactivation(IDENTITY_1, DEVICE_1);

        // Assert
        acting.Should().Throw<DomainException>()
            .WithError("error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipReactivationRequestAddressedToSomeoneElse");
    }
}
