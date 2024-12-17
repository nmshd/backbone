using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipAcceptReactivationTests : AbstractTestsBase
{
    [Fact]
    public void Accepting_reactivation_transitions_relationship_to_status_active()
    {
        // Arrange
        var relationship = CreateRelationshipWithRequestedReactivation(from: IDENTITY_1, to: IDENTITY_2, reactivationRequestedBy: IDENTITY_1);

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

        var relationship = CreateRelationshipWithRequestedReactivation(from: IDENTITY_1, to: IDENTITY_2, reactivationRequestedBy: IDENTITY_1);

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
    public void Raises_RelationshipReactivationCompletedDomainEvent()
    {
        // Arrange
        var relationship = CreateRelationshipWithRequestedReactivation(IDENTITY_1, IDENTITY_2, IDENTITY_1);
        relationship.ClearDomainEvents();

        // Act
        relationship.AcceptReactivation(IDENTITY_2, DEVICE_2);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<RelationshipReactivationCompletedDomainEvent>();
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.Peer.Should().Be(IDENTITY_1);
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
            "error.platform.validation.relationship.noAcceptableRelationshipReactivationRequestExists"
        );
    }

    [Fact]
    public void Can_only_accept_relationship_reactivation_request_addressed_to_self()
    {
        // Arrange
        var relationship = CreateRelationshipWithRequestedReactivation(from: IDENTITY_1, to: IDENTITY_2, reactivationRequestedBy: IDENTITY_1);

        // Act
        var acting = () => relationship.AcceptReactivation(IDENTITY_1, DEVICE_1);

        // Assert
        acting.Should().Throw<DomainException>()
            .WithError("error.platform.validation.relationship.noAcceptableRelationshipReactivationRequestExists");
    }
}
