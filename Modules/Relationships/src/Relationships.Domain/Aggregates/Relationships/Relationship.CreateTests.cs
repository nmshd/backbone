using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

public class RelationshipCreateTests : AbstractTestsBase
{
    [Fact]
    public void New_Relationship_Has_Correct_Data()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        // Act
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, [0, 1, 2], []);

        // Assert
        relationship.Id.Should().NotBeNull();
        relationship.From.Should().Be(IDENTITY_1);
        relationship.To.Should().Be(IDENTITY_2);
        relationship.Status.Should().Be(RelationshipStatus.Pending);
        relationship.RelationshipTemplateId.Should().Be(RELATIONSHIP_TEMPLATE_OF_2.Id);
        relationship.RelationshipTemplate.Should().Be(RELATIONSHIP_TEMPLATE_OF_2);
        relationship.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
        relationship.CreationContent.Should().Equal([0, 1, 2]);
    }

    [Fact]
    public void Raises_RelationshipStatusChangedDomainEvent()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        // Act
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, [0, 1, 2], []);

        // Assert
        var domainEvent = relationship.Should().HaveASingleDomainEvent<RelationshipStatusChangedDomainEvent>();
        domainEvent.RelationshipId.Should().Be(relationship.Id);
        domainEvent.NewStatus.Should().Be("Pending");
        domainEvent.Initiator.Should().Be(IDENTITY_1);
        domainEvent.Peer.Should().Be(IDENTITY_2);
    }

    [Fact]
    public void New_relationship_has_AuditLogEntry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        // Act
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_2, IDENTITY_1, DEVICE_1, null, []);

        // Assert
        relationship.AuditLog.Should().HaveCount(1);

        var auditLogEntry = relationship.AuditLog.First();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.Creation);
        auditLogEntry.OldStatus.Should().Be(null);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Pending);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_1);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_1);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Cannot_create_Relationship_to_self()
    {
        // Act
        var acting = () => new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_1, DEVICE_1, null, []);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotSendRelationshipRequestToYourself");
    }

    [Fact]
    public void There_can_only_be_one_active_relationship_between_two_identities()
    {
        // Arrange
        var existingRelationships = new List<Relationship>
        {
            CreateActiveRelationship()
        };

        // Act
        var acting = () => new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, null, existingRelationships);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.relationshipToTargetAlreadyExists");
    }

    [Fact]
    public void Creating_a_new_relationship_is_possible_if_existing_ones_are_rejected_or_revoked()
    {
        // Arrange
        var existingRelationships = new List<Relationship>
        {
            CreateRejectedRelationship(),
            CreateRevokedRelationship()
        };

        // Act
        var acting = () => new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, null, existingRelationships);

        // Assert
        acting.Should().NotThrow<DomainException>();
    }

    [Fact]
    public void Cannot_create_Relationship_if_terminated_Relationship_exists()
    {
        // Arrange
        var existingRelationships = new List<Relationship>
        {
            CreateTerminatedRelationship()
        };

        // Act
        var acting = () => new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, null, existingRelationships);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.relationshipToTargetAlreadyExists");
    }

    [Fact]
    public void A_new_relationship_can_be_created_after_decomposing_the_old_one()
    {
        // Arrange
        var existingRelationships = new List<Relationship>
        {
            CreateDecomposedRelationship(IDENTITY_1, IDENTITY_2)
        };

        // Act
        var newRelationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_1, IDENTITY_2, DEVICE_2, [], existingRelationships);

        // Assert
        newRelationship.Status.Should().Be(RelationshipStatus.Pending);
    }
}
