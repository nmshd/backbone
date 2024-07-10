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

public class AcceptRelationshipTests : AbstractTestsBase
{
    [Fact]
    public void Accepting_creation_transitions_relationship_to_status_active()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Accept(IDENTITY_2, DEVICE_2, [0], [relationship]);

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
        relationship.Accept(IDENTITY_2, DEVICE_2, [], [relationship]);

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
        var acting = () => relationship.Accept(IDENTITY_2, DEVICE_2, [], [relationship]);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Pending)
        );
    }

    [Fact]
    public void Cannot_accept_own_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.Accept(IDENTITY_1, DEVICE_1, [], [relationship]);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse");
    }

    [Fact]
    public void Cannot_accept_foreign_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var foreignAddress = IdentityAddress.ParseUnsafe("some-other-identity");

        // Act
        var acting = () => relationship.Accept(foreignAddress, DeviceId.New(), [], [relationship]);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse");
    }

    [Fact]
    public void Raises_RelationshipStatusChangedDomainEvent()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Accept(IDENTITY_2, DEVICE_2, [], [relationship]);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.NewStatus.Should().Be(relationship.Status.ToString());
        domainEvent.Initiator.Should().Be(relationship.LastModifiedBy);
        domainEvent.Peer.Should().Be(relationship.GetPeerOf(relationship.LastModifiedBy));
    }

    [Fact]
    public void P1_active_identity_P1_not_decomposed_P2_decomposed()
    {
        // Arrange
        var existingRelationships = CreateRelationships();

        existingRelationships.First().Terminate(IDENTITY_2, DEVICE_2);
        existingRelationships.First().Decompose(IDENTITY_2, DEVICE_2);

        var newRelationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, null, existingRelationships);

        // Act
        var acting = () => newRelationship.Accept(IDENTITY_1, DEVICE_1, [], existingRelationships);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.oldRelationshipNotDecomposed");
    }

    [Fact]
    public void P1_active_identity_P1_decomposed_P2_not_decomposed()
    {
        // Arrange
        var existingRelationships = CreateRelationships();

        existingRelationships.First().Terminate(IDENTITY_2, DEVICE_2);
        existingRelationships.First().Decompose(IDENTITY_2, DEVICE_2);

        var newRelationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, null, existingRelationships);

        existingRelationships.First().Decompose(IDENTITY_1, DEVICE_1);

        // Act
        newRelationship.Accept(IDENTITY_1, DEVICE_1, [], existingRelationships);

        // Assert
        newRelationship.Status.Should().Be(RelationshipStatus.Active);
    }

    [Fact]
    public void P2_active_identity_P1_not_decomposed_P2_decomposed()
    {
        // Arrange
        var existingRelationships = CreateRelationships();

        existingRelationships.First().Terminate(IDENTITY_1, DEVICE_1);
        existingRelationships.First().Decompose(IDENTITY_1, DEVICE_1);

        var newRelationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, null, existingRelationships);

        // Act
        var acting = () => newRelationship.Accept(IDENTITY_2, DEVICE_2, [], existingRelationships);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.oldRelationshipNotDecomposed");
    }

    [Fact]
    public void P2_active_identity_P1_decomposed_P2_not_decomposed()
    {
        // Arrange
        var existingRelationships = CreateRelationships();

        existingRelationships.First().Terminate(IDENTITY_1, DEVICE_1);
        existingRelationships.First().Decompose(IDENTITY_1, DEVICE_1);

        var newRelationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, null, existingRelationships);

        existingRelationships.First().Decompose(IDENTITY_2, DEVICE_2);

        // Act
        newRelationship.Accept(IDENTITY_2, DEVICE_2, [], existingRelationships);

        // Assert
        newRelationship.Status.Should().Be(RelationshipStatus.Active);
    }

    private static List<Relationship> CreateRelationships()
    {
        var existingRelationships = new List<Relationship>
        {
            CreateActiveRelationship(IDENTITY_1, IDENTITY_2)
        };
        return existingRelationships;
    }
}
