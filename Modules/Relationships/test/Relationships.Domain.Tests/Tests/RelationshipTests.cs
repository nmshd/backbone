using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Domain.Tests.Extensions;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests;

public class RelationshipTests
{
    private static readonly IdentityAddress FROM_IDENTITY = IdentityAddress.Create([1, 1, 1], "id1");
    private static readonly DeviceId FROM_DEVICE = DeviceId.New();

    private static readonly IdentityAddress TO_IDENTITY = IdentityAddress.Create([2, 2, 2], "id1");
    private static readonly DeviceId TO_DEVICE = DeviceId.New();

    private static readonly RelationshipTemplate RELATIONSHIP_TEMPLATE_OF_FROM = new(FROM_IDENTITY, FROM_DEVICE, 1, null, []);
    private static readonly RelationshipTemplate RELATIONSHIP_TEMPLATE_OF_TO = new(TO_IDENTITY, TO_DEVICE, 1, null, []);

    private static Relationship CreatePendingRelationship()
    {
        return new Relationship(RELATIONSHIP_TEMPLATE_OF_TO, FROM_IDENTITY, FROM_DEVICE, null, []);
    }

    private static Relationship CreateActiveRelationship()
    {
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_TO, FROM_IDENTITY, FROM_DEVICE, null, []);
        relationship.Accept(TO_IDENTITY, TO_DEVICE);
        return relationship;
    }

    #region Revoke Creation

    [Fact]
    public void Revoke_creation_transitions_relationship_to_status_revoked()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Revoke(FROM_IDENTITY, FROM_DEVICE);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Revoked);
    }

    [Fact]
    public void Revoking_creation_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreatePendingRelationship();

        // Act
        relationship.Revoke(FROM_IDENTITY, FROM_DEVICE);

        // Assert
        relationship.AuditLog.Should().HaveCount(2);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.RevocationOfCreation);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Pending);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Revoked);
        auditLogEntry.CreatedBy.Should().Be(FROM_IDENTITY);
        auditLogEntry.CreatedByDevice.Should().Be(FROM_DEVICE);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Can_only_revoke_creation_when_relationship_is_in_status_pending()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        var acting = () => relationship.Revoke(TO_IDENTITY, TO_DEVICE);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.relationshipIsNotInCorrectStatus",
            nameof(RelationshipStatus.Pending)
        );
    }

    [Fact]
    public void Cannot_revoke_own_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        var acting = () => relationship.Revoke(TO_IDENTITY, TO_DEVICE);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotRevokeRelationshipRequestNotCreatedByYourself");
    }

    [Fact]
    public void Cannot_revoke_foreign_relationship_request()
    {
        // Arrange
        var relationship = CreatePendingRelationship();
        var foreignAddress = IdentityAddress.ParseUnsafe("some-other-identity");

        // Act
        var acting = () => relationship.Revoke(foreignAddress, DeviceId.New());

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotRevokeRelationshipRequestNotCreatedByYourself");
    }

    /*
     * Test new relationship possible if existing ones with status rejected/revoked exist
     */

    #endregion

    #region Reject Creation

    [Fact]
    public void Reject_creation_transitions_relationship_to_status_rejected()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Reject(TO_IDENTITY, TO_DEVICE);

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
        relationship.Reject(TO_IDENTITY, TO_DEVICE);

        // Assert
        relationship.AuditLog.Should().HaveCount(2);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.RejectionOfCreation);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Pending);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Rejected);
        auditLogEntry.CreatedBy.Should().Be(TO_IDENTITY);
        auditLogEntry.CreatedByDevice.Should().Be(TO_DEVICE);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Can_only_reject_creation_when_relationship_is_in_status_pending()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        var acting = () => relationship.Reject(TO_IDENTITY, TO_DEVICE);

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
        var acting = () => relationship.Reject(FROM_IDENTITY, FROM_DEVICE);

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
        var acting = () => relationship.Reject(foreignAddress, DeviceId.New());

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse");
    }

    #endregion


    # region Creation

    [Fact]
    public void New_Relationship_Has_Correct_Data()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        // Act
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_TO, FROM_IDENTITY, FROM_DEVICE, [0, 1, 2], []);

        // Assert
        relationship.Id.Should().NotBeNull();
        relationship.From.Should().Be(FROM_IDENTITY);
        relationship.To.Should().Be(TO_IDENTITY);
        relationship.Status.Should().Be(RelationshipStatus.Pending);
        relationship.RelationshipTemplateId.Should().Be(RELATIONSHIP_TEMPLATE_OF_TO.Id);
        relationship.RelationshipTemplate.Should().Be(RELATIONSHIP_TEMPLATE_OF_TO);
        relationship.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
        relationship.CreationContent.Should().Equal([0, 1, 2]);
    }

    [Fact]
    public void New_relationship_has_AuditLogEntry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        // Act
        var relationship = new Relationship(RELATIONSHIP_TEMPLATE_OF_TO, FROM_IDENTITY, FROM_DEVICE, null, []);

        // Assert
        relationship.AuditLog.Should().HaveCount(1);

        var auditLogEntry = relationship.AuditLog.First();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.Creation);
        auditLogEntry.OldStatus.Should().Be(null);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Pending);
        auditLogEntry.CreatedBy.Should().Be(FROM_IDENTITY);
        auditLogEntry.CreatedByDevice.Should().Be(FROM_DEVICE);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Cannot_create_Relationship_to_self()
    {
        // Act
        var acting = () => new Relationship(RELATIONSHIP_TEMPLATE_OF_FROM, FROM_IDENTITY, FROM_DEVICE, null, []);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotSendRelationshipRequestToYourself");
    }

    [Fact]
    public void Cannot_create_multiple_relationships_between_two_identities()
    {
        // Arrange
        var existingRelationships = new List<Relationship>
        {
            CreateActiveRelationship()
        };

        // Act
        var acting = () => new Relationship(RELATIONSHIP_TEMPLATE_OF_FROM, TO_IDENTITY, TO_DEVICE, null, existingRelationships);

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.relationshipToTargetAlreadyExists");
    }

    #endregion

    # region Accept Creation

    [Fact]
    public void Accepting_creation_transitions_relationship_to_status_active()
    {
        // Arrange
        var relationship = CreatePendingRelationship();

        // Act
        relationship.Accept(TO_IDENTITY, TO_DEVICE);

        // Assert
        relationship.Status.Should().Be(RelationshipStatus.Active);
    }

    [Fact]
    public void Accepting_creation_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");

        var relationship = CreatePendingRelationship();

        // Act
        relationship.Accept(TO_IDENTITY, TO_DEVICE);

        // Assert
        relationship.AuditLog.Should().HaveCount(2);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.AcceptanceOfCreation);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Pending);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Active);
        auditLogEntry.CreatedBy.Should().Be(TO_IDENTITY);
        auditLogEntry.CreatedByDevice.Should().Be(TO_DEVICE);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Can_only_accept_creation_when_relationship_is_in_status_pending()
    {
        // Arrange
        var relationship = CreateActiveRelationship();

        // Act
        var acting = () => relationship.Accept(TO_IDENTITY, TO_DEVICE);

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
        var acting = () => relationship.Accept(FROM_IDENTITY, FROM_DEVICE);

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
        var acting = () => relationship.Accept(foreignAddress, DeviceId.New());

        // Assert
        acting.Should().Throw<DomainException>().WithError("error.platform.validation.relationshipRequest.cannotAcceptOrRejectRelationshipRequestAddressedToSomeoneElse");
    }

    # endregion
}
